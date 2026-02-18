using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace QUANTM.DTOs.Parameter
{
    public class SystemCodeCreateDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
        [JsonPropertyName("code_type")]
        public string CodeTypeCode { get; set; } = null!;

    }
}