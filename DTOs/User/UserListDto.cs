using System.Text.Json.Serialization;
using QUANTM.DTOs.Parameter;
using QUANTM.DTOs.Common;

namespace QUANTM.DTOs.User
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = null!;

        [JsonPropertyName("staff_id")]
        public string? StaffId { get; set; }

        public string Username { get; set; } = null!;

        [JsonPropertyName("status")]
        public SystemCodeNestedDto? Status { get; set; }
        [JsonPropertyName("role")]
        public SystemCodeNestedDto? Role { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; set; }

    }
}