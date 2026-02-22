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
    public int? CreatedBy { get; set; }
    public string? CreatorName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public string? UpdaterName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
