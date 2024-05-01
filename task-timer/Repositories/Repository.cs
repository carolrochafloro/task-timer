using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using task_timer.Context;

namespace task_timer.Repositories;

// T must be a class

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly TTDbContext _context;

    public Repository(TTDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(predicate);
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }

    public async Task<T> DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return entity;
    }

}
