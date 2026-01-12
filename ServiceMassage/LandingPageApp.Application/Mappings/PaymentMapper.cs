using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Mappings;

public class PaymentMapper : Profile
{
    public PaymentMapper()
    {
        // Entity -> DTO
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.PersonalName, opt => opt.MapFrom(src => src.Personal.Name));

        // DTO -> Entity
        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Amount, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Booking, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Personal, opt => opt.Ignore());
    }
}
