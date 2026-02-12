using QUANTM.DTOs.Auth;
using QUANTM.DTOs.User;
using QUANTM.Model.Common;
using QUANTM.DTOs.Session;

namespace QUANTM.Services.Auth
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<LoginResponseData>> LoginAsync(LoginRequest request, string ipAddress, string userAgent);
        Task<ApiResponse<string>> LogoutAsync(string token);
        Task<ApiResponse<List<SessionDto>>> GetActiveSessionsAsync(string username);
        Task<ApiResponse<string>> LogoutAllSessionsAsync(string username, string password);
        Task<ApiResponse<string>> LogoutSpecificSessionAsync(int sessionId, string username, string password);
    }
}
