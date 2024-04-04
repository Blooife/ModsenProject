using Domain.Repositories.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{

    private readonly AppDbContext _dbContext;
    
    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
    }
    
    public async Task<bool> Exists(string id)
    {
        var entity = await Get(id);
        return entity != null;
    }
    
    public async Task<T?> Get(string id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _dbContext.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
       _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }
}