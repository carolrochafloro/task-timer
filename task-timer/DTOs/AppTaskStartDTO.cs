using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class AppTaskStartDTO
{
    [Required]
    public string? Name;

    public string? Obs;

    [Required]
    public int categoryId;
}
