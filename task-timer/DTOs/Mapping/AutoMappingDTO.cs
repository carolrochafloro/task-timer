using AutoMapper;
using task_timer.DTOs;
using task_timer.Models;

namespace task_timer.DTOs.Mapping;

public class AutoMappingDTO : Profile
{
    public AutoMappingDTO()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        // map tasks       
    }
}
