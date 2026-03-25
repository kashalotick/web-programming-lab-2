using web_programming_lab_2.Repository;

namespace web_programming_lab_2.Services;

public interface IUnitOfWork
{
    IGuestRepository Guests { get; }
    IRoomRepository Rooms { get; }
    IReservationRepository Reservations { get; }
    
    Task<int> SaveChangesAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _dbContext;
    private IGuestRepository? _guestRepository;
    private IRoomRepository? _roomRepository;
    private IReservationRepository? _reservationRepository;

    public UnitOfWork(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IGuestRepository Guests => _guestRepository ??= new GuestRepository(_dbContext);
    public IRoomRepository Rooms => _roomRepository ??= new RoomRepository(_dbContext);
    public IReservationRepository Reservations => _reservationRepository ??= new ReservationRepository(_dbContext);

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}