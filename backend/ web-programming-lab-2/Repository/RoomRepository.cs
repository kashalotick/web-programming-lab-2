using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Services;

namespace web_programming_lab_2.Repository;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int id);
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room?> GetByIdWithActiveReservationsAsync(int id, DateOnly from, DateOnly to);
    Task<Room?> GetByIdWithAllReservationsAsync(int id);
}

public class RoomRepository : IRoomRepository
{
    private readonly DatabaseContext _dbContext;

    public RoomRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _dbContext.Rooms.FindAsync(id);
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _dbContext.Rooms.ToListAsync();
    }

    public async Task<Room?> GetByIdWithActiveReservationsAsync(int id, DateOnly from, DateOnly to)
    {
        return await _dbContext.Rooms
            .Include(r => r.Reservations!
                .Where(res => res.IsActive && res.CheckIn < to && res.CheckOut > from))
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Room?> GetByIdWithAllReservationsAsync(int id)
    {
        return await _dbContext.Rooms
            .Include(r => r.Reservations)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}