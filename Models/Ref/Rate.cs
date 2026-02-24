using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUANTM.Models.Parameter;
using UserEntity = QUANTM.Models.User.User;

namespace QUANTM.Models.Ref;

[Table("rates")]
public class Rate
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("code")]
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
    public string Code { get; set; } = null!;

    [Column("description")]
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string Description { get; set; } = null!;

    [Column("rate_type_id")]
    [Required(ErrorMessage = "Rate Type is required")]
    public int RateTypeId { get; set; }

    [Column("rate", TypeName = "decimal(18, 4)")]
    [Required(ErrorMessage = "Rate is required")]
    public decimal RateValue { get; set; }

    [ForeignKey("RateTypeId")]
    public SystemCode? RateType { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public UserEntity? Creator { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [ForeignKey("UpdatedBy")]
    public UserEntity? Updater { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
