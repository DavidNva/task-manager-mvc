using AutoMapper;
using TaskManagerMVC.Entities;
using TaskManagerMVC.Models;

namespace TaskManagerMVC.Services
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<TaskItem, TaskDTO>();
            CreateMap<TaskDTO, TaskItem>();
        }
    }
}
