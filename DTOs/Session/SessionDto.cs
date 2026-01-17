using System.Text.Json.Serialization;

namespace LMS.DTOs.Session
{
    public class SessionDto
    {
        public int Id { get; set; }

        [JsonPropertyName("token_jti")]
        public string TokenJti { get; set; } = null!;

        [JsonPropertyName("session_duration")]
        public int SessionDuration { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt { get; set; }

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
    }
}
