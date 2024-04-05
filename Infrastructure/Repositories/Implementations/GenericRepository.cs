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
    
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }
    
    public async Task<bool> Exists(string id, CancellationToken cancellationToken)
    {
        var entity = await Get(id, cancellationToken);
        return entity != null;
    }
    
    public async Task<T?> Get(string id, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbContext.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
    {
       _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        _dbContext.Set<T>().Remove(entity);
    }
    
    public IQueryable<T> FindAll()
    {
        return this._dbContext.Set<T>();
    }
}