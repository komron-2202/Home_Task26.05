using Domain.Enums;

namespace Domain.Filters;

public class OrderFilter
{
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public int? From { get; set; }
    public int? To { get; set; }
    public DateTime? OrderDate { get; set; }
    public Status? Status { get; set; }
    public int PagesNumber { get; set; }
    public int PageSize { get; set; }
}