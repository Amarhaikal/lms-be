using QUANTM.DTOs.Parameter;

namespace QUANTM.DTOs.Rate;

public class RateDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Rate { get; set; }

    // Read-only info from Navigation Properties
    public SystemCodeNestedDto? RateType { get; set; }

    // Audit Fields
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
