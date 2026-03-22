using AutoMapper;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;

namespace web_programming_lab_2.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reservation, ReservationDtoGet>();
        CreateMap<ReservationCreateRequest, Reservation>();
        
        CreateMap<Room, RoomDtoGet>();

    }
}