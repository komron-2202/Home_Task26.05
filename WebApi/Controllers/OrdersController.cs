using Domain.DTOs;
using Domain.Dtos.Order;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    public async Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto request)
    {
        return await orderService.CreateAsync(request);
    }
    public async Task<Response<string>> DeleteAsync(int Id)
    {
        return await orderService.DeleteAsync(Id);
    }


    public async Task<Response<List<GetOrderDto>>> GetAllAsync([FromQuery] OrderFilter filter)
    {
        return await orderService.GetAllAsync(filter);
    }
    public async Task<Response<GetOrderDto>> GetByIdAsync(int Id)
    {
        return await orderService.GetByIdAsync(Id);

    }

    public async Task<Response<GetOrderDto>> UpdateAsync(int Id, UpdateOrderDto request)
    {
        return await orderService.UpdateAsync(Id, request);
    }
}