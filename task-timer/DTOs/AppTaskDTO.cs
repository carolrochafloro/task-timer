﻿using System.ComponentModel.DataAnnotations;

namespace task_timer.DTOs;

public class AppTaskDTO
{
    [Required]
    public string? Name;

    public string? Obs;

    [Required]
    public DateTime? Beginning;

    [Required]
    public DateTime? End;

    [Required]
    public int categoryId;

}
