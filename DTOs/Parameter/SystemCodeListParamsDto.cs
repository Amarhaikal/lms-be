using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace QUANTM.DTOs.Parameter
{
    public class SystemCodeListParamsDto
    {
        [FromQuery(Name = "code_type")]
        [JsonPropertyName("code_type")]
        public string? CodeTypeCode { get; set; }

        [FromQuery(Name = "code")]
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [FromQuery(Name = "description")]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [FromQuery(Name = "page_no")]
        [JsonPropertyName("page_no")]
        public int PageNo { get; set; } = 1;

        [FromQuery(Name = "page_size")]
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

    }
}