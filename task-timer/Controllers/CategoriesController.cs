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

}
