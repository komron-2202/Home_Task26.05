using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Filters;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Order 
{
    [Key] public int Id { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public int ProductId { get; set; }
    [Required] public int Quantity { get; set; }
    [Required] public DateTimeOffset OrderDate { get; set; }
    [Required] public Status Status { get; set; }


    public virtual Product Product { get; set; }
    public virtual IdentityUser User { get; set; }
}