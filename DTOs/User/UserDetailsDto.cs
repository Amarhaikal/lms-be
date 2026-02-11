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

        [JsonPropertyName("id_no")]
        public string IdNo { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        [JsonPropertyName("status")]
        public SystemCodeNestedDto? Status { get; set; }
        [JsonPropertyName("role")]
        public SystemCodeNestedDto? Role { get; set; }
        [JsonPropertyName("gender")]
        public SystemCodeNestedDto? Gender { get; set; }
        [JsonPropertyName("address")]
        public AddressDto? Address { get; set; }


        public string? ProfileImageUrl { get; set; }
    }
}