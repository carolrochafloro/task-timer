namespace task_timer.DTOs;

public class AppTaskAvgDurationDTO
{
    public int TotalTasks { get; set; }
    public TimeSpan AvgDuration { get; set; }
}
