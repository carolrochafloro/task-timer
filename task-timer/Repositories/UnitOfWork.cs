using Microsoft.AspNetCore.Identity;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class UnitOfWork : IUnitOfWork
{

    private IAppTasksRepository _tasksRepository;

    private ICategoriesRepository _categoriesRepository;

    public TTDbContext _context;

    public UserManager<AppUser> _userManager;

    public UnitOfWork(TTDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IAppTasksRepository TasksRepository
    {
        get
        {
            // lazy loading: avoids unecessary instances checking if there's an instance
            // of the object before creating another one

            return _tasksRepository = _tasksRepository ?? new AppTasksRepository(_context, _userManager);
        }
    }

    public ICategoriesRepository CategoriesRepository
    {
        get
        {
            // lazy loading: avoids unecessary instances checking if there's an instance
            // of the object before creating another one

            return _categoriesRepository = _categoriesRepository ?? new CategoriesRepository(_context, _userManager);
        }
    }
    public async Task CommitAsync()
    {
       await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        // disposes unmanaged resources such as db connection
        _context.Dispose();
    }
}
