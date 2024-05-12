using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Models;
using task_timer.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using task_timer.DTOs;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppTasksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public AppTasksController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager,
                              IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<AppTaskDTO>> GetByUserId(string id)
    {
        var user = _userManager.GetUserId(User);

        var tasks = _unitOfWork.TasksRepository.GetByUserId(id);
        
        var tasksDTO = _mapper.Map<List<AppTaskDTO>>(tasks);

        return Ok(tasksDTO);
    }

    [HttpGet("{id:int:min(1)}")]
    public ActionResult<IEnumerable<AppTaskDTO>> Get(int id)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        var taskById = _unitOfWork.TasksRepository.Get(t => t.Id == id);

        if (taskById is null) {
            return BadRequest("This task doesn't exist.");
        }

        var taskDTO = _mapper.Map<AppTaskDTO>(taskById);


        return Ok(taskDTO);
    }

    [HttpPost]
    public async Task<ActionResult> Post(AppTaskDTO appTaskDTO)
    {
        if (appTaskDTO is null)
        {
            return BadRequest("All data must be provided.");
        }

        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        if (appTaskDTO.Beginning > appTaskDTO.End)
        {
            return BadRequest("The completion time of a task cannot be" +
                              "earlier than the start time. Please enter a valid time.");
        }

        var appTask = _mapper.Map<AppTask>(appTaskDTO);

        appTask.AspNetUsersId = userId;

        _unitOfWork.TasksRepository.CreateAsync(appTask);
        await _unitOfWork.CommitAsync();

        return Ok($"Task {appTask.Name} was successfully created.");
    }

    [HttpPost]
    [Route("api/[controller]/start")]
    public async Task<ActionResult> PostStart(AppTaskStartDTO appTaskStartDTO)
    {
        if (appTaskStartDTO is null)
        {
            return BadRequest("All data must be provided.");
        }

        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return BadRequest("User not found.");
        }

        var startTask = _mapper.Map<AppTask>(appTaskStartDTO);

        startTask.AspNetUsersId = userId;
        startTask.Beginning = DateTime.UtcNow;

        _unitOfWork.TasksRepository.CreateAsync(startTask);
        await _unitOfWork.CommitAsync();

        return Ok($"{startTask.Name} started at {startTask.Beginning}");

    }

    [HttpPatch]
    public async Task<ActionResult> PostStop(DateTime stop, int taskId)
    {
        var currentTask = _unitOfWork.TasksRepository.Get(t => t.Id == taskId);

        if (currentTask is null)
        {
            return BadRequest("This task doesn't exist.");
        }

        if (stop < currentTask.Beginning)
        {
            return BadRequest("The completion time of a task cannot be" +
                              "earlier than the start time. Please enter a valid time.");
        }

        currentTask.End = stop;
        await _unitOfWork.CommitAsync();

        return Ok($"{currentTask.Name} stopped at {currentTask.End}.");

    }

}
