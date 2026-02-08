using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUANTM.Models.Parameter;

namespace QUANTM.Models.Common;

[Table("menu_roles")]
public class MenuRole
{
    [Column("id")]
    public int Id { get; set; }

    [Column("menu_id")]
    public int MenuId { get; set; }

    [ForeignKey("MenuId")]
    public Menu Menu { get; set; } = null!;

    [Column("role_id")]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public SystemCode Role { get; set; } = null!;

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}