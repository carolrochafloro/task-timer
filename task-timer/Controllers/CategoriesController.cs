using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Models;
using task_timer.Repositories;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Category>> Get()
    {
        // get authentication and authorization to allow the user to 
        // only access the categories he created

        var categories = _unitOfWork.CategoriesRepository.GetAllAsync();
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

        await _unitOfWork.CategoriesRepository.CreateAsync(category);
        _unitOfWork.CommitAsync();
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
        dbCategory.UserId = category.UserId;

        _unitOfWork.CategoriesRepository.UpdateAsync(dbCategory);
        _unitOfWork.CommitAsync();

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
        _unitOfWork.CommitAsync();

        return Ok($"{category.Name} was successfully deleted.");

    }
}
