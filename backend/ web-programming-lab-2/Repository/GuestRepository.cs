using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Services;

namespace web_programming_lab_2.Repository;

public interface IGuestRepository
{
    Task<Guest?> GetByIdAsync(int id);
    Task<IEnumerable<Guest>> GetAllAsync();
    Task<Guest?> GetByIdWithReservationsAsync(int id);
    Task<Guest?> GetByEmailAsync(string email);
    void Add(Guest guest);
}

public class GuestRepository : IGuestRepository
{
    private readonly DatabaseContext _dbContext;

    public GuestRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guest?> GetByIdAsync(int id)
    {
        return await _dbContext.Guests.FindAsync(id);
    }

    public async Task<IEnumerable<Guest>> GetAllAsync()
    {
        return await _dbContext.Guests.ToListAsync();
    }

    public async Task<Guest?> GetByIdWithReservationsAsync(int id)
    {
        return await _dbContext.Guests
            .Include(r => r.Reservations)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Guest?> GetByEmailAsync(string email)
    {
        return await _dbContext.Guests.FirstOrDefaultAsync(g => g.Email == email);
    }

    public void Add(Guest guest)
    {
        _dbContext.Guests.Add(guest);
    }
}