using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LMS.Models.Parameter;

namespace LMS.Models.Common;

[Table("addresses")]
public class Address
{
    [Column("id")]
    public int Id { get; set; }

    [Column("address_line_1")]
    [Required(ErrorMessage = "Address Line 1 is required")]
    [MaxLength(255, ErrorMessage = "Address Line 1 cannot exceed 255 characters")]
    public string? AddressLine1 { get; set; }

    [Column("address_line_2")]
    [MaxLength(255, ErrorMessage = "Address Line 2 cannot exceed 255 characters")]
    public string? AddressLine2 { get; set; }

    [Column("postcode")]
    [MaxLength(6, ErrorMessage = "Postcode cannot exceed 6 characters")]
    public string? Postcode { get; set; }

    [Column("city")]
    [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }

    [Column("state_id")]
    public int? StateId { get; set; }
    [ForeignKey("StateId")]
    public SystemCode? State { get; set; }

    [Column("country_id")]
    [Required(ErrorMessage = "Country is required")]
    public int CountryId { get; set; }
    [ForeignKey("CountryId")]
    public SystemCode? Country { get; set; }
}