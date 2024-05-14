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
    public ActionResult<IEnumerable<AppTaskClientDTO>> GetByUserId()
    {
        var user = _userManager.GetUserId(User);

        var tasks = _unitOfWork.TasksRepository.GetByUserId(user);

        var tasksDTO = _mapper.Map<List<AppTaskClientDTO>>(tasks);

        return Ok(tasksDTO);
    }

    [HttpGet("{id:int:min(1)}")]
    public ActionResult<IEnumerable<AppTaskClientDTO>> Get(int id)
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

        var taskDTO = _mapper.Map<AppTaskClientDTO>(taskById);

        return Ok(taskDTO);
    }

    [HttpGet("getbycategory/{id:int:min(1)}")]
    public ActionResult<IEnumerable<AppTaskClientDTO>> GetByCategory(int id)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        var taskByCategory = _unitOfWork.TasksRepository.GetByCategoryId(id);

        if (taskByCategory is null)
        {
            return BadRequest("There are no tasks related to this category.");
        }

        var tasksDTO = _mapper.Map<List<AppTaskClientDTO>>(taskByCategory);

        return Ok(tasksDTO);
    }

    [HttpGet]
    [Route("avgduration")]
    public ActionResult<AppTaskAvgDurationDTO> GetAvgDuration(int id)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        var taskByCategory = _unitOfWork.TasksRepository.GetByCategoryId(id);

        if (taskByCategory is null)
        {
            return BadRequest("There are no tasks related to this category.");
        }

        var averageDuration = TimeSpan.FromTicks((long)taskByCategory.
                              Average(t => t.Duration.Ticks));

        var totalTasks = taskByCategory.Count();

        var result = new AppTaskAvgDurationDTO
        {
            TotalTasks = totalTasks,
            AvgDuration = averageDuration
        };

        return Ok(result);
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

        var category = _unitOfWork.CategoriesRepository.
               Get(c => c.Id == appTaskDTO.categoryId);

        if (category is null || category.AspNetUsersId != userId)
        {
            return BadRequest("The category doesn't exist.");
        }

        var appTask = _mapper.Map<AppTask>(appTaskDTO);

        appTask.AspNetUsersId = userId;
        appTask.Duration = appTask.End - appTask.Beginning;

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

        var category = _unitOfWork.CategoriesRepository.
                       Get(c => c.Id == appTaskStartDTO.categoryId);

        if (category is null || category.AspNetUsersId != userId) 
        {
            return BadRequest("The category doesn't exist.");
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
        currentTask.Duration = stop - currentTask.Beginning;    
        await _unitOfWork.CommitAsync();

        return Ok($"{currentTask.Name} stopped at {currentTask.End}.");

    }

}
