
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(120)]
        public string Fullname { get; set; } = null!;

        [Required]
        [MaxLength(12)]
        [JsonPropertyName("id_no")]
        public string IdNo { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        [JsonPropertyName("role_code")]
        public string RoleCode { get; set; } = null!;
    }
}

