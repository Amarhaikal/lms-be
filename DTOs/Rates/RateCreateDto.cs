using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QUANTM.DTOs.Common;

namespace QUANTM.DTOs.Rates;

public class RateCreateDto
{
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(255)]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Rate is required")]
    public decimal Rate { get; set; }

    [Required(ErrorMessage = "Rate type is required")]
    [JsonPropertyName("rate_type")]
    public CodeReferenceDto RateType { get; set; } = null!;
}
