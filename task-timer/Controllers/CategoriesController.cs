using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using task_timer.Context;
using task_timer.Models;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly TTDbContext _context;

    public CategoriesController(TTDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> Get()
    {
        // get authentication and authorization to allow the user to 
        // only access the categories he created

        var categories = await _context.Categories.ToListAsync();
        return categories;
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCategory")]
    public async Task<ActionResult<Category>> Get(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound("The category doesn't exist.");
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Category category)
    {
        var dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category.Name);

        if (category is null)
        {
            return BadRequest("All data must be provided.");
        }

        if (dbCategory != null)
        {
            return BadRequest("This category already exists.");
        }

        try
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return Ok(category);
        }

        catch (Exception)
        {
            return BadRequest("Internal server error.");
        }
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> Put([FromRoute] int id, [FromBody] Category category)
    {
        if (category is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (dbCategory is null)
        {
            return BadRequest($"Could not find category {id}");
        }

        _context.Entry(dbCategory).CurrentValues.SetValues(category);

        await _context.SaveChangesAsync();

        return Ok($"{dbCategory.Name} has been updated.");
    }

    [HttpDelete("{id:int:min(10)}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return NotFound("The category you're trying to delete doesn't exist.");
        }

        if (category.Tasks.Any())
        {
            return BadRequest($"{category.Name} cannot be deleted because it's associated with existing tasks.");
        }

        _context.Remove(category);
        await _context.SaveChangesAsync();

        return Ok($"{category.Name} was successfully deleted.");

    }
}
