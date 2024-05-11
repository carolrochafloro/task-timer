using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class CategoriesRepository : Repository<Category>, ICategoriesRepository
{
    public CategoriesRepository(TTDbContext context, UserManager<AppUser> userManager) : base(context, userManager)
    {
    }

    public IEnumerable<Category> GetByUserId(string id)
    {

        return _context.Set<Category>().
               AsNoTracking().Where(t => t.AspNetUsersId == id).ToList();

    }
}
