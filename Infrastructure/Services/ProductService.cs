using System.Net;
using AutoMapper;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Domain.Dtos.Product;

namespace Infrastructure.Services;

public class ProductService(IBaseRepository<Product, int> repository,IMemoryCacheService memoryCacheService, IMapper mapper) : IProductService
{
    public async Task<Response<GetProductDto>> CreateAsync(CreateProductDto input)
    {
        var Product = mapper.Map<Product>(input);

        var result = await repository.AddAsync(Product);

        if (result == 0)
        {
            return new Response<GetProductDto>(HttpStatusCode.InternalServerError, "Failed");
        }
        await memoryCacheService.DeleteData("Products"); 
        var data = mapper.Map<GetProductDto>(Product);
        return new Response<GetProductDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var Product = await repository.GetByIdAsync(id);
        if (Product == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Not found");
        }

        await repository.DeleteAsync(Product);
        await memoryCacheService.DeleteData("Products");

        return new Response<string>(HttpStatusCode.BadRequest, "Product deleted");
    }

    public async Task<Response<List<GetProductDto>>> GetAllAsync(ProductFilter filter)
    {
        const string cacheKey = "categories";
        var validFilter = new ValidFilter(filter.PagesNumber, filter.PageSize);
        var ProductsInCache = await memoryCacheService.GetData<List<GetProductDto>>(cacheKey);

        if (ProductsInCache == null)
        {
            var Product = await repository.GetAll();
            ProductsInCache = Product.Select(c => new GetProductDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                CategoryId = c.CategoryId,
                UserId = c.UserId,
                IsTop = c.IsTop,
                IsPremium = c.IsPremium,
                PremiumOrTopExpiryDate = c.PremiumOrTopExpiryDate
            }).ToList();

            await memoryCacheService.SetData(cacheKey, ProductsInCache, 1);
        }

        if (filter.UserId != null)
        {
            ProductsInCache = (List<GetProductDto>)ProductsInCache.Where(s => s.UserId >= filter.UserId);
        }

        var mapped = mapper.Map<List<GetProductDto>>(ProductsInCache);

        var totalRecords = mapped.Count;

        var data = mapped.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize).ToList();

        return new PagedResponse<List<GetProductDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<GetProductDto>> GetByIdAsync(int id)
    {
        var Product = await repository.GetByIdAsync(id);

        if (Product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Not found");
        }

        var data = mapper.Map<GetProductDto>(Product);

        return new Response<GetProductDto>(data);
    }

    public async Task<Response<GetProductDto>> UpdateAsync(int id, UpdateProductDto input)
    {
        var Product = await repository.GetByIdAsync(id);

        if (Product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Not found");
        }


        Product.Name = input.Name;
        Product.Description = input.Description;
        Product.Price = input.Price;
        Product.CategoryId = input.CategoryId;
        Product.UserId = input.UserId;
        Product.IsTop = input.IsTop;
        Product.IsPremium = input.IsPremium;
        Product.PremiumOrTopExpiryDate = input.PremiumOrTopExpiryDate;

        var result = await repository.UpdateAsync(Product);
        var data = mapper.Map<GetProductDto>(Product);

        if (result == 0)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Not to update");
        }

        await memoryCacheService.DeleteData("Products");

        return new Response<GetProductDto>(data);
    }
}