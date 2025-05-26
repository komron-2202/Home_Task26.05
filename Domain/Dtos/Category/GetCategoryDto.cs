using Domain.Dtos.Order;

namespace Domain.Dtos.Category;

public class GetCategoryDto : CreateCategoryDto
{
    public int Id { get; set; }
}