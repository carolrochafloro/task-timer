using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using task_timer.DTOs;
using task_timer.Models;
using task_timer.Serices;

namespace task_timer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(ITokenService tokenService, UserManager<AppUser> userManager,
                          RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("login")]

    public async Task<IActionResult> Login([FromBody] LoginModelDTO modelDTO)
    {
        var user = await _userManager.FindByNameAsync(modelDTO.UserName);

        if (user is not null && await _userManager.CheckPasswordAsync(user, modelDTO.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // adds user roles to token
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                             out int refreshTokenValInMin);

            user.RefreshTokenEspiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValInMin);

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO modelDTO)
    {
        var userExists = await _userManager.FindByNameAsync(modelDTO.Login);

        if (userExists is not null)
        {
            return StatusCode(StatusCodes.Status400BadRequest,
                   new ResponseDTO { Status = "Error", Message = "User already exists." });
        }

        AppUser user = new()
        {
            Email = modelDTO.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = modelDTO.Login,
        };

        var result = await _userManager.CreateAsync(user, modelDTO.Password);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ResponseDTO { Status = "Error", Message = "User registry failed." });
        }

        return Ok(new ResponseDTO { Status = "Success", Message = "User registered." });

    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModelDTO tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest("Invalid client request.");

        }
        string? accessToken = tokenModel.AccessToken ??
                              throw new ArgumentNullException(nameof(tokenModel));

        string refreshToken = tokenModel.RefreshToken ??
                              throw new ArgumentNullException(nameof(tokenModel));

        var principal = _tokenService.GetPrincipalExpiredToken(accessToken!, _configuration);

        if (principal == null)
        {
            return BadRequest("Invalid access token/refresh token.");
        }

        string userName = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(userName);

        if (user == null || user.RefreshToken != refreshToken ||
            user.RefreshTokenEspiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid access token/refresh toekn.");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(),
                             _configuration);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken,
        });
    }

    [HttpPost]
    [Route("revoke/[username]")]

    public async Task<IActionResult> Revoke(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return BadRequest("Invalid user name.");
        }

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}
