using System.Net;
using AutoMapper;
using Domain.DTOs;
using Domain.Dtos.Order;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class OrderService(IBaseRepository<Order, int> repository,IMemoryCacheService memoryCacheService, IMapper mapper, IRedisCacheService redisCacheService) : IOrderService
{
    public async Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto input)
    {
        var order = mapper.Map<Order>(input);

        var result = await repository.AddAsync(order);

        if (result == 0)
        {
            return new Response<GetOrderDto>(HttpStatusCode.InternalServerError, "Failed");
        }
        // await memoryCacheService.DeleteData("orders"); 
        await redisCacheService.RemoveData("orders");

        var data = mapper.Map<GetOrderDto>(order);
        return new Response<GetOrderDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var order = await repository.GetByIdAsync(id);
        if (order == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Not found");
        }

        await repository.DeleteAsync(order);
        // await memoryCacheService.DeleteData("orders");
        await redisCacheService.RemoveData("orders");

        return new Response<string>(HttpStatusCode.BadRequest, "Order deleted");
    }

    public async Task<Response<List<GetOrderDto>>> GetAllAsync(OrderFilter filter)
    {
        const string cacheKey = "categories";
        var validFilter = new ValidFilter(filter.PagesNumber, filter.PageSize);
        // var ordersInCache = await memoryCacheService.GetData<List<GetOrderDto>>(cacheKey);
        var ordersInCache = await redisCacheService.GetData<List<GetOrderDto>>(cacheKey);

        if (ordersInCache == null)
        {
            var order = await repository.GetAll();
            ordersInCache = order.Select(c => new GetOrderDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                OrderDate = c.OrderDate,
                Status = c.Status
            }).ToList();

            await memoryCacheService.SetData(cacheKey, ordersInCache, 1);
        }

        if (filter.From != null)
        {
            ordersInCache = (List<GetOrderDto>)ordersInCache.Where(o => o.Quantity >= filter.From);
        }

        if (filter.To != null)
        {
            ordersInCache = (List<GetOrderDto>)ordersInCache.Where(s => s.Quantity <= filter.To);
        }

        if (filter.Status != null)
        {
            ordersInCache = (List<GetOrderDto>)ordersInCache.Where(s => s.Status == filter.Status);
        }

        if (filter.ProductId != null)
        {
            ordersInCache = (List<GetOrderDto>)ordersInCache.Where(s => s.ProductId >= filter.ProductId);
        }

        if (filter.UserId != null)
        {
            ordersInCache = (List<GetOrderDto>)ordersInCache.Where(s => s.UserId >= filter.UserId);
        }

        if (filter.OrderDate != null)
        {
            ordersInCache = (List<GetOrderDto>)ordersInCache.Where(o => o.OrderDate == filter.OrderDate);
        }

        var mapped = mapper.Map<List<GetOrderDto>>(ordersInCache);

        var totalRecords = mapped.Count;

        var data = mapped.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize).ToList();

        return new PagedResponse<List<GetOrderDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<GetOrderDto>> GetByIdAsync(int id)
    {
        var order = await repository.GetByIdAsync(id);

        if (order == null)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Not found");
        }

        var data = mapper.Map<GetOrderDto>(order);

        return new Response<GetOrderDto>(data);
    }

    public async Task<Response<GetOrderDto>> UpdateAsync(int id, UpdateOrderDto input)
    {
        var order = await repository.GetByIdAsync(id);

        if (order == null)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Not found");
        }


        order.Quantity = input.Quantity;
        order.OrderDate = input.OrderDate;
        order.Status = input.Status;
        order.ProductId = input.ProductId;
        order.UserId = input.UserId;

        var result = await repository.UpdateAsync(order);
        var data = mapper.Map<GetOrderDto>(order);

        if (result == 0)
        {
            return new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Not to update");
        }

        // await memoryCacheService.DeleteData("orders");
        await redisCacheService.RemoveData("orders");

        return new Response<GetOrderDto>(data);
    }
}