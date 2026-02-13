using System.Text.Json.Serialization;
using QUANTM.DTOs.Parameter;
using QUANTM.DTOs.Common;

namespace QUANTM.DTOs.User
{
    public class UserDetailsDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; } = null!;
        public string? Shortname { get; set; }
        public DateOnly? JoinedDt { get; set; }

        [JsonPropertyName("id_no")]
        public string IdNo { get; set; } = null!;
        [JsonPropertyName("staff_id")]
        public string? StaffId { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        [JsonPropertyName("phone_no")]
        public string? PhoneNo { get; set; }

        [JsonPropertyName("department")]
        public SystemCodeNestedDto? Department { get; set; }

        [JsonPropertyName("designation")]
        public string? Designation { get; set; }

        [JsonPropertyName("remarks")]
        public string? Remarks { get; set; }
        [JsonPropertyName("status")]
        public SystemCodeNestedDto? Status { get; set; }
        [JsonPropertyName("role")]
        public SystemCodeNestedDto? Role { get; set; }
        [JsonPropertyName("gender")]
        public SystemCodeNestedDto? Gender { get; set; }
        [JsonPropertyName("address")]
        public AddressDto? Address { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("created_by")]
        public string? CreatedBy { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonPropertyName("updated_by")]
        public string? UpdatedBy { get; set; }
    }
}