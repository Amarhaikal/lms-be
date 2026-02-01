using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANTM.Models.Common;

[Table("documents")]
public class Document
{
    [Column("id")]
    [Key]
    public int Id { get; set; }

    [Column("file_name")]
    [Required]
    [MaxLength(255)]
    public string FileName { get; set; } = null!;

    [Column("file_path")]
    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = null!;

    [Column("content_type")]
    [MaxLength(100)]
    public string? ContentType { get; set; }

    [Column("file_extension")]
    [MaxLength(20)]
    public string? FileExtension { get; set; }

    [Column("file_size")]
    public long FileSize { get; set; }

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
