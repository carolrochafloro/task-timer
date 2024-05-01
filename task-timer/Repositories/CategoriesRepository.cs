using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class CategoriesRepository : Repository<Category>, ICategoriesRepository
{
    public CategoriesRepository(TTDbContext context) : base(context)
    {
    }
}
