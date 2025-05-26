using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class ProductRepository(DataContext context) : IBaseRepository<Product , int>
{
    public Task<IQueryable<Product>> GetAll()
    {
        var cars = context.Products.AsQueryable();
        return Task.FromResult(cars);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var student = await context.Products.FindAsync(id);
        return student;
    }

    public async Task<int> AddAsync(Product entity)
    {
        await context.Products.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Product entity)
    { 
        context.Products.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Product entity)
    {
        context.Products.Remove(entity);
        return await context.SaveChangesAsync();
    }
}