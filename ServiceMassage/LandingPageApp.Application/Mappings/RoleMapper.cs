using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Mappings;

public class RoleMapper : Profile
{
    public RoleMapper()
    {
        // Entity -> DTO
        CreateMap<Role, RoleDto>();

        // CreateRoleDto -> Entity
        CreateMap<CreateRoleDto, Role>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.People, opt => opt.Ignore());

        // UpdateRoleDto -> Entity
        CreateMap<UpdateRoleDto, Role>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.People, opt => opt.Ignore());
    }
}
