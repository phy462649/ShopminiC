using AutoMapper;
using LandingPageApp.Application.Dtos;
using LandingPageApp.Domain.Entities;

namespace LandingPageApp.Application.Mappings;

public class RoomMapper : Profile
{
    public RoomMapper()
    {
        CreateMap<Room, RoomDto>();

        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());

        CreateMap<UpdateRoomDto, Room>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Bookings, opt => opt.Ignore());
    }
}
