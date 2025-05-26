using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace Domain.DTOs.User;

public class CreateUserDto
{
    [Required] [MaxLength(30)] public string UserName { get; set; }
    [MaxLength(30)] public string? Phone { get; set; }
    [EmailAddress]
    public string Email { get; set; }
    [Required] public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}