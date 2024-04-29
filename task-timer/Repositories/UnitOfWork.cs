using task_timer.Context;

namespace task_timer.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IUsersRepository _usersRepository;

    private ITasksRepository _tasksRepository;

    private ICategoriesRepository _categoriesRepository;

    public TTDbContext _context;

    public UnitOfWork(TTDbContext context)
    {
        _context = context;
    }

    public IUsersRepository UsersRepository
    {
        get

        {
            return _usersRepository = _usersRepository ?? new UsersRepository(_context);
        }
    }

    public ITasksRepository TasksRepository
    {
        get
        {
            // lazy loading: avoids unecessary instances checking if there's an instance
            // of the object before creating another one

            return _tasksRepository = _tasksRepository ?? new TasksRepository(_context);
        }
    }

    public ICategoriesRepository CategoriesRepository
    {
        get
        {
            // lazy loading: avoids unecessary instances checking if there's an instance
            // of the object before creating another one

            return _categoriesRepository = _categoriesRepository ?? new CategoriesRepository(_context);
        }
    }
    public void CommitAsync()
    {
        _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        // disposes unmanaged resources such as db connection
        _context.Dispose();
    }
}
