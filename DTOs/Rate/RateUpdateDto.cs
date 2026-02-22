using System.ComponentModel.DataAnnotations;

namespace QUANTM.DTOs.Rate;

public class RateUpdateDto
{
    [MaxLength(50)]
    public string? Code { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public string? RateTypeCode { get; set; }
}
