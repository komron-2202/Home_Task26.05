using Domain.DTOs;
using Domain.Dtos.Category;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<Response<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter);
    Task<Response<GetCategoryDto>> GetByIdAsync(int id);
    Task<Response<GetCategoryDto>> UpdateAsync(int id, UpdateCategoryDto request);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<GetCategoryDto>> CreateAsync(CreateCategoryDto request);
}