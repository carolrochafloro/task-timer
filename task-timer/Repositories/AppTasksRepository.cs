using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class AppTasksRepository : Repository<AppTask>, IAppTasksRepository
{
    public AppTasksRepository(TTDbContext context) : base(context)
    {
    }

}
