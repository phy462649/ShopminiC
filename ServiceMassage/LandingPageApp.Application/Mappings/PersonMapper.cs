using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Mappings;

public class PersonMapper : Profile
{
    public PersonMapper()
    {
        CreateMap<Person, PersonDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));

        CreateMap<Person, PersonDetailDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.BookingCount, opt => opt.MapFrom(src => src.BookingCustomers.Count))
            .ForMember(dest => dest.OrderCount, opt => opt.MapFrom(src => src.Orders.Count));

        CreateMap<CreatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.StatusVerify, opt => opt.Ignore())
            .ForMember(dest => dest.OTP, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.BookingCustomers, opt => opt.Ignore())
            .ForMember(dest => dest.BookingStaffs, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.StaffSchedules, opt => opt.Ignore());

        CreateMap<UpdatePersonDto, Person>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.StatusVerify, opt => opt.Ignore())
            .ForMember(dest => dest.OTP, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.BookingCustomers, opt => opt.Ignore())
            .ForMember(dest => dest.BookingStaffs, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Payments, opt => opt.Ignore())
            .ForMember(dest => dest.StaffSchedules, opt => opt.Ignore());
    }
}
