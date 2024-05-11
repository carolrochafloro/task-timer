using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

// T must be a class

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly TTDbContext _context;
    protected readonly UserManager<AppUser> _userManager;

    public Repository(TTDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IEnumerable<T> GetAllAsync()
    {
        return _context.Set<T>().AsNoTracking().ToList();
    }

    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().FirstOrDefault(predicate);
    }

    public T CreateAsync(T entity)
    {
        _context.Set<T>().Add(entity);
        return entity;
    }

    public T UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        return entity;
    }

    public T DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        return entity;
    }

}
