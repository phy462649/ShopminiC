using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Mappings;

public class StaffScheduleMapper : Profile
{
    public StaffScheduleMapper()
    {
        // Entity -> DTO
        CreateMap<StaffSchedule, StaffScheduleDto>()
            .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff.Name));

        // DTO -> Entity
        CreateMap<CreateStaffScheduleDto, StaffSchedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore());

        CreateMap<UpdateStaffScheduleDto, StaffSchedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StaffId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore());

        CreateMap<ScheduleItemDto, StaffSchedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StaffId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Staff, opt => opt.Ignore());
    }
}
