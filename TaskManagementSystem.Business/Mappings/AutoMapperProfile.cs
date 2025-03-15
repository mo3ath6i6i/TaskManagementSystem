using AutoMapper;
using TaskManagementSystem.Core.DTOs;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.Business.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<TaskItem, TaskDto>().ReverseMap();
            CreateMap<CreateTaskDto, TaskItem>();
            CreateMap<UpdateTaskDto, TaskItem>();
        }
    }
}