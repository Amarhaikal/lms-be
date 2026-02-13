using System.Text.Json.Serialization;

namespace QUANTM.DTOs.Parameter
{
    public class SystemCodeCreateDto
    {
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        [JsonPropertyName("code_type_code")]
        public string CodeTypeCode { get; set; } = null!;

    }
}