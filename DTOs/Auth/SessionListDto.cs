using System.Text.Json.Serialization;
using QUANTM.DTOs.Parameter;

namespace QUANTM.DTOs.Auth
{
    public class SessionListDto
    {
        public int Id { get; set; }

        [JsonPropertyName("session_duration")]
        public int SessionDuration { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("logged_out_at")]
        public DateTime? LoggedOutAt { get; set; }

        [JsonPropertyName("is_active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }

        [JsonPropertyName("device_type")]
        public string? DeviceType { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("last_activity_at")]
        public DateTime? LastActivityAt { get; set; }

        [JsonPropertyName("logout_reason")]
        public string? LogoutReason { get; set; }

        [JsonPropertyName("user")]
        public SessionUserDto? User { get; set; }
    }

    public class SessionUserDto
    {
        public int Id { get; set; }

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; } = null!;

        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("role")]
        public SystemCodeNestedDto? Role { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; set; }
    }
}