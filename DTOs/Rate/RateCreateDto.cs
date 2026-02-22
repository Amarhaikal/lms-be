using System.ComponentModel.DataAnnotations;

namespace QUANTM.DTOs.Rate;

public class RateCreateDto
{
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(50)]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [MaxLength(255)]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Rate type code is required (Fixed, Floating, etc.)")]
    public string RateTypeCode { get; set; } = null!;
}
