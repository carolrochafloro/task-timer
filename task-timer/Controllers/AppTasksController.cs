using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using Npgsql.NodaTime;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using task_timer.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppTasksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AppTasksController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AppTask>> Get()
    {
        var tasks = _unitOfWork.TasksRepository.GetAllAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:int:min(1)}")]
    public ActionResult<IEnumerable<AppTask>> Get(int id)
    {
        var taskById = _unitOfWork.TasksRepository.Get(t => t.Id == id);

        if (taskById is null) {
            return BadRequest("This task doesn't exist.");
        }

        return Ok(taskById);
    }

    // post - manually logged task
    [HttpPost]
    public async Task<ActionResult> Post(AppTask appTask)
    {
        if (appTask is null)
        {
            return BadRequest("All data must be provided.");
        }

        // verify if userIs belongs to the logged user and if beginning is before end.

        _unitOfWork.TasksRepository.CreateAsync(appTask);
        await _unitOfWork.CommitAsync();

        return Ok($"Task {appTask.Name} was successfully created.");
    }

}
