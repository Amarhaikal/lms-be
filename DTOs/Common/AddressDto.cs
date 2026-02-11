using System.Text.Json.Serialization;
using QUANTM.DTOs.Parameter;

namespace QUANTM.DTOs.Common
{
    public class AddressDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("address_line_1")]
        public string AddressLine1 { get; set; } = null!;
        [JsonPropertyName("address_line_2")]
        public string AddressLine2 { get; set; } = null!;
        [JsonPropertyName("city")]
        public string City { get; set; } = null!;
        [JsonPropertyName("postcode")]
        public string Postcode { get; set; } = null!;
        [JsonPropertyName("state")]
        public SystemCodeNestedDto? State { get; set; }
        [JsonPropertyName("country")]
        public SystemCodeNestedDto? Country { get; set; }
    }
}
