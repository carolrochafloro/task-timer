﻿using Microsoft.AspNetCore.Mvc;
using task_timer.Context;
using task_timer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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

    private readonly IConfiguration _configuration;
    public AppUsersController(TTDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;

    }

    // Create user
    [HttpPost]
    public async Task<IActionResult> PostUserAsync(AppUser user)
    {

        if (user is null)
        {
            return BadRequest("All data must be provided.");
        }

        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

        if (dbUser != null)
        {
            return BadRequest("This e-mail already is already registered.");
        }

        // Using bcrypt to hash the password
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        try
        {
            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered.");
        }
        catch (Exception)
        {

            return BadRequest("Internal server error.");
        }


    }

    [HttpPost("login")]
    public async Task<IActionResult> PostLoginAsync(string email, string password)
    {
        // find user by email
        // compare password with bcrypt
        // generate token

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

        try
        {
            if (user is null)
            {
                return Unauthorized("User or password are invalid.");
            }

            bool validation = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (!validation)
            {
                return Unauthorized("User or password are invalid.");
            }

            // Definir os claims do token
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSettings:secretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["jwtSettings:issuer"],
                audience: _configuration["jwtSettings:audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
        catch (Exception)
        {

            return BadRequest("Internal server error.");
        }


    }

    //[Authorize]
    [HttpPut("{id:int:min(1)}")]
    public async Task<IActionResult> PutAsync(AppUser user)
    {
        // se trocar senha, fazer hash. encontrar user pelo bearer token,
        // verificar se cada item foi enviado e, se sim, fazer a troca

        // receber id do usuário pelo jwt

        var previousUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);

        try
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _context.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User data has been successfully updated.");
        }
        catch (Exception)
        {
            return BadRequest("Internal server error.");
        }

    }
}


