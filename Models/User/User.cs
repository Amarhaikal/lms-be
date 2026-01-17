using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Models.Common;
using LMS.Models.Parameter;

namespace LMS.Models.User;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }

    [Column("fullname")]
    [Required(ErrorMessage = "Fullname is required")]
    [MaxLength(120, ErrorMessage = "Fullname cannot exceed 120 characters")]
    public string Fullname { get; set; } = null!;

    [Column("id_no")]
    [Required(ErrorMessage = "ID No. is required")]
    [MaxLength(12, ErrorMessage = "ID No. cannot exceed 12 characters")]
    public string IdNo { get; set; } = null!;

    [Column("username")]
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(30, ErrorMessage = "Username cannot exceed 30 characters")]
    public string Username { get; set; } = null!;

    [Column("email")]
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
    public string Email { get; set; } = null!;

    [Column("password")]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;

    [Column("phone_no")]
    [MaxLength(12, ErrorMessage = "Phone number cannot exceed 12 characters")]
    public string? PhoneNo { get; set; }

    [Column("address_id")]
    public int? AddressId { get; set; }
    [ForeignKey("AddressId")]
    public Address? Address { get; set; }

    [Column("status_id")]
    [Required(ErrorMessage = "Status is required")]
    public int StatusId { get; set; }
    [ForeignKey("StatusId")]
    public SystemCode? Status { get; set; }

    [Column("role_id")]
    [Required(ErrorMessage = "Role is required")]
    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public SystemCode? Role { get; set; }

    [Column("created_by")]
    public int? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_by")]
    public int? UpdatedBy { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("failed_login_attempts")]
    public int FailedLoginAttempts { get; set; }

    [Column("locked_until")]
    public DateTime? LockedUntil { get; set; }

    // Navigation properties
    public ICollection<Session.Session>? Sessions { get; set; }
}