using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ArtemisBankingPro.Core.Domain.Interfaces;
using ArtemisBankingPro.Infrastructure.Persistence.Context;

namespace ArtemisBankingPro.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ArtemisBankingDbContext _dbContext;

    public GenericRepository(ArtemisBankingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllListWithIncludeAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbContext.Set<T>();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.ToListAsync();
    }

    public virtual IQueryable<T> GetAllQuery()
    {
        return _dbContext.Set<T>().AsQueryable();
    }

    public virtual IQueryable<T> GetAllQueryWithInclude(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbContext.Set<T>();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query;
    }
}
