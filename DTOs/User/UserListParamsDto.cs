using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace QUANTM.DTOs.User
{
    public class UserListParamsDto
    {
        [FromQuery(Name = "fullname")]
        [JsonPropertyName("fullname")]
        public string? Fullname { get; set; }

        [FromQuery(Name = "username")]
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [FromQuery(Name = "role")]
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [FromQuery(Name = "page_no")]
        [JsonPropertyName("page_no")]
        public int PageNo { get; set; } = 1;

        [FromQuery(Name = "page_size")]
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;
    }
}