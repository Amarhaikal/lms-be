using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Auth
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username or email is required")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = null!;
    }
}
