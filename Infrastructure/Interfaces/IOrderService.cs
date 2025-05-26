using Domain.DTOs;
using Domain.Dtos.Order;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task<Response<List<GetOrderDto>>> GetAllAsync(OrderFilter filter);
    Task<Response<GetOrderDto>> GetByIdAsync(int id);
    Task<Response<GetOrderDto>> UpdateAsync(int id, UpdateOrderDto request);
    Task<Response<string>> DeleteAsync(int id);
    Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto request);
}