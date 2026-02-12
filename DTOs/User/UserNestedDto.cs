using System.Text.Json.Serialization;

namespace QUANTM.DTOs.User
{
    public class UserNestedDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("fullname")]
        public string Fullname { get; set; } = null!;
    }
}
