using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using QUANTM.Data;
using QUANTM.Models.Audit;
using Microsoft.EntityFrameworkCore;

namespace QUANTM.Services
{
    public class AuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string? entityType = null, int? entityId = null,
            object? oldValues = null, object? newValues = null, int? userId = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Get user ID from parameter or JWT token
            var logUserId = userId ?? GetUserIdFromToken(httpContext);

            var auditLog = new AuditLog
            {
                UserId = logUserId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                IpAddress = GetIpAddress(httpContext),
                UserAgent = GetUserAgent(httpContext),
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        private int? GetUserIdFromToken(HttpContext? httpContext)
        {
            if (httpContext == null) return null;

            var authHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            try
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                    return userId;
            }
            catch
            {
                // Invalid token, return null
            }

            return null;
        }

        private string? GetIpAddress(HttpContext? httpContext)
        {
            return httpContext?.Connection.RemoteIpAddress?.ToString();
        }

        private string? GetUserAgent(HttpContext? httpContext)
        {
            return httpContext?.Request.Headers["User-Agent"].ToString();
        }
    }
}
