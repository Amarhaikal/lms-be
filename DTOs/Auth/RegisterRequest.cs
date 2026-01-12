
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(150)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(120)]
        public string Fullname { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(70)]
        public string IdNo { get; set; } = null!;

        [MaxLength(20)]
        public string? PhoneNo { get; set; }
    }
}

