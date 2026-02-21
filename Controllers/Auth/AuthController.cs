using QUANTM.Controllers.Common;
using QUANTM.DTOs.Auth;
using QUANTM.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QUANTM.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [Authorize(Roles = "SA,ADM")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.Status != 200)
            {
                return StatusCode(result.Status, result);
            }
            return Ok(result);
        }

        [HttpPost("login-microsoft")]
        public async Task<IActionResult> LoginMicrosoft([FromBody] MicrosoftLoginRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginWithMicrosoftAsync(request, ipAddress, userAgent);

            if (result.Status != 200)
            {
                if (result.Status == 401) return CResponseUnauthorized(result.Message);
                if (result.Status == 409) return CResponseAlreadyLoggedIn();
                return StatusCode(result.Status, result);
            }

            // Set Cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // SameSite=None strictly requires Secure=true
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(240),
            };

            Response.Cookies.Append("X-Access-Token", result.Data!.Token, cookieOptions);

            return CResponseLoginWithCookieSuccessful(result.Data.User);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginAsync(request, ipAddress, userAgent);

            if (result.Status != 200)
            {
                if (result.Status == 401) return CResponseUnauthorized(result.Message);
                if (result.Status == 409) return CResponseAlreadyLoggedIn();
                return StatusCode(result.Status, result);
            }

            // Set Cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // SameSite=None strictly requires Secure=true
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(240),
            };

            Response.Cookies.Append("X-Access-Token", result.Data!.Token, cookieOptions);

            return CResponseLoginWithCookieSuccessful(result.Data.User);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(token))
            {
                return CResponseUnauthorized("No token provided");
            }

            var result = await _authService.LogoutAsync(token);
            if (result.Status != 200)
            {
                return StatusCode(result.Status, result);
            }

            Response.Cookies.Delete("X-Access-Token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // SameSite=None strictly requires Secure=true
                SameSite = SameSiteMode.None
            });

            return Ok(result);
        }

        [Authorize(Roles = "SA,ADM")]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions([FromQuery] SessionListParamsDto sessionListParamsDto)
        {
            var result = await _authService.GetSessionsAsync(sessionListParamsDto);
            if (result.Status != 200)
            {
                return StatusCode(result.Status, result);
            }
            return Ok(result);
        }

        [Authorize(Roles = "SA,ADM")]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAllSessions()
        {
            var result = await _authService.LogoutAllSessionsAsync();
            if (result.Status != 200)
            {
                return StatusCode(result.Status, result);
            }

            Response.Cookies.Delete("X-Access-Token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // SameSite=None strictly requires Secure=true
                SameSite = SameSiteMode.None
            });

            return Ok(result);
        }

        [Authorize(Roles = "SA,ADM")]
        [HttpDelete("logout-session/{sessionId}")]
        public async Task<IActionResult> LogoutSpecificSession(int sessionId)
        {
            var result = await _authService.LogoutSpecificSessionAsync(sessionId);
            if (result.Status != 200)
            {
                return StatusCode(result.Status, result);
            }
            return Ok(result);
        }
    }
}