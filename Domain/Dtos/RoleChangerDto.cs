using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User;

public class RoleChangerDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Role { get; set; }
}