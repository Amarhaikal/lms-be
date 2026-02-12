using System.Security.Claims;

namespace QUANTM.Services.Auth
{
    public class IdentityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user?.FindFirst("sub")?.Value;

            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
