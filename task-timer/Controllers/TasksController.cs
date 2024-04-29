using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using Npgsql.NodaTime;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
    public ActionResult PostManual([FromBody] AppTask task)
    {
        if (task is null || task.End == default(DateTime))
        {
            return BadRequest("All data must be provided.");
        }

        var dbTask = _context.Tasks.FirstOrDefault(t => t.Beginning == task.Beginning);

        if (dbTask != null)
        {
            return BadRequest($"{dbTask.Name} starts at the same time.");
        }

        _context.Tasks.Add(task);
        _context.SaveChangesAsync();

        return Ok($"Task {task.Name} was successfully created.");
    }

    // post - timer task (start)

    [HttpPost("/Start")]
    public async Task<ActionResult> Post(AppTask task)
    {
        // creates a new task and initiates it

        if (task is null)
        {
            return BadRequest("All data must me provided.");
        }

        var dbTask = await _context.Tasks.FirstOrDefaultAsync(t => t.Beginning == task.Beginning);

        if (dbTask != null)
        {
            return BadRequest($"{dbTask.Name} starts at the same time.");
        }

        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return Ok();

    }

    [HttpPut("/End/{id:int:min(1)}")]
    public async Task<ActionResult> PutEnd([FromRoute] int id, [FromBody]AppTask task)
    {
        return Ok("");
    }




}
