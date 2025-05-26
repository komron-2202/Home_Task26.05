using System.Net;
using AutoMapper;
using Domain.DTOs;
using Domain.Dtos.Order;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class OrderService(IBaseRepository<Order, int> repository, IMapper mapper) : IOrderService
{
    public async Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto input)
    {
        var order = mapper.Map<Order>(input);
        // var 
        // if ()
        // {
        //     
        // }
        
        var result = await repository.AddAsync(order);

        var data = mapper.Map<GetOrderDto>(order);

        return result == 0
            ? new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Order no added")
            : mapper.Map<Response<GetOrderDto>>(data);
    }

    public async Task<Response<string>> DeleteAsync(int id)
    {
        var order = await repository.GetByIdAsync(id);
        if (order == null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Not found");
        }

        await repository.DeleteAsync(order);

        return new Response<string>(HttpStatusCode.BadRequest, "Order deleted");
    }

    public async Task<Response<List<GetOrderDto>>> GetAllAsync(OrderFilter filter)
    {
        var validFilter = new ValidFilter(filter.PagesNumber, filter.PageSize);
        var order = await repository.GetAll();

        if (filter.From != null)
        {
            order = order.Where(o => o.Quantity >= filter.From);
        }

        if (filter.To != null)
        {
            order = order.Where(s => s.Quantity <= filter.To);
        }

        if (filter.Status != null)
        {
            order = order.Where(s => s.Status == filter.Status);
        }

        if (filter.ProductId != null)
        {
            order = order.Where(s => s.ProductId >= filter.ProductId);
        }

        if (filter.UserId != null)
        {
            order = order.Where(s => s.UserId >= filter.UserId);
        }

        if (filter.OrderDate != null)
        {
            order = order.Where(o => o.OrderDate == filter.OrderDate);
        }

        var mapped = mapper.Map<List<GetOrderDto>>(order);

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

        return result == 0
            ? new Response<GetOrderDto>(HttpStatusCode.BadRequest, "Not to update")
            : new Response<GetOrderDto>(data);
    }
}