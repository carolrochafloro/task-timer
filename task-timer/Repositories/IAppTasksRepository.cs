using Microsoft.AspNetCore.Mvc;
using task_timer.Models;

namespace task_timer.Repositories;

public interface IAppTasksRepository : IRepository<AppTask>
{
    public IEnumerable<AppTask> GetByUserId(string id);

    public IEnumerable<AppTask> GetByCategoryId(int id);

}
