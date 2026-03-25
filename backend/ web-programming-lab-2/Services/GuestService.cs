using AutoMapper;
using Microsoft.EntityFrameworkCore;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Exceptions;

namespace web_programming_lab_2.Services;

public class GuestService
{
    private readonly ILogger<GuestService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GuestService(ILogger<GuestService> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GuestDtoGet> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.Guests.GetByIdAsync(id) ?? throw new NotFoundException<Guest>(id);
        var dto = _mapper.Map<GuestDtoGet>(result);
        return dto;
    }

    public async Task<IEnumerable<GuestDtoGet>> GetAllAsync()
    {
        var result = await _unitOfWork.Guests.GetAllAsync();
        return result.Select(r => _mapper.Map<GuestDtoGet>(r));
    }

    public async Task<IEnumerable<ReservationDtoGet>> GetReservationsAsync(int id)
    {
        var result = await _unitOfWork.Guests.GetByIdWithReservationsAsync(id)
                     ?? throw new NotFoundException<Guest>(id);

        var dtoEnumerable = result.Reservations?.OrderBy(r => r.CheckIn).Select(r => _mapper.Map<ReservationDtoGet>(r));
        return dtoEnumerable ?? [];
    }

    internal async Task<Guest> GetOrCreateGuestAsync(GuestDtoCreate dto)
    {
        var guest = await _unitOfWork.Guests.GetByEmailAsync(dto.Email);
        if (guest == null)
        {
            guest = new Guest { Name = dto.Name, Email = dto.Email };
            _unitOfWork.Guests.Add(guest);
        }

        return guest;
    }
}