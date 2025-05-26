namespace Domain.Filters;

public class ProductFilter
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }
    public bool? IsTop { get; set; }
    public bool? IsPremium { get; set; }
    public int PagesNumber { get; set; }
    public int PageSize { get; set; }
}