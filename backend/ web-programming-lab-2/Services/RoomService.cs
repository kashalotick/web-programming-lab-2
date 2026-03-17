using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Room> GetByIdAsync(int id)
    {
        var room = await _dbContext.Rooms.FindAsync(id);

        return room ?? throw new NotFoundException<Room>(id);
    }

    public async Task<IEnumerable<ReservationDtoGet>> GetReservationsAsync(int id)
    {
        var room = await _dbContext.Rooms.Include(r => r.Reservations).FirstOrDefaultAsync(r => r.Id == id);

        var reservations = room?.Reservations?.OrderBy(r => r.CheckIn).Select(r => _mapper.Map<ReservationDtoGet>(r));
        return reservations ?? throw new NotFoundException<Room>(id);
    }

    public async Task<IEnumerable<RoomDtoGet>> GetAllAsync()
    {
        var rooms = await _dbContext.Rooms.ToListAsync();
        return rooms.Select(r => _mapper.Map<RoomDtoGet>(r));
    }
}