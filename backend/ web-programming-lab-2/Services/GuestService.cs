using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Exceptions;

namespace web_programming_lab_2.Services;

public class GuestService
{
    private readonly ILogger<GuestService> _logger;

    private readonly DatabaseContext _dbContext;
    private readonly IMapper _mapper;

    public GuestService(ILogger<GuestService> logger, DatabaseContext dbContext, IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<GuestDtoGet> GetByIdAsync(int id) // TODO: dto?
    {
        var result = await _dbContext.Guests.FindAsync(id) ?? throw new NotFoundException<Guest>(id);
        var dto = _mapper.Map<GuestDtoGet>(result);
        return dto;
    }

    public async Task<IEnumerable<GuestDtoGet>> GetAllAsync()
    {
        var result = await _dbContext.Guests.ToListAsync();
        return result.Select(r => _mapper.Map<GuestDtoGet>(r));
    }

    public async Task<IEnumerable<ReservationDtoGet>> GetReservationsAsync(int id)
    {
        var result = await _dbContext.Guests
                         .Include(r => r.Reservations)
                         .FirstOrDefaultAsync(r => r.Id == id)
                     ?? throw new NotFoundException<Guest>(id);

        var dtoEnumerable = result.Reservations?.OrderBy(r => r.CheckIn).Select(r => _mapper.Map<ReservationDtoGet>(r));
        return dtoEnumerable ?? [];
    }
}