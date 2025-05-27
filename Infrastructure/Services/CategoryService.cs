using System.Net;
using Domain.Dtos.Category;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class CategoryService(
    IBaseRepository<Category, int> categoryRepository,
    IMemoryCacheService memoryCacheService) : ICategoryService
{
    public async Task<PagedResponse<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        const string cacheKey = "categories";

        var categoriesInCache = await memoryCacheService.GetData<List<GetCategoryDto>>(cacheKey);

        if (categoriesInCache == null)
        {
            var categories = await categoryRepository.GetAll();
            categoriesInCache = categories.Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            await memoryCacheService.SetData(cacheKey, categoriesInCache, 1);
        }

        if (!string.IsNullOrEmpty(filter.Name))
        {
            categoriesInCache = categoriesInCache.Where(c => c.Name.ToLower().Trim().Contains(filter.Name.ToLower().Trim())).ToList();
        }

        var totalRecords = categoriesInCache.Count;

        var paginatedData = categoriesInCache
            .Skip((filter.PagesNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        return new PagedResponse<List<GetCategoryDto>>(paginatedData, totalRecords, filter.PagesNumber,
            filter.PageSize);
    }

    public async Task<Response<string>> CreateAsync(CreateCategoryDto request)
    {
        var category = new Category()
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await categoryRepository.AddAsync(category);

        if (result != 1) return new Response<string>(HttpStatusCode.InternalServerError, "Failed");

        await memoryCacheService.DeleteData("categories"); 
        // await redisCacheService.RemoveData("categories");
        return new Response<string>("Success");
    }

    public async Task<Response<string>> UpdateAsync(int id, UpdateCategoryDto request)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        category.Name = request.Name;
        category.Description = request.Description;

        var result = await categoryRepository.UpdateAsync(category);

        if (result != 1) return new Response<string>(HttpStatusCode.BadRequest, "Failed");

        await memoryCacheService.DeleteData("categories"); 
        return new Response<string>("Success");
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        var result = await categoryRepository.DeleteAsync(category);

        if (result != 1) return new Response<string>(HttpStatusCode.BadRequest, "Failed");

        await memoryCacheService.DeleteData("categories");
        return new Response<string>("Success");
    }

    Task<Response<List<GetCategoryDto>>> ICategoryService.GetAllAsync(CategoryFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task<Response<GetCategoryDto>> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    Task<Response<GetCategoryDto>> ICategoryService.UpdateAsync(int id, UpdateCategoryDto request)
    {
        throw new NotImplementedException();
    }

    Task<Response<GetCategoryDto>> ICategoryService.CreateAsync(CreateCategoryDto request)
    {
        throw new NotImplementedException();
    }
}