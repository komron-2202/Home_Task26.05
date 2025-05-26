using System.Net;
using AutoMapper;
using Domain.DTOs;
using Domain.Dtos.Category;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class CategoryService(IBaseRepository<Category, int> repository, IMapper mapper) : ICategoryService
{
    public async Task<Response<GetCategoryDto>> CreateAsync(CreateCategoryDto input)
    {
        var customer = mapper.Map<Category>(input);

        var result = await repository.AddAsync(customer);

        var data = mapper.Map<GetCategoryDto>(customer);

        return result == 0
            ? new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Category no added")
            : mapper.Map<Response<GetCategoryDto>>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var customer = await repository.GetByIdAsync(id);
        if (customer == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Not found");
        }

        await repository.DeleteAsync(customer);

        return new Response<string>(HttpStatusCode.BadRequest, "Category deleted");
    }

    public async Task<Response<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        var validFilter = new ValidFilter(filter.PagesNumber, filter.PageSize);
        var customer = await repository.GetAll();

        if (filter.Description != null)
        {
            customer = customer.Where(o => o.Description.Contains(filter.Description));
        }

        if (filter.Name != null)
        {
            customer = customer.Where(s => s.Name.Contains(filter.Name));
        }

        var mapped = mapper.Map<List<GetCategoryDto>>(customer);

        var totalRecords = mapped.Count;

        var data = mapped.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize).ToList();

        return new PagedResponse<List<GetCategoryDto>>(data, validFilter.PageNumber, validFilter.PageSize,
            totalRecords);
    }

    public async Task<Response<GetCategoryDto>> GetByIdAsync(int id)
    {
        var customer = await repository.GetByIdAsync(id);

        if (customer == null)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Not found");
        }

        var data = mapper.Map<GetCategoryDto>(customer);

        return new Response<GetCategoryDto>(data);
    }

    public async Task<Response<GetCategoryDto>> UpdateAsync(int id, UpdateCategoryDto input)
    {
        var customer = await repository.GetByIdAsync(id);

        if (customer == null)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Not found");
        }

        customer.Description = input.Description;
        customer.Name = input.Name;


        var result = await repository.UpdateAsync(customer);
        var data = mapper.Map<GetCategoryDto>(customer);

        return result == 0
            ? new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "Not to update")
            : new Response<GetCategoryDto>(data);
    }
}