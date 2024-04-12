using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using BCrypt.Net;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppUsersController : ControllerBase
{
    // DI for TTDbContext is set up here. An instance of TTDbContext
    // is provided by ASP.NET Core's built-in DI system whenever an AppUsersController is
    // created. This instance is used to interact with the database, keeping the controller
    // decoupled from specific database technologies.

    private readonly TTDbContext _context;
    public AppUsersController(TTDbContext context)
    {
        _context = context;
    }

     // Create user
    [HttpPost]
    public ActionResult Post(AppUser user)
    {
        try
        {
            if (user is null)
            {
                return BadRequest("All data must be provided.");
            }

            // Using bcrypt to hash the password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Add(user);
            _context.SaveChanges();

            return Ok("User registered.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
        }
;
    }


}
