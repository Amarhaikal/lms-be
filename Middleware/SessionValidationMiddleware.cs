using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using LMS.Data;

namespace LMS.Middleware
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Skip validation for public endpoints
            var path = context.Request.Path.Value?.ToLower();
            if (path == "/api/auth/login" ||
                path == "/api/auth/register" ||
                path == "/" ||
                path == "/api/auth/logout" ||
                path == "/api/auth/logout-all")
            {
                await _next(context);
                return;
            }

            // Extract token from Authorization header
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await _next(context);
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Decode JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Extract JTI claim
                var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(jti))
                {
                    // Check if session is still active
                    var session = await dbContext.Sessions
                        .FirstOrDefaultAsync(s => s.TokenJti == jti && s.IsActive);

                    if (session == null)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Status = 401,
                            Message = "Session has been logged out or expired"
                        });
                        return;
                    }

                    // Check if session has expired
                    if (session.ExpiresAt < DateTime.UtcNow)
                    {
                        session.IsActive = false;
                        session.LogoutReason = "Expired";
                        session.LoggedOutAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Status = 401,
                            Message = "Session has expired"
                        });
                        return;
                    }

                    // Check for inactivity timeout (15 minutes)
                    if (session.LastActivityAt.HasValue &&
                        session.LastActivityAt.Value < DateTime.UtcNow.AddMinutes(-15))
                    {
                        session.IsActive = false;
                        session.LogoutReason = "Inactivity Timeout";
                        session.LoggedOutAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Status = 401,
                            Message = "Session timed out due to inactivity. Please login again."
                        });
                        return;
                    }

                    // Update last activity time
                    session.LastActivityAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                // Invalid token format, let the authentication middleware handle it
            }

            await _next(context);
        }
    }
}
