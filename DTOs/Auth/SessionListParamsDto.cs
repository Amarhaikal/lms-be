using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace QUANTM.DTOs.Auth
{
    public class SessionListParamsDto
    {

        [FromQuery(Name = "username")]
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [FromQuery(Name = "from_date")]
        [JsonPropertyName("from_date")]
        public DateTime? FromDate { get; set; }

        [FromQuery(Name = "to_date")]
        [JsonPropertyName("to_date")]
        public DateTime? ToDate { get; set; }

        [FromQuery(Name = "last_activity_date")]
        [JsonPropertyName("last_activity_date")]
        public DateTime? LastActivityDate { get; set; }

        [FromQuery(Name = "is_active")]
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "sort_by")]
        [JsonPropertyName("sort_by")]
        public string? SortBy { get; set; }

        [FromQuery(Name = "sort_order")]
        [JsonPropertyName("sort_order")]
        public string? SortOrder { get; set; }

        [FromQuery(Name = "page_no")]
        [JsonPropertyName("page_no")]
        public int PageNo { get; set; } = 1;

        [FromQuery(Name = "page_size")]
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;
    }
}