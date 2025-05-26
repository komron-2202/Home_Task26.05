using Domain.DTOs;
using Domain.Dtos.Product;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<Response<List<GetProductDto>>> GetAllAsync(ProductFilter filter);
    Task<Response<GetProductDto>> GetByIdAsync(int id);
    Task<Response<GetProductDto>> UpdateAsync(int id, UpdateProductDto request);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<GetProductDto>> CreateAsync(CreateProductDto request);
}