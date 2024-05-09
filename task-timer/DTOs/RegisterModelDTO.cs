using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class RegisterModelDTO
{
    [EmailAddress]
    [Required(ErrorMessage = "E-mail is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "User name is required.")]
    public string? Login { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string? Password { get; set; }

}
