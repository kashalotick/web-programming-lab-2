using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Exceptions;

namespace web_programming_lab_2.Services;

public class RoomService
{
    private readonly ILogger<RoomService> _logger;

    private readonly DatabaseContext _dbContext;
    private readonly IMapper _mapper;

    public RoomService(ILogger<RoomService> logger, DatabaseContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<RoomDtoGet> GetByIdAsync(int id)
    {
        var result = await _dbContext.Rooms.FindAsync(id) ?? throw new NotFoundException<Room>(id);
        var dto = _mapper.Map<RoomDtoGet>(result);
        return dto;
    }

    public async Task<IEnumerable<RoomDtoGet>> GetAllAsync()
    {
        var result = await _dbContext.Rooms.ToListAsync();
        return result.Select(r => _mapper.Map<RoomDtoGet>(r));
    }

    public async Task<Dictionary<DateOnly, AvailabilityDtoGet>> GetAvailabilityAsync(
        int id,
        TimePeriodFilter rangeFilter
    )
    {
        var from = rangeFilter.From ?? DateOnly.FromDateTime(DateTime.Today);
        var to = rangeFilter.To ?? from.AddMonths(1);

        var room = await _dbContext.Rooms
                       .Include(r => r.Reservations!
                           .Where(r => r.IsActive && r.CheckIn < to && r.CheckOut > from))
                       .FirstOrDefaultAsync(r => r.Id == id)
                   ?? throw new NotFoundException<Room>(id);

        var result = new Dictionary<DateOnly, AvailabilityDtoGet>();

        for (var date = from; date < to; date = date.AddDays(1))
        {
            var isTaken = room.Reservations!.Any(r => r.CheckIn <= date && r.CheckOut > date);
            result[date] = new AvailabilityDtoGet
            {
                Price = room.Price,
                IsAvailable = !isTaken
            };
        }

        return result;
    }

    public async Task<IEnumerable<ReservationDtoGet>> GetReservationsAsync(int id)
    {
        var result = await _dbContext.Rooms
                         .Include(r => r.Reservations)
                         .FirstOrDefaultAsync(r => r.Id == id)
                     ?? throw new NotFoundException<Room>(id);

        var dtoEnumerable = result.Reservations?.OrderBy(r => r.CheckIn).Select(r => _mapper.Map<ReservationDtoGet>(r));
        return dtoEnumerable ?? [];
    }
}