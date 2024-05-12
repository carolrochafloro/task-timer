using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class AppTaskStartDTO
{
    [Required]
    public string? Name { get; set; }

    public string? Obs { get; set; }

    [Required]
    public int categoryId { get; set; }
}
