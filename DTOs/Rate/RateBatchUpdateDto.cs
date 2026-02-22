using System.ComponentModel.DataAnnotations;

namespace QUANTM.DTOs.Rate;

public class RateBatchUpdateDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public decimal? Rate { get; set; }

    public string? RateTypeCode { get; set; }
}
