using System.Text.Json.Serialization;
using QUANTM.DTOs.User;

namespace QUANTM.DTOs.Auth
{
    public class LoginResponseData
    {
        public string Token { get; set; } = null!;
        public UserDetailsDto User { get; set; } = null!;
    }

    public class LoginResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = null!;
        public LoginResponseData? Data { get; set; }
    }
}
