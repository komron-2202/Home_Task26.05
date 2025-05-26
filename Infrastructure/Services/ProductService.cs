using System.Net;
using AutoMapper;
using Domain.DTOs;
using Domain.Dtos.Product;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class ProductService(IBaseRepository<Product, int> repository, IMapper mapper) : IProductService
{
    public async Task<Response<GetProductDto>> CreateAsync(CreateProductDto input)
    {
        var product = mapper.Map<Product>(input);

        var result = await repository.AddAsync(product);

        var data = mapper.Map<GetProductDto>(product);

        return result == 0
            ? new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product no added")
            : mapper.Map<Response<GetProductDto>>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);
        if (product == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Not found");
        }

        await repository.DeleteAsync(product);

        return new Response<string>(HttpStatusCode.BadRequest, "Product deleted");
    }

    public async Task<Response<List<GetProductDto>>> GetAllAsync(ProductFilter filter)
    {
        var validFilter = new ValidFilter(filter.PagesNumber, filter.PageSize);
        var product = await repository.GetAll();

        if (filter.Description != null)
        {
            product = product.Where(o => o.Description.Contains(filter.Description));
        }

        if (filter.Name != null)
        {
            product = product.Where(s => s.Name.Contains(filter.Name));
        }

        var mapped = mapper.Map<List<GetProductDto>>(product);

        var totalRecords = mapped.Count;

        var data = mapped.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize).ToList();

        return new PagedResponse<List<GetProductDto>>(data, validFilter.PageNumber, validFilter.PageSize,
            totalRecords);
    }

    public async Task<Response<GetProductDto>> GetByIdAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);

        if (product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Not found");
        }

        var data = mapper.Map<GetProductDto>(product);

        return new Response<GetProductDto>(data);
    }

    public async Task<Response<GetProductDto>> UpdateAsync(int id, UpdateProductDto input)
    {
        var product = await repository.GetByIdAsync(id);

        if (product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Not found");
        }

        product.Name = input.Name;
        product.Description = input.Description;
        product.IsPremium = input.IsPremium;
        product.Price = input.Price;
        product.CategoryId= input.CategoryId;
        product.UserId = input.UserId;
        product.IsTop = input.IsTop;

        var result = await repository.UpdateAsync(product);
        var data = mapper.Map<GetProductDto>(product);

        return result == 0
            ? new Response<GetProductDto>(HttpStatusCode.BadRequest, "Not to update")
            : new Response<GetProductDto>(data);
    }
}