using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class AppTasksRepository : Repository<AppTask>, IAppTasksRepository
{
    public AppTasksRepository(TTDbContext context, UserManager<AppUser> userManager) : base(context, userManager)
    {
    }

    public IEnumerable<AppTask> GetByUserId(string id)
    {
        return _context.Set<AppTask>().
               AsNoTracking().Where(t => t.AspNetUsersId == id).ToList();
    }
}
