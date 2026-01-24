using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUANTM.Models;

namespace QUANTM.Models.Auth
{
    [Table("password_history")]
    public class PasswordHistory
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User.User? User { get; set; }

        [Column("password_hash")]
        [Required]
        public string PasswordHash { get; set; } = null!;

        [Column("created_at")]
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}