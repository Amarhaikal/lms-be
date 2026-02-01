using AutoMapper;
using QUANTM.Controllers.Common;
using QUANTM.Data;
using QUANTM.DTOs.Auth;
using QUANTM.DTOs.User;
using QUANTM.Model.Common;
using QUANTM.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QUANTM.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly QUANTM.Services.Auth.JwtService _jwtService;
        private readonly QUANTM.Services.Auth.AuditService _auditService;
        private readonly QUANTM.Services.Auth.PasswordPolicyService _passwordPolicyService;
        private readonly QUANTM.Services.Auth.EmailService _emailService;
        private readonly QUANTM.Services.Auth.EncryptionService _encryptionService;
        private readonly IWebHostEnvironment _environment;

        public AuthController(ApplicationDbContext context, IMapper mapper, ILogger<AuthController> logger,
            QUANTM.Services.Auth.JwtService jwtService, QUANTM.Services.Auth.AuditService auditService,
            QUANTM.Services.Auth.PasswordPolicyService passwordPolicyService, QUANTM.Services.Auth.EmailService emailService,
            QUANTM.Services.Auth.EncryptionService encryptionService, IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _jwtService = jwtService;
            _auditService = auditService;
            _passwordPolicyService = passwordPolicyService;
            _emailService = emailService;
            _encryptionService = encryptionService;
            _environment = environment;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existingUserName = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                var existingUserEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                var idNoHash = _encryptionService.Hash(request.IdNo);
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.IdNoHash == idNoHash);

                if (existingUserName != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Username already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                if (existingUserEmail != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "Email already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                if (existingUser != null)
                {
                    var response = new ApiResponse<string>
                    {
                        Status = 400,
                        Message = "ID Number already exists",
                        Data = null
                    };
                    return BadRequest(response);
                }

                // Validate password policy
                var passwordValidation = _passwordPolicyService.ValidatePassword(
                    request.Password,
                    request.Username
                );

                if (!passwordValidation.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Status = 400,
                        Message = "Password does not meet requirements",
                        Data = new
                        {
                            Errors = passwordValidation.Errors,
                            Strength = _passwordPolicyService.CalculatePasswordStrength(request.Password)
                        }
                    });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var statusNewUser = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == "NEW");

                var newUser = new Models.User.User
                {
                    Fullname = request.Fullname,
                    IdNo = _encryptionService.Encrypt(request.IdNo), // Encrypt for storage
                    IdNoHash = _encryptionService.Hash(request.IdNo), // Hash for searching
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    PasswordChangedAt = DateTime.UtcNow,
                    RoleId = request.RoleId,
                    StatusId = statusNewUser?.Id ?? 0,
                    CreatedBy = null,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHistories = new List<PasswordHistory>
                    {
                        new PasswordHistory
                        {
                            PasswordHash = hashedPassword,
                            CreatedAt = DateTime.UtcNow
                        }
                    }
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Send email in background (fire-and-forget) - only in production
                if (!_environment.IsDevelopment())
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendRegisterSuccessEmailAsync(request.Email, request.Username);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send registration email to {Email}", request.Email);
                        }
                    });
                }

                return CResponseRegisterSuccessful();

            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Find user by username or email
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Status)
                    .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

                if (user == null)
                {
                    await _auditService.LogAsync("LOGIN_FAILED", "User", null);
                    return CResponseUnauthorized("Invalid username or password");
                }

                // Get device information from request (needed for email notifications)
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var deviceType = GetDeviceType(userAgent);

                // Check if account is suspended
                if (user.Status?.Code == "SUSPENDED")
                {
                    return CResponseUnauthorized("Account has been suspended. Please contact administrator.");
                }

                // Check if account is locked (temporary lock - kept for backward compatibility)
                if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
                {
                    var minutesLeft = (user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
                    return CResponseUnauthorized($"Account locked. Try again in {Math.Ceiling(minutesLeft)} minutes.");
                }

                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    // Increment failed login attempts
                    user.FailedLoginAttempts++;

                    // Suspend account after 5 failed attempts
                    if (user.FailedLoginAttempts >= 5)
                    {
                        // Get SUSPENDED status code
                        var suspendedStatus = await _context.SystemCodes
                            .FirstOrDefaultAsync(s => s.Code == "SUSPENDED");

                        if (suspendedStatus != null)
                        {
                            var oldStatusId = user.StatusId;
                            user.StatusId = suspendedStatus.Id;
                            await _context.SaveChangesAsync();

                            await _auditService.LogAsync("ACCOUNT_SUSPENDED", "User", user.Id,
                                oldValues: new { StatusId = oldStatusId, StatusCode = user.Status?.Code },
                                newValues: new { StatusId = suspendedStatus.Id, StatusCode = "SUSPENDED" },
                                userId: user.Id);

                            // Send suspension emails in background
                            var userEmail = user.Email;
                            var userFullname = user.Fullname;
                            var userName = user.Username;
                            var userIp = ipAddress ?? "Unknown";

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _emailService.SendAccountSuspendedEmailAsync(userEmail, userFullname);

                                    // Get all admin users (ADM + SA roles)
                                    var adminUsers = await _context.Users
                                        .Include(u => u.Role)
                                        .Where(u => u.Role != null && (u.Role.Code == "ADM" || u.Role.Code == "SA"))
                                        .ToListAsync();

                                    // Send alert to all admins
                                    foreach (var admin in adminUsers)
                                    {
                                        await _emailService.SendAccountSuspendedAdminAlertAsync(
                                            admin.Email,
                                            admin.Fullname,
                                            userName,
                                            userEmail,
                                            userIp
                                        );
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to send account suspension emails for {Email}", userEmail);
                                }
                            });
                        }

                        return CResponseUnauthorized("Account has been suspended due to multiple failed login attempts. Please contact administrator.");
                    }

                    await _context.SaveChangesAsync();
                    await _auditService.LogAsync("LOGIN_FAILED", "User", user.Id, userId: user.Id);
                    return CResponseUnauthorized("Invalid username or password");
                }

                // Reset failed login attempts on successful login
                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;

                // Check if user already has an active session
                var activeSession = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.UserId == user.Id && s.IsActive && s.ExpiresAt > DateTime.UtcNow);

                if (activeSession != null)
                {
                    return CResponseAlreadyLoggedIn();
                }

                // Generate JWT token
                var (token, jti) = _jwtService.GenerateToken(
                    user.Id,
                    user.Username,
                    user.Email,
                    user.RoleId,
                    user.Role?.Code
                );

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Prevents JavaScript access (XSS protection)
                    Secure = !_environment.IsDevelopment(),  // Only send over HTTPS (set to false if testing locally without HTTPS),
                    SameSite = SameSiteMode.Strict, // CSRF protection to prevent token theft
                    Expires = DateTimeOffset.UtcNow.AddMinutes(10), // Session duration
                };

                Response.Cookies.Append("X-Access-Token", token, cookieOptions);

                // Check for new IP BEFORE creating session
                var knownIps = await _context.Sessions
                    .Where(s => s.UserId == user.Id && s.IpAddress != null)
                    .Select(s => s.IpAddress)
                    .Distinct()
                    .ToListAsync();

                bool isNewIp = !knownIps.Contains(ipAddress);

                // Create new session record
                var newSession = new Models.Session.Session
                {
                    UserId = user.Id,
                    TokenJti = jti,
                    SessionDuration = 10,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    IsActive = true,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    DeviceType = deviceType,
                    LastActivityAt = DateTime.UtcNow
                };

                _context.Sessions.Add(newSession);
                await _context.SaveChangesAsync();

                // Log successful login
                await _auditService.LogAsync("USER_LOGIN", "User", user.Id, userId: user.Id);

                // Send email notification if new IP detected (in background) - only in production
                if (isNewIp && !_environment.IsDevelopment())
                {
                    var userEmail = user.Email;
                    var userFullname = user.Fullname;
                    var userIp = ipAddress ?? "Unknown";
                    var userDevice = deviceType;

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendNewLoginAlertAsync(userEmail, userFullname, userIp, userDevice);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send new login alert to {Email}", userEmail);
                        }
                    });
                }

                // Map user to DTO
                var userDto = _mapper.Map<UserDetailsDto>(user);
                userDto.IdNo = _encryptionService.Decrypt(userDto.IdNo);

                return CResponseLoginWithCookieSuccessful(userDto);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = HttpContext.Request.Cookies["X-Access-Token"];

                if (string.IsNullOrEmpty(token))
                {
                    return CResponseUnauthorized("No token provided");
                }

                // Decode JWT token to get JTI
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrEmpty(jti))
                {
                    return CResponseUnauthorized("Invalid token");
                }

                // Find and deactivate session by JTI
                var session = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.TokenJti == jti && s.IsActive);

                if (session == null)
                {
                    return CResponseUnauthorized("Session not found or already logged out");
                }

                // Deactivate session
                session.IsActive = false;
                session.LoggedOutAt = DateTime.UtcNow;
                session.LogoutReason = "Manual";
                await _context.SaveChangesAsync();

                // Log audit
                await _auditService.LogAsync("USER_LOGOUT", "User", session.UserId, userId: session.UserId);

                Response.Cookies.Delete("X-Access-Token", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !_environment.IsDevelopment(),
                    SameSite = SameSiteMode.Strict
                });

                var response = new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Logout successful"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetActiveSessions()
        {
            try
            {
                // Get user ID from JWT token (you'll need to add authentication middleware)
                // For now, we'll require username in query parameter
                var username = HttpContext.Request.Query["username"].ToString();

                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { Status = 400, Message = "Username is required" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return CResponseNotFound();
                }

                var activeSessions = await _context.Sessions
                    .Where(s => s.UserId == user.Id && s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                var sessionDtos = _mapper.Map<List<QUANTM.DTOs.Session.SessionDto>>(activeSessions);

                return CResponseGetListSuccessful(sessionDtos);
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAllSessions([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

                if (user == null)
                {
                    return CResponseUnauthorized("Invalid username or password");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    return CResponseUnauthorized("Invalid username or password");
                }

                // Deactivate all active sessions
                var activeSessions = await _context.Sessions
                    .Where(s => s.UserId == user.Id && s.IsActive)
                    .ToListAsync();

                foreach (var session in activeSessions)
                {
                    session.IsActive = false;
                    session.LoggedOutAt = DateTime.UtcNow;
                    session.LogoutReason = "LogoutAll";
                }

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<string>
                {
                    Status = 200,
                    Message = $"Successfully logged out {activeSessions.Count} session(s)"
                });
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        [HttpPost("logout-session/{sessionId}")]
        public async Task<IActionResult> LogoutSpecificSession(int sessionId, [FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

                if (user == null)
                {
                    return CResponseUnauthorized("Invalid username or password");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    return CResponseUnauthorized("Invalid username or password");
                }

                var session = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == user.Id);

                if (session == null)
                {
                    return CResponseNotFound();
                }

                session.IsActive = false;
                session.LoggedOutAt = DateTime.UtcNow;
                session.LogoutReason = "Manual";
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<string>
                {
                    Status = 200,
                    Message = "Session logged out successfully"
                });
            }
            catch (Exception ex)
            {
                return CResponseException(ex.Message);
            }
        }

        private string GetDeviceType(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Unknown";

            userAgent = userAgent.ToLower();
            if (userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone"))
                return "Mobile";
            if (userAgent.Contains("tablet") || userAgent.Contains("ipad"))
                return "Tablet";
            return "Desktop";
        }
    }
}