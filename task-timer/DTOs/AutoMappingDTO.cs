using AutoMapper;
using task_timer.Models;

namespace task_timer.DTOs;

public class AutoMappingDTO : Profile
{
    public AutoMappingDTO()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<AppTask, AppTaskDTO>().ReverseMap();     
    }
}
