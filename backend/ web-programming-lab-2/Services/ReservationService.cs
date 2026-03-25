using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Exceptions;

namespace web_programming_lab_2.Services;

public class ReservationService
{
    private readonly ILogger<ReservationService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly GuestService _guestService;

    public ReservationService(ILogger<ReservationService> logger, IUnitOfWork unitOfWork, IMapper mapper, GuestService guestService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _guestService = guestService;
    }

    public async Task<ReservationDtoGet> ReserveRoom(ReservationCreateRequest dto)
    {
        return await CreateReservation(dto, null);
    }

    public async Task<ReservationDtoGet> Create(ReservationCreateRequest dto)
    {
        return await CreateReservation(dto, dto.Reservation.GrandTotal);
    }

    private async Task<ReservationDtoGet> CreateReservation(ReservationCreateRequest dto, int? grandTotal)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(dto.Reservation.RoomId)
                   ?? throw new NotFoundException<Room>(dto.Reservation.RoomId);

        var isAvailable = await _unitOfWork.Reservations.IsRoomAvailableAsync(room.Id, dto.Reservation.CheckIn, dto.Reservation.CheckOut);

        if (!isAvailable)
            throw new ConflictException($"Room {dto.Reservation.RoomId} is already booked for these dates.",
                "ROOM_BOOKING_OVERLAP");
        
        var guest = await _guestService.GetOrCreateGuestAsync(dto.Guest);

        var nights = dto.Reservation.CheckOut.DayNumber - dto.Reservation.CheckIn.DayNumber;
        var newGrandTotal = grandTotal ?? room.Price * nights;

        var reservation = _mapper.Map<Reservation>(dto.Reservation);
        reservation.Guest = guest;
        reservation.GrandTotal = newGrandTotal;
        
        _unitOfWork.Reservations.Add(reservation);
        await _unitOfWork.SaveChangesAsync();

        var dtoOut = _mapper.Map<ReservationDtoGet>(reservation);
        return dtoOut;
    }

    public async Task<ReservationDtoGet> Update(int id, ReservationDtoUpdate dto)
    {
        var reservation = await _unitOfWork.Reservations.GetByIdAsync(id) ?? throw new NotFoundException<Reservation>(id);
        var newRoomId = dto.RoomId ?? reservation.RoomId;
        var room = await _unitOfWork.Rooms.GetByIdAsync(newRoomId)
                   ?? throw new NotFoundException<Room>(newRoomId);
        
        if (dto.GuestCount.HasValue) reservation.GuestCount = dto.GuestCount.Value;

        if (dto.CheckIn.HasValue && dto.CheckIn != reservation.CheckIn
            || dto.CheckOut.HasValue && dto.CheckOut != reservation.CheckOut
            || dto.RoomId.HasValue && dto.RoomId != reservation.RoomId)
        {
            var newCheckIn = dto.CheckIn ?? reservation.CheckIn;
            var newCheckOut = dto.CheckOut ?? reservation.CheckOut;

            var isAvailable = await _unitOfWork.Reservations.IsRoomAvailableAsync(newRoomId, newCheckIn, newCheckOut, reservation.Id);
            if (!isAvailable)
                throw new ConflictException($"Room {newRoomId} is already booked for these dates.",
                    "ROOM_BOOKING_OVERLAP"
                );
            reservation.CheckIn = newCheckIn;
            reservation.CheckOut = newCheckOut;
            reservation.RoomId = newRoomId;
            
            var nights = newCheckOut.DayNumber - newCheckIn.DayNumber;
            reservation.GrandTotal = room.Price * nights;
        }

        if (dto.GrandTotal.HasValue)
        {
            reservation.GrandTotal = dto.GrandTotal.Value;
        }

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ReservationDtoGet>(reservation);
    }



    public async Task<IEnumerable<ReservationDtoGet>> GetAllAsync(TimePeriodFilter rangeFilter, ReservationFilter filter)
    {
        var results = await _unitOfWork.Reservations.GetAllFilteredAsync(rangeFilter, filter);
        return results.Select(r => _mapper.Map<ReservationDtoGet>(r));
    }

    public async Task<ReservationDtoGet> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.Reservations.GetByIdAsync(id) ?? throw new NotFoundException<Reservation>(id);
        return _mapper.Map<ReservationDtoGet>(result);
    }

    public async Task CancelReservation(int id)
    {
        var result = await _unitOfWork.Reservations.GetByIdAsync(id) ?? throw new NotFoundException<Reservation>(id);
        if (!result.IsActive) return;

        result.IsActive = false;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveReservation(int id)
    {
        var result = await _unitOfWork.Reservations.GetByIdAsync(id) ?? throw new NotFoundException<Reservation>(id);

        _unitOfWork.Reservations.Remove(result);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ReservationDtoFullGet> GetFullByIdAsync(int id)
    {
        var result = await _unitOfWork.Reservations.GetFullByIdAsync(id)
                     ?? throw new NotFoundException<Reservation>(id);

        return _mapper.Map<ReservationDtoFullGet>(result);
    }
}