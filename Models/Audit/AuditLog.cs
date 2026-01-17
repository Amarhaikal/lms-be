using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models.Audit
{
    [Table("audit_logs")]
    public class AuditLog
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Column("action")]
        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = null!;

        [Column("entity_type")]
        [MaxLength(50)]
        public string? EntityType { get; set; }

        [Column("entity_id")]
        public int? EntityId { get; set; }

        [Column("old_values")]
        public string? OldValues { get; set; }

        [Column("new_values")]
        public string? NewValues { get; set; }

        [Column("ip_address")]
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
