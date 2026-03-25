using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Services;

namespace web_programming_lab_2.Repository;

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id);
    Task<Reservation?> GetFullByIdAsync(int id);
    Task<IEnumerable<Reservation>> GetAllFilteredAsync(TimePeriodFilter rangeFilter, ReservationFilter filter);
    Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null);
    void Add(Reservation reservation);
    void Remove(Reservation reservation);
}

public class ReservationRepository : IReservationRepository
{
    private readonly DatabaseContext _dbContext;

    public ReservationRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Reservation?> GetByIdAsync(int id)
    {
        return await _dbContext.Reservations.FindAsync(id);
    }

    public async Task<Reservation?> GetFullByIdAsync(int id)
    {
        return await _dbContext.Reservations
            .Include(r => r.Room)
            .Include(r => r.Guest)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Reservation>> GetAllFilteredAsync(TimePeriodFilter rangeFilter, ReservationFilter filter)
    {
        IQueryable<Reservation> query = _dbContext.Reservations;

        if (rangeFilter.From.HasValue) query = query.Where(r => r.CheckIn >= rangeFilter.From.Value);
        if (rangeFilter.To.HasValue) query = query.Where(r => r.CheckIn <= rangeFilter.To.Value);
        if (filter.RoomId.HasValue) query = query.Where(r => r.RoomId == filter.RoomId.Value);
        if (filter.IsActive.HasValue) query = query.Where(r => r.IsActive == filter.IsActive.Value);

        query = filter.SortBy?.ToLower() switch
        {
            "grand_total" => filter.IsDescending ? query.OrderByDescending(r => r.GrandTotal) : query.OrderBy(r => r.GrandTotal),
            "check_in" => filter.IsDescending ? query.OrderByDescending(r => r.CheckIn) : query.OrderBy(r => r.CheckIn),
            "created_at" => filter.IsDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        return await query.ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null)
    {
        return !await _dbContext.Reservations.AnyAsync(r =>
            r.RoomId == roomId &&
            r.IsActive &&
            r.CheckIn < checkOut &&
            r.CheckOut > checkIn &&
            r.Id != excludeReservationId);
    }

    public void Add(Reservation reservation)
    {
        _dbContext.Reservations.Add(reservation);
    }

    public void Remove(Reservation reservation)
    {
        _dbContext.Reservations.Remove(reservation);
    }
}