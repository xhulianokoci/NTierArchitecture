using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models;

public class ApplicationUser : IdentityUser<int>
{
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(50)]
    public string LastName { get; set; }
    [MaxLength(128)]
    public string? Address { get; set; }
    [MaxLength(50)]
    public string? City { get; set; }
    [MaxLength(50)]
    public string? Province { get; set; }
    [MaxLength(50)]
    public string? State { get; set; }
    [MaxLength(10)]
    public string? ZipCode { get; set; }
    [MaxLength(1)]
    public string? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Occupation { get; set; }
    public string? Image { get; set; }
    public bool UserIsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? TokenHash { get; set; }
    public DateTime? DateCreated { get; set; }
    public bool? FirstTimeLogin { get; set; }
}
