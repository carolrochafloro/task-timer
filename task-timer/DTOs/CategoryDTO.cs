using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class CategoryDTO
{
    [Required]
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(400)]
    public string? Description { get; set; }

}
