using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using QUANTM.Data;
using QUANTM.Models.Audit;
using Microsoft.EntityFrameworkCore;

namespace QUANTM.Services.Auth
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

        public virtual async Task LogAsync(string action, string? entityType = null, int? entityId = null,
            object? oldValues = null, object? newValues = null, int? userId = null,
            string? module = null, string? status = "Success", string? errorMessage = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Get user ID from parameter or JWT token
            var logUserId = userId ?? GetUserIdFromToken(httpContext);

            var (oldDiff, newDiff) = GetDifferences(oldValues, newValues);

            var auditLog = new AuditLog
            {
                UserId = logUserId,
                Action = action,
                Module = module,
                Status = status,
                ErrorMessage = errorMessage,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldDiff,
                NewValues = newDiff,
                IpAddress = GetIpAddress(httpContext),
                UserAgent = GetUserAgent(httpContext),
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        private (string? OldDiff, string? NewDiff) GetDifferences(object? oldValues, object? newValues)
        {
            if (oldValues == null || newValues == null)
            {
                return (
                    oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    newValues != null ? JsonSerializer.Serialize(newValues) : null
                );
            }

            try
            {
                var oldNode = JsonSerializer.SerializeToNode(oldValues) as JsonObject;
                var newNode = JsonSerializer.SerializeToNode(newValues) as JsonObject;

                if (oldNode == null || newNode == null)
                {
                    return (JsonSerializer.Serialize(oldValues), JsonSerializer.Serialize(newValues));
                }

                var oldDiff = new JsonObject();
                var newDiff = new JsonObject();
                bool hasChanges = false;

                var ignoredProperties = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "created_at", "createdat", "created_by", "createdby",
                    "updated_at", "updatedat", "updated_by", "updatedby"
                };

                foreach (var prop in newNode)
                {
                    var propName = prop.Key;
                    var newVal = prop.Value;

                    if (ignoredProperties.Contains(propName)) continue;

                    if (oldNode.TryGetPropertyValue(propName, out var oldVal))
                    {
                        // Compare serialized values
                        if (JsonSerializer.Serialize(oldVal) != JsonSerializer.Serialize(newVal))
                        {
                            oldDiff.Add(propName, oldVal?.DeepClone());
                            newDiff.Add(propName, newVal?.DeepClone());
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        newDiff.Add(propName, newVal?.DeepClone());
                        hasChanges = true;
                    }
                }

                foreach (var prop in oldNode)
                {
                    var propName = prop.Key;
                    if (ignoredProperties.Contains(propName)) continue;

                    if (!newNode.ContainsKey(propName))
                    {
                        oldDiff.Add(propName, prop.Value?.DeepClone());
                        hasChanges = true;
                    }
                }

                if (!hasChanges) return (null, null);

                return (oldDiff.ToJsonString(), newDiff.ToJsonString());
            }
            catch
            {
                // Fallback to full serialization if an error occurs
                return (JsonSerializer.Serialize(oldValues), JsonSerializer.Serialize(newValues));
            }
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
