namespace Domain.Filters;

public class CategoryFilter
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int PagesNumber { get; set; }
    public int PageSize { get; set; }
}