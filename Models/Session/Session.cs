using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUANTM.Models.User;

namespace QUANTM.Models.Session
{
    [Table("sessions")]
    public class Session
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User.User? User { get; set; }

        [Column("token_jti")]
        [Required]
        [MaxLength(100)]
        public string TokenJti { get; set; } = null!;

        [Column("session_duration")]
        [Required]
        public int SessionDuration { get; set; }

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }

        [Column("expires_at")]
        [Required]
        public DateTime ExpiresAt { get; set; }

        [Column("logged_out_at")]
        public DateTime? LoggedOutAt { get; set; }

        [Column("is_active")]
        [Required]
        public bool IsActive { get; set; }

        [Column("ip_address")]
        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [Column("device_type")]
        [MaxLength(50)]
        public string? DeviceType { get; set; }

        [Column("location")]
        [MaxLength(200)]
        public string? Location { get; set; }

        [Column("last_activity_at")]
        public DateTime? LastActivityAt { get; set; }

        [Column("logout_reason")]
        [MaxLength(50)]
        public string? LogoutReason { get; set; }
    }
}
