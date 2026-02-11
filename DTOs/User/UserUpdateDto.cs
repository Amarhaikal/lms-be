using System.Text.Json.Serialization;
using QUANTM.DTOs.Common;

namespace QUANTM.DTOs.User
{
    public class UserUpdateDto
    {
        [JsonPropertyName("fullname")]
        public string? Fullname { get; set; }

        [JsonPropertyName("shortname")]
        public string? Shortname { get; set; }

        [JsonPropertyName("id_no")]
        public string? IdNo { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone_no")]
        public string? PhoneNo { get; set; }

        [JsonPropertyName("gender")]
        public CodeReferenceDto? Gender { get; set; }

        [JsonPropertyName("address")]
        public AddressUpdateDto? Address { get; set; }

        [JsonPropertyName("profile_image_id")]
        public int? ProfileImageId { get; set; }

        [JsonPropertyName("status")]
        public CodeReferenceDto? Status { get; set; }

        [JsonPropertyName("role")]
        public CodeReferenceDto? Role { get; set; }
    }
}