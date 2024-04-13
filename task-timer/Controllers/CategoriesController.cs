using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    public ActionResult<IEnumerable<Category>> Get()
    {
        var categories = _context.Categories.ToList();
        return categories;
    }

}
