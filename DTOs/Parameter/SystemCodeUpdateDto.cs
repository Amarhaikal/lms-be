using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Parameter
{
    public class SystemCodeUpdateDto
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("code_type")]
        public string? CodeTypeCode { get; set; }
    }
}