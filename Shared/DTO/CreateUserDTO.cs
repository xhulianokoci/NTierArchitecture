namespace Shared.DTO;

public class CreateUserDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Occupation { get; set; }
    public string? Image { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}
