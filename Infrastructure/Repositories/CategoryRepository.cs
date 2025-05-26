using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Repositories;

public class CategoryRepository(DataContext context) : IBaseRepository<Category , int>
{
    public Task<IQueryable<Category>> GetAll()
    {
        var cars = context.Categories.AsQueryable();
        return Task.FromResult(cars);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var student = await context.Categories.FindAsync(id);
        return student;
    }

    public async Task<int> AddAsync(Category entity)
    {
        await context.Categories.AddAsync(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(Category entity)
    { 
        context.Categories.Update(entity);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(Category entity)
    {
        context.Categories.Remove(entity);
        return await context.SaveChangesAsync();
    }
}