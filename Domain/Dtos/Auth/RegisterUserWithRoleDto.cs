namespace Domain.DTOs.Auth;

public class RegisterUserWithRoleDto
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}