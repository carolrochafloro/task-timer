using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class TasksRepository : Repository<AppTask>, ITasksRepository
{
    public TasksRepository(TTDbContext context) : base(context)
    {
    }

}
