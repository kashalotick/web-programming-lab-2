using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Exceptions;

namespace web_programming_lab_2.Services;

public class ReservationService
{
    private readonly ILogger<ReservationService> _logger;

    private readonly DatabaseContext _dbContext;
    private readonly IMapper _mapper;

    public ReservationService(ILogger<ReservationService> logger, DatabaseContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ReservationDtoGet> CreateReservation(ReservationCreateRequest createRequest)
    {
        // var roomExists = await _dbContext.Rooms.AnyAsync(r => r.Id == createRequest.RoomId);
        //
        // if (!roomExists)
        // {
        //     throw new NotFoundException<Room>(createRequest.RoomId);
        // }
        //
        // var hasOverlap = await _dbContext.Reservations
        //     .AnyAsync(r =>
        //         r.RoomId == createRequest.RoomId && r.CheckIn < createRequest.CheckOut && r.CheckOut > createRequest.CheckIn);
        //
        // if (hasOverlap)
        // {
        //     throw new ConflictException($"Room {createRequest.RoomId} is already booked for these dates.","ROOM_BOOKING_OVERLAP");
        //     
        // }
        //
        // var reservation = _mapper.Map<Reservation>(createRequest);
        // _dbContext.Reservations.Add(reservation);
        //
        // await _dbContext.SaveChangesAsync();
        //
        // var output = _mapper.Map<ReservationDtoGet>(reservation);
        //
        // return output;
        return Task.FromResult(new ReservationDtoGet()).Result;
    }

    public async Task<IEnumerable<ReservationDtoGet>> GetAllAsync()
    {
        var reservations = await _dbContext.Reservations.ToListAsync();
        return reservations.Select(r => _mapper.Map<ReservationDtoGet>(r));
    }

    public async Task<ReservationDtoGet?> GetByIdAsync(int id)
    {
        var reservation = await _dbContext.Reservations.FindAsync(id);
        return _mapper.Map<ReservationDtoGet>(reservation);
    }

    public async Task CancelReservation(int id)
    {
        var reservation = await _dbContext.Reservations.FindAsync(id);
        if (reservation == null)
        {
            throw new NotFoundException<Reservation>(id);
        }

        if (!reservation.IsActive) return;

        reservation.IsActive = false;
        await _dbContext.SaveChangesAsync();
    }
}