using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Models;
using task_timer.Repositories;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;

    public CategoriesController(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Category>> GetByUserId()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return BadRequest("User not found.");
        }

        var categories = _unitOfWork.CategoriesRepository.GetByUserId(userId);
        return Ok(categories);

    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public ActionResult<Category> Get(int id)
    {
        var category = _unitOfWork.CategoriesRepository.Get(c => c.Id == id);

        if (category == null)
        {
            return NotFound("The category doesn't exist.");
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Category category)
    {
        if (category is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbCategory = _unitOfWork.CategoriesRepository.Get(c => c.Name == category.Name);
       
        if (dbCategory != null)
        {
            return BadRequest("This category already exists.");
        }

        _unitOfWork.CategoriesRepository.CreateAsync(category);
        await _unitOfWork.CommitAsync();
        return Ok($"Created:\nName: {category.Name}\nDescription: {category.Description}");

    }

    // fix - copy category properties to dbcategory and then pass it to the update method
    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Put([FromRoute] int id, [FromBody] Category category)
    {
        if (category is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbCategory = _unitOfWork.CategoriesRepository.Get(c => c.Id == id);

        if (dbCategory is null)
        {
            return BadRequest($"Could not find category {id}");
        }

        dbCategory.Name = category.Name;
        dbCategory.Description = category.Description;
        dbCategory.ImgUrl = category.ImgUrl;

        // to do: get user id from header if possible
        dbCategory.AspNetUsersId = category.AspNetUsersId;

        _unitOfWork.CategoriesRepository.UpdateAsync(dbCategory);
        await _unitOfWork.CommitAsync();

        return Ok($"{dbCategory.Name} has been updated.");
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var category = _unitOfWork.CategoriesRepository.Get(c => c.Id == id);

        if (category is null)
        {
            return NotFound("The category you're trying to delete doesn't exist.");
        }

        if (category.appTasks.Any())
        {
            return BadRequest($"{category.Name} cannot be deleted because it's associated with existing tasks.");
        }

        _unitOfWork.CategoriesRepository.DeleteAsync(category);
        await _unitOfWork.CommitAsync();

        return Ok($"{category.Name} was successfully deleted.");

    }
}
