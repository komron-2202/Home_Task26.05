using Domain.Enums;

namespace Domain.Dtos.Order;

public class CreateOrderDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public Status Status { get; set; }
}