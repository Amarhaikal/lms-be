using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANTM.Models.Common;

[Table("menus")]
public class Menu
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required(ErrorMessage = "Menu name is required")]
    [MaxLength(100, ErrorMessage = "Menu name cannot exceed 100 characters")]
    public string Name { get; set; } = null!;

    [Column("code")]
    [Required(ErrorMessage = "Menu code is required")]
    [MaxLength(50, ErrorMessage = "Menu code cannot exceed 50 characters")]
    public string Code { get; set; } = null!;

    [Column("url")]
    [MaxLength(255, ErrorMessage = "URL cannot exceed 255 characters")]
    public string? Url { get; set; }

    [Column("icon")]
    [MaxLength(50, ErrorMessage = "Icon cannot exceed 50 characters")]
    public string? Icon { get; set; }

    [Column("parent_id")]
    public int? ParentId { get; set; }
    [ForeignKey("ParentId")]
    public Menu? Parent { get; set; }

    [Column("sort_order")]
    public int SortOrder { get; set; } = 0;

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Menu> SubMenus { get; set; } = new List<Menu>();
    public ICollection<MenuRole> MenuRoles { get; set; } = new List<MenuRole>();
}