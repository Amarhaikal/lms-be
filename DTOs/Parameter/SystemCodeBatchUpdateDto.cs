using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Parameter
{
    public class SystemCodeBatchUpdateDto
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("code_type")]
        public string? CodeTypeCode { get; set; }
    }
}
