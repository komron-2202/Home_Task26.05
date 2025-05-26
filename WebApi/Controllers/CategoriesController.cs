using Domain.DTOs;
using Domain.Dtos.Category;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ICategoryService orderService) : ControllerBase
{
    public async Task<Response<GetCategoryDto>> CreateAsync(CreateCategoryDto request)
    {
        return await orderService.CreateAsync(request);
    }
    public async Task<Response<string>> DeleteAsync(int Id)
    {
        return await orderService.DeleteAsync(Id);
    }


    public async Task<Response<List<GetCategoryDto>>> GetAllAsync([FromQuery] CategoryFilter filter)
    {
        return await orderService.GetAllAsync(filter);
    }
    public async Task<Response<GetCategoryDto>> GetByIdAsync(int Id)
    {
        return await orderService.GetByIdAsync(Id);

    }

    public async Task<Response<GetCategoryDto>> UpdateAsync(int Id, UpdateCategoryDto request)
    {
        return await orderService.UpdateAsync(Id, request);
    }
}