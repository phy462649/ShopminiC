using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Mappings;

public class ServiceMapper : Profile
{
    public ServiceMapper()
    {
        CreateMap<Service, ServiceDto>();

        CreateMap<CreateServiceDto, Service>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BookingServices, opt => opt.Ignore());

        CreateMap<UpdateServiceDto, Service>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.BookingServices, opt => opt.Ignore());
    }
}
