using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QUANTM.Data;
using QUANTM.DTOs.Auth;
using QUANTM.DTOs.Session;
using QUANTM.DTOs.User;
using QUANTM.Model.Common;
using QUANTM.Models.Auth;
using QUANTM.Models.User;

namespace QUANTM.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtService _jwtService;
        private readonly AuditService _auditService;
        private readonly PasswordPolicyService _passwordPolicyService;
        private readonly EmailService _emailService;
        private readonly EncryptionService _encryptionService;
        private readonly IdentityService _identityService;
        private readonly IWebHostEnvironment _environment;

        public AuthService(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<AuthService> logger,
            JwtService jwtService,
            AuditService auditService,
            PasswordPolicyService passwordPolicyService,
            EmailService emailService,
            EncryptionService encryptionService,
            IdentityService identityService,
            IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _jwtService = jwtService;
            _auditService = auditService;
            _passwordPolicyService = passwordPolicyService;
            _emailService = emailService;
            _encryptionService = encryptionService;
            _identityService = identityService;
            _environment = environment;
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var existingUserName = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                var existingUserEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                var rawIdNo = request.IdNo;
                var idNoHash = _encryptionService.Hash(rawIdNo);
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.IdNoHash == idNoHash);

                if (existingUserName != null)
                {
                    return new ApiResponse<object> { Status = 400, Message = "Username already exists" };
                }

                if (existingUserEmail != null)
                {
                    return new ApiResponse<object> { Status = 400, Message = "Email already exists" };
                }

                if (existingUser != null)
                {
                    return new ApiResponse<object> { Status = 400, Message = "ID Number already exists" };
                }

                // Validate password policy
                var passwordValidation = _passwordPolicyService.ValidatePassword(request.Password, request.Username);

                if (!passwordValidation.IsValid)
                {
                    return new ApiResponse<object>
                    {
                        Status = 400,
                        Message = "Password does not meet requirements",
                        Data = new
                        {
                            Errors = passwordValidation.Errors,
                            Strength = _passwordPolicyService.CalculatePasswordStrength(request.Password)
                        }
                    };
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var statusNewUser = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == "NEW");

                // Determine gender from last character of IdNo
                string genderCode = "M"; // Default to Male
                if (!string.IsNullOrEmpty(rawIdNo))
                {
                    char lastChar = rawIdNo[rawIdNo.Length - 1];
                    if (char.IsDigit(lastChar))
                    {
                        int lastDigit = int.Parse(lastChar.ToString());
                        genderCode = lastDigit % 2 == 0 ? "F" : "M";
                    }
                }
                var gender = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == genderCode);

                var role = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == request.RoleCode);

                var newUser = new User
                {
                    Fullname = request.Fullname,
                    IdNo = _encryptionService.Encrypt(request.IdNo), // Encrypt for storage
                    IdNoHash = _encryptionService.Hash(request.IdNo), // Hash for searching
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    PasswordChangedAt = DateTime.UtcNow,
                    RoleId = role?.Id ?? 0,
                    StatusId = statusNewUser?.Id ?? 0,
                    GenderId = gender?.Id,
                    CreatedBy = _identityService.GetUserId(),
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

                return new ApiResponse<object> { Status = 200, Message = "Registration successful" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object> { Status = 500, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<LoginResponseData>> LoginAsync(LoginRequest request, string ipAddress, string userAgent)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Status)
                    .Include(u => u.Gender)
                    .Include(u => u.Address).ThenInclude(a => a!.Country)
                    .Include(u => u.Address).ThenInclude(a => a!.State)
                    .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

                if (user == null)
                {
                    await _auditService.LogAsync("LOGIN_FAILED", "User", null);
                    return new ApiResponse<LoginResponseData> { Status = 401, Message = "Invalid username or password" };
                }

                var deviceType = GetDeviceType(userAgent);

                // Check if account is suspended
                if (user.Status?.Code == "SUSPENDED")
                {
                    return new ApiResponse<LoginResponseData> { Status = 401, Message = "Account has been suspended. Please contact administrator." };
                }

                // Check if account is locked
                if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.UtcNow)
                {
                    var minutesLeft = (user.LockedUntil.Value - DateTime.UtcNow).TotalMinutes;
                    return new ApiResponse<LoginResponseData> { Status = 401, Message = $"Account locked. Try again in {Math.Ceiling(minutesLeft)} minutes." };
                }

                // Verify password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if (!isPasswordValid)
                {
                    user.FailedLoginAttempts++;

                    if (user.FailedLoginAttempts >= 5)
                    {
                        var suspendedStatus = await _context.SystemCodes.FirstOrDefaultAsync(s => s.Code == "SUSPENDED");
                        if (suspendedStatus != null)
                        {
                            var oldStatusId = user.StatusId;
                            user.StatusId = suspendedStatus.Id;
                            await _context.SaveChangesAsync();

                            await _auditService.LogAsync("ACCOUNT_SUSPENDED", "User", user.Id,
                                oldValues: new { StatusId = oldStatusId, StatusCode = user.Status?.Code },
                                newValues: new { StatusId = suspendedStatus.Id, StatusCode = "SUSPENDED" },
                                userId: user.Id);

                            _ = Task.Run(async () =>
                            {
                                try
                                {
                                    await _emailService.SendAccountSuspendedEmailAsync(user.Email, user.Fullname);
                                    var adminUsers = await _context.Users
                                        .Include(u => u.Role)
                                        .Where(u => u.Role != null && (u.Role.Code == "ADM" || u.Role.Code == "SA"))
                                        .ToListAsync();

                                    foreach (var admin in adminUsers)
                                    {
                                        await _emailService.SendAccountSuspendedAdminAlertAsync(admin.Email, admin.Fullname, user.Username, user.Email, ipAddress ?? "Unknown");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Failed to send account suspension emails for {Email}", user.Email);
                                }
                            });
                        }
                        return new ApiResponse<LoginResponseData> { Status = 401, Message = "Account has been suspended due to multiple failed login attempts. Please contact administrator." };
                    }

                    await _context.SaveChangesAsync();
                    await _auditService.LogAsync("LOGIN_FAILED", "User", user.Id, userId: user.Id);
                    return new ApiResponse<LoginResponseData> { Status = 401, Message = "Invalid username or password" };
                }

                // Reset failed login attempts
                user.FailedLoginAttempts = 0;
                user.LockedUntil = null;

                // Check for active session
                var activeSession = await _context.Sessions
                    .FirstOrDefaultAsync(s => s.UserId == user.Id && s.IsActive && s.ExpiresAt > DateTime.UtcNow);

                if (activeSession != null)
                {
                    return new ApiResponse<LoginResponseData> { Status = 409, Message = "Your account is already logged in from another session. Please logout from the other device first." };
                }

                // Generate JWT token
                var (token, jti) = _jwtService.GenerateToken(user.Id, user.Username, user.RoleId, user.Role?.Code ?? "");

                // Check for new IP
                var knownIps = await _context.Sessions
                    .Where(s => s.UserId == user.Id && s.IpAddress != null)
                    .Select(s => s.IpAddress)
                    .Distinct()
                    .ToListAsync();

                bool isNewIp = !knownIps.Contains(ipAddress);

                // Create session
                var newSession = new Models.Session.Session
                {
                    UserId = user.Id,
                    TokenJti = jti,
                    SessionDuration = 10,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(240),
                    IsActive = true,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    DeviceType = deviceType,
                    LastActivityAt = DateTime.UtcNow
                };

                _context.Sessions.Add(newSession);
                await _context.SaveChangesAsync();

                // Log audit
                await _auditService.LogAsync("USER_LOGIN", "User", user.Id, userId: user.Id);

                // Send email alert
                if (isNewIp && !_environment.IsDevelopment())
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendNewLoginAlertAsync(user.Email, user.Fullname, ipAddress ?? "Unknown", deviceType);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send new login alert to {Email}", user.Email);
                        }
                    });
                }

                var userDto = _mapper.Map<UserDetailsDto>(user);
                userDto.IdNo = _encryptionService.Decrypt(userDto.IdNo);

                return new ApiResponse<LoginResponseData>
                {
                    Status = 200,
                    Message = "Login successful",
                    Data = new LoginResponseData { Token = token, User = userDto }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponseData> { Status = 500, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<string>> LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return new ApiResponse<string> { Status = 401, Message = "No token provided" };
                }

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrEmpty(jti))
                {
                    return new ApiResponse<string> { Status = 401, Message = "Invalid token" };
                }

                var session = await _context.Sessions.FirstOrDefaultAsync(s => s.TokenJti == jti && s.IsActive);
                if (session == null)
                {
                    return new ApiResponse<string> { Status = 401, Message = "Session not found or already logged out" };
                }

                session.IsActive = false;
                session.LoggedOutAt = DateTime.UtcNow;
                session.LogoutReason = "Manual";
                await _context.SaveChangesAsync();

                await _auditService.LogAsync("USER_LOGOUT", "User", session.UserId, userId: session.UserId);

                return new ApiResponse<string> { Status = 200, Message = "Logout successful" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Status = 500, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<List<SessionDto>>> GetActiveSessionsAsync(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return new ApiResponse<List<SessionDto>> { Status = 400, Message = "Username is required" };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return new ApiResponse<List<SessionDto>> { Status = 404, Message = "User not found" };
                }

                var activeSessions = await _context.Sessions
                    .Where(s => s.UserId == user.Id && s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                var sessionDtos = _mapper.Map<List<SessionDto>>(activeSessions);

                return new ApiResponse<List<SessionDto>>
                {
                    Status = 200,
                    Message = "Data retrieved successfully",
                    Data = sessionDtos
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<SessionDto>> { Status = 500, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<string>> LogoutAllSessionsAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
                if (user == null)
                {
                    return new ApiResponse<string> { Status = 401, Message = "Invalid username or password" };
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (!isPasswordValid)
                {
                    return new ApiResponse<string> { Status = 401, Message = "Invalid username or password" };
                }

                var activeSessions = await _context.Sessions.Where(s => s.UserId == user.Id && s.IsActive).ToListAsync();
                foreach (var session in activeSessions)
                {
                    session.IsActive = false;
                    session.LoggedOutAt = DateTime.UtcNow;
                    session.LogoutReason = "LogoutAll";
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<string> { Status = 200, Message = $"Successfully logged out {activeSessions.Count} session(s)" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Status = 500, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<string>> LogoutSpecificSessionAsync(int sessionId, string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username || u.Email == username);
                if (user == null)
                {
                    return new ApiResponse<string> { Status = 401, Message = "Invalid username or password" };
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                if (!isPasswordValid)
                {
                    return new ApiResponse<string> { Status = 401, Message = "Invalid username or password" };
                }

                var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == user.Id);
                if (session == null)
                {
                    return new ApiResponse<string> { Status = 404, Message = "Session not found" };
                }

                session.IsActive = false;
                session.LoggedOutAt = DateTime.UtcNow;
                session.LogoutReason = "Manual";
                await _context.SaveChangesAsync();

                return new ApiResponse<string> { Status = 200, Message = "Session logged out successfully" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string> { Status = 500, Message = $"An error occurred: {ex.Message}" };
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
