using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> PostUserAsync(AppUser user)
    {

        if (user is null)
        {
            return BadRequest("All data must be provided.");
        }

            // Using bcrypt to hash the password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered.");

    }

    [HttpPost("login")]
    public async Task<IActionResult> PostLoginAsync(string email, string password)
    {
        // find user by email
        // compare password with bcrypt
        // generate token

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null)
        {
            return Unauthorized("User or password are invalid.");
        }

        bool validation = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!validation)
        {
            return Unauthorized("User or password are invalid.");
        }

        // jwt

        return Ok("Login successful.");
    }


}
