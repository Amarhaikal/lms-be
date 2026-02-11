using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Common
{
    public class CodeReferenceDto
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
