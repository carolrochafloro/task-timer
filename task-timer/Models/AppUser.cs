using Microsoft.AspNetCore.Identity;

namespace task_timer.Models;

public class AppUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenEspiryTime { get; set; }
}
