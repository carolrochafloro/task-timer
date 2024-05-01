using task_timer.Context;
using task_timer.Models;

namespace task_timer.Repositories;

public class UsersRepository : Repository<AppUser>, IUsersRepository
{
    public UsersRepository(TTDbContext context) : base(context)
    {
    }

}
