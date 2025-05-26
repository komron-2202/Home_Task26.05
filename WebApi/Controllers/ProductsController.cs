using Domain.DTOs;
using Domain.Dtos.Product;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService orderService) : ControllerBase
{
    public async Task<Response<GetProductDto>> CreateAsync(CreateProductDto request)
    {
        return await orderService.CreateAsync(request);
    }
    public async Task<Response<string>> DeleteAsync(int Id)
    {
        return await orderService.DeleteAsync(Id);
    }


    public async Task<Response<List<GetProductDto>>> GetAllAsync([FromQuery] ProductFilter filter)
    {
        return await orderService.GetAllAsync(filter);
    }
    public async Task<Response<GetProductDto>> GetByIdAsync(int Id)
    {
        return await orderService.GetByIdAsync(Id);

    }

    public async Task<Response<GetProductDto>> UpdateAsync(int Id, UpdateProductDto request)
    {
        return await orderService.UpdateAsync(Id, request);
    }
}