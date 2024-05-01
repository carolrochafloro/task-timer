using task_timer.Context;

namespace task_timer.Repositories;

public class TasksRepository : Repository<Task>, ITasksRepository
{
    public TasksRepository(TTDbContext context) : base(context)
    {
    }
}
