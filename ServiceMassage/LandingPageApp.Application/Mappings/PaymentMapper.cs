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
            .ForMember(dest => dest.PersonalName, opt => opt.MapFrom(src => src.Personal != null ? src.Personal.Name : null))
            .ForMember(dest => dest.PersonalPhone, opt => opt.MapFrom(src => src.Personal != null ? src.Personal.Phone : null))
            .ForMember(dest => dest.PersonalEmail, opt => opt.MapFrom(src => src.Personal != null ? src.Personal.Email : null))
            .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.Status.ToString() : null))
            .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.StartTime : (DateTime?)null))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Order != null ? src.Order.Status.ToString() : null))
            .ForMember(dest => dest.OrderTotalAmount, opt => opt.MapFrom(src => src.Order != null ? src.Order.TotalAmount : null));

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
