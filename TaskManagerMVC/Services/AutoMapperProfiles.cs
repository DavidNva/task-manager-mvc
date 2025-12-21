using AutoMapper;
using TaskManagerMVC.Entities;
using TaskManagerMVC.Models;

namespace TaskManagerMVC.Services
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<TaskItem, TaskDTO>()
                .ForMember(dto => dto.PasosTotal, ent => ent.MapFrom(x => x.Steps.Count()))
                .ForMember(dto => dto.PasosRealizados, ent =>
                    ent.MapFrom(x => x.Steps.Where(p => p.IsCompleted).Count()));
            CreateMap<TaskDTO, TaskItem>();
        }
    }
}
