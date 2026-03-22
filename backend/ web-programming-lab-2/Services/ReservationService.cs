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

    private readonly DatabaseContext _dbContext;
    private readonly IMapper _mapper;

    public ReservationService(ILogger<ReservationService> logger, DatabaseContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
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
        var room = await _dbContext.Rooms.FindAsync(dto.Reservation.RoomId)
                   ?? throw new NotFoundException<Room>(dto.Reservation.RoomId);

        // перевірка чи номер вільний на ці дати
        var isAvailable = await IsRoomAvailableAsync(room.Id, dto.Reservation.CheckIn, dto.Reservation.CheckOut);

        if (!isAvailable)
            throw new ConflictException($"Room {dto.Reservation.RoomId} is already booked for these dates.",
                "ROOM_BOOKING_OVERLAP");

        // знайти або створити гостя по email
        var guest = await GetOrCreateGuestAsync(dto.Guest);


        var nights = dto.Reservation.CheckOut.DayNumber - dto.Reservation.CheckIn.DayNumber;
        var newGrandTotal = grandTotal ?? room.Price * nights;


        var reservation = _mapper.Map<Reservation>(dto.Reservation);
        reservation.Guest = guest;
        reservation.GrandTotal = newGrandTotal;

        _dbContext.Reservations.Add(reservation);
        await _dbContext.SaveChangesAsync();

        var dtoOut = _mapper.Map<ReservationDtoGet>(reservation);
        return dtoOut;
    }

    public async Task<ReservationDtoGet> Update(int id, ReservationDtoUpdate dto)
    {
        var reservation = await _dbContext.Reservations.FindAsync(id) ?? throw new NotFoundException<Reservation>(id);
        var newRoomId = dto.RoomId ?? reservation.RoomId;
        var room = await _dbContext.Rooms.FindAsync(newRoomId)
                   ?? throw new NotFoundException<Room>(newRoomId);
        
        if (dto.GuestCount.HasValue) reservation.GuestCount = dto.GuestCount.Value;

        if (dto.CheckIn.HasValue && dto.CheckIn != reservation.CheckIn
            || dto.CheckOut.HasValue && dto.CheckOut != reservation.CheckOut
            || dto.RoomId.HasValue && dto.RoomId != reservation.RoomId)
        {
            var newCheckIn = dto.CheckIn ?? reservation.CheckIn;
            var newCheckOut = dto.CheckOut ?? reservation.CheckOut;

            var isAvailable = await IsRoomAvailableAsync(newRoomId, newCheckIn, newCheckOut, reservation.Id);
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

        await _dbContext.SaveChangesAsync();
        return _mapper.Map<ReservationDtoGet>(reservation);
    }

    private async Task<Guest> GetOrCreateGuestAsync(GuestDtoCreate dto)
    {
        var guest = await _dbContext.Guests.FirstOrDefaultAsync(g => g.Email == dto.Email);
        if (guest == null)
        {
            guest = new Guest { Name = dto.Name, Email = dto.Email };
            _dbContext.Guests.Add(guest);
        }

        return guest;
    }

    private async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null)
    {
        return !await _dbContext.Reservations.AnyAsync(r =>
            r.RoomId == roomId &&
            r.IsActive &&
            r.CheckIn < checkOut &&
            r.CheckOut > checkIn &&
            r.Id != excludeReservationId);
    }

    public async Task<IEnumerable<ReservationDtoGet>> GetAllAsync(
        TimePeriodFilter rangeFilter,
        ReservationFilter filter
    )
    {
        IQueryable<Reservation> query = _dbContext.Reservations;

        if (rangeFilter.From.HasValue) query = query.Where(r => r.CheckIn >= rangeFilter.From.Value);
        if (rangeFilter.To.HasValue) query = query.Where(r => r.CheckIn <= rangeFilter.To.Value);
        if (filter.RoomId.HasValue) query = query.Where(r => r.RoomId == filter.RoomId.Value);
        if (filter.IsActive.HasValue) query = query.Where(r => r.IsActive == filter.IsActive.Value);


        query = filter.SortBy?.ToLower() switch
        {
            "grand_total" => filter.IsDescending
                ? query.OrderByDescending(r => r.GrandTotal)
                : query.OrderBy(r => r.GrandTotal),
            "check_in" => filter.IsDescending ? query.OrderByDescending(r => r.CheckIn) : query.OrderBy(r => r.CheckIn),
            "created_at" => filter.IsDescending
                ? query.OrderByDescending(r => r.CreatedAt)
                : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        var results = await query.ToListAsync();
        return results.Select(r => _mapper.Map<ReservationDtoGet>(r));
    }

    public async Task<ReservationDtoGet> GetByIdAsync(int id)
    {
        var result = await _dbContext.Reservations.FindAsync(id) ?? throw new NotFoundException<Reservation>(id);
        var dto = _mapper.Map<ReservationDtoGet>(result);
        return dto;
    }

    public async Task CancelReservation(int id)
    {
        var result = await _dbContext.Reservations.FindAsync(id) ?? throw new NotFoundException<Reservation>(id);
        if (!result.IsActive) return;

        result.IsActive = false;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveReservation(int id)
    {
        var result = await _dbContext.Reservations.FindAsync(id) ?? throw new NotFoundException<Reservation>(id);

        _dbContext.Reservations.Remove(result);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<ReservationDtoFullGet> GetFullByIdAsync(int id)
    {
        var result = await _dbContext.Reservations
                         .Include(r => r.Room)
                         .Include(r => r.Guest)
                         .FirstOrDefaultAsync(r => r.Id == id)
                     ?? throw new NotFoundException<Reservation>(id);

        var dto = _mapper.Map<ReservationDtoFullGet>(result);
        return dto;
    }
}