using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUANTM.Models.Common;
using UserEntity = QUANTM.Models.User.User;
namespace QUANTM.Models.Parameter;

[Table("system_codes")]
public class SystemCode
{
    [Column("id")]
    public int Id { get; set; }

    [Column("code_type_id")]
    [Required(ErrorMessage = "Code type is required")]
    public int CodeTypeId { get; set; }

    [Column("code")]
    [Required(ErrorMessage = "Code is required")]
    [MaxLength(50, ErrorMessage = "Code cannot exceed 50 characters")]
    public string Code { get; set; } = null!;

    [Column("description")]
    [Required(ErrorMessage = "Description is required")]
    [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
    public string Description { get; set; } = null!;

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    public UserEntity? CreatedByUser { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [ForeignKey("UpdatedBy")]
    public UserEntity? UpdatedByUser { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    public CodeType? CodeType { get; set; }

    public ICollection<MenuRole> MenuRoles { get; set; } = new List<MenuRole>();
}