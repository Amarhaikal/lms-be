using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.User;

[Table("screens")]
public class Screen
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [Required(ErrorMessage = "Screen name is required")]
    [MaxLength(100, ErrorMessage = "Screen name cannot exceed 100 characters")]
    public string Name { get; set; } = null!;

    [Column("code")]
    [Required(ErrorMessage = "Screen code is required")]
    [MaxLength(50, ErrorMessage = "Screen code cannot exceed 50 characters")]
    public string Code { get; set; } = null!;

    [Column("url")]
    [MaxLength(255, ErrorMessage = "URL cannot exceed 255 characters")]
    public string? Url { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}