using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class OrderRepository(DataContext context) : IBaseRepository<Order , int>
{
    public Task<IQueryable<Order>> GetAll()
    {
        var cars = context.Orders.AsQueryable();
        return Task.FromResult(cars);
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        var student = await context.Orders.FindAsync(id);
        return student;
    }

    public async Task<int> AddAsync(Order entity)
    {
        await context.Orders.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Order entity)
    { 
        context.Orders.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Order entity)
    {
        context.Orders.Remove(entity);
        return await context.SaveChangesAsync();
    }
}