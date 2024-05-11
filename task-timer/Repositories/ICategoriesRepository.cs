using Microsoft.AspNetCore.Http.HttpResults;
using task_timer.Models;

namespace task_timer.Repositories;

public interface ICategoriesRepository : IRepository<Category>
{
    public IEnumerable<Category> GetByUserId(string id);

}
