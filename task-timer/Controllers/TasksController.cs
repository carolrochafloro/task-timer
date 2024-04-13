using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TTDbContext _context;

        public TasksController(TTDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppTask>> Get()
        {
            var tasks = _context.Tasks.ToList();

            return tasks;
        }
    }
}
