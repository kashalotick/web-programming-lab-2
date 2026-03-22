using AutoMapper;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;

namespace web_programming_lab_2.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Reservation, ReservationDtoGet>();
        CreateMap<Reservation, ReservationDtoFullGet>(); // TODO: works??
        CreateMap<ReservationDtoCreate, Reservation>();
        
        CreateMap<Room, RoomDtoGet>();
        
        CreateMap<Guest, GuestDtoGet>();
        CreateMap<GuestDtoCreate, Guest>();
    }
}