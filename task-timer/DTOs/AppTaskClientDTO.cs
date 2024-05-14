using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class AppTaskClientDTO
{
    public string? Name { get; set; }
    public string? Obs { get; set; }
    public DateTime? Beginning { get; set; }
    public DateTime? End { get; set; }
    public TimeSpan? Duration { get; set; }
    public int categoryId { get; set; }
}
