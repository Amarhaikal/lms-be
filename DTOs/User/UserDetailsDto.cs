using System.Text.Json.Serialization;
using LMS.DTOs.Parameter;

namespace LMS.DTOs.User
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = null!;

        [JsonPropertyName("id_no")]
        public string IdNo { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        [JsonPropertyName("role")]
        public SystemCodeNestedDto? Role { get; set; }
    }
}