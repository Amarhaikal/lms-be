using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Common
{
    public class AddressUpdateDto
    {
        [JsonPropertyName("address_line_1")]
        public string? AddressLine1 { get; set; }

        [JsonPropertyName("address_line_2")]
        public string? AddressLine2 { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("postcode")]
        public string? Postcode { get; set; }

        [JsonPropertyName("state")]
        public CodeReferenceDto? State { get; set; }

        [JsonPropertyName("country")]
        public CodeReferenceDto? Country { get; set; }
    }
}
