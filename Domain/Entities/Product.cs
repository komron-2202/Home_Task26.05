using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required] public string Description { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public bool IsTop { get; set; }
    [Required] public bool IsPremium { get; set; }
    [Required] public DateTimeOffset PremiumOrTopExpiryDate{ get; set; }

    public virtual Category Category { get; set; }
    public virtual IdentityUser User { get; set; }
    public virtual List<Order> Orders { get; set; }
}