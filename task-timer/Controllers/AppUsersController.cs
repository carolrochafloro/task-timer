using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using task_timer.Repositories;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppUsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public AppUsersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

    }

    // only admin user can access
    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<AppUser>> Get()
    {

        var users = _unitOfWork.UsersRepository.GetAllAsync();
        return Ok(users);

    }

    [HttpGet("{id:int:min(1)}")]
    public IActionResult Get([FromRoute] int id)
    {
        var user = _unitOfWork.UsersRepository.Get(u => u.Id == id);

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> PostUserAsync(AppUser user)
    {

        if (user is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbUser = _unitOfWork.UsersRepository.Get(u => u.Email == user.Email);

        if (dbUser != null)
        {
            return BadRequest("This e-mail already is already registered.");
        }

        // Using bcrypt to hash the password
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _unitOfWork.UsersRepository.CreateAsync(user);

        return Ok($"User {user.Email} registered.");

    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> PutAsync(AppUser user)
    {

        var previousUser = _unitOfWork.UsersRepository.Get(u => u.Email == user.Email);

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        _unitOfWork.UsersRepository.UpdateAsync(user);

        _unitOfWork.CommitAsync();

        return Ok($"User {user.Email} has been successfully updated.");

    }

}


