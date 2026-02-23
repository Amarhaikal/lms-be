using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QUANTM.DTOs.Common;

namespace QUANTM.DTOs.Rate;

public class RateUpdateDto
{
    [Required]
    public int Id { get; set; }
    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    [JsonPropertyName("rate_type")]
    public CodeReferenceDto? RateType { get; set; }
}
