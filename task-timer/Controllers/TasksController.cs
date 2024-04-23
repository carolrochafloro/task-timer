using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using Npgsql.NodaTime;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly TTDbContext _context;

    public TasksController(TTDbContext context)
    {
        _context = context;
    }

    // User needs to be authenticated

    [HttpGet]
    public ActionResult<IEnumerable<AppTask>> Get()
    {
      
        var tasks = _context.Tasks.ToList();

        return tasks;
    }

    // post - manually logged task
    [HttpPost("/Manual")]
    public ActionResult Post(AppTask task)
    {
        if (task is null)
        {
            return BadRequest("All data must be provided.");
        }

        var DbTask = _context.Tasks.FirstOrDefault(t => t.Beginning == task.Beginning);

        if (DbTask != null)
        {
            return BadRequest($"{DbTask.Name} starts at the same time.");
        }

        _context.Tasks.Add(task);

        return Ok($"Task {task.Name} was successfully created.");
    }

    // post - timer task (start and finish)
}
