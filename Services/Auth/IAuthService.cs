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
        Task<ApiResponse<object>> GetActiveSessionsAsync(SessionListParamsDto sessionListParamsDto);
        Task<ApiResponse<string>> LogoutAllSessionsAsync();
        Task<ApiResponse<string>> LogoutSpecificSessionAsync(int sessionId);
    }
}
