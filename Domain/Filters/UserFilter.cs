namespace Domain.Filters;

public class UserFilter
{
    public string? Name { get; set; }
    
    public int PagesNumber { get; set; }
    public int PageSize { get; set; }
}