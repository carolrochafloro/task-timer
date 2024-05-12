using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class AppTaskDTO
{
    [Required]
    public string? Name { get; set; }

    public string? Obs {  get; set; }

    [Required]
    public DateTime? Beginning {  get; set; }

    [Required]
    public DateTime? End {  get; set; }

    [Required]
    public int categoryId { get; set; }

}
