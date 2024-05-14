using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class AppTasksRepository : Repository<AppTask>, IAppTasksRepository
{
    public AppTasksRepository(TTDbContext context, UserManager<AppUser> userManager) : base(context, userManager)
    {
    }

    public IEnumerable<AppTask> GetByCategoryId(int id)
    {
        return _context.Set<AppTask>().AsNoTracking().
               Where(t => t.CategoryId == id).ToList();
    }

    public IEnumerable<AppTask> GetByUserId(string id)
    {
        return _context.Set<AppTask>().
               AsNoTracking().Where(t => t.AspNetUsersId == id).ToList();
    }

}
