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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoomService(ILogger<RoomService> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RoomDtoGet> GetByIdAsync(int id)
    {
        var result = await _unitOfWork.Rooms.GetByIdAsync(id) ?? throw new NotFoundException<Room>(id);
        return _mapper.Map<RoomDtoGet>(result);
    }

    public async Task<IEnumerable<RoomDtoGet>> GetAllAsync()
    {
        var result = await _unitOfWork.Rooms.GetAllAsync();
        return result.Select(r => _mapper.Map<RoomDtoGet>(r));
    }

    public async Task<Dictionary<DateOnly, AvailabilityDtoGet>> GetAvailabilityAsync(int id, TimePeriodFilter rangeFilter)
    {
        var from = rangeFilter.From ?? DateOnly.FromDateTime(DateTime.Today);
        var to = rangeFilter.To ?? from.AddMonths(1);

        var room = await _unitOfWork.Rooms.GetByIdWithActiveReservationsAsync(id, from, to)
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
        var result = await _unitOfWork.Rooms.GetByIdWithAllReservationsAsync(id)
                     ?? throw new NotFoundException<Room>(id);

        var dtoEnumerable = result.Reservations?.OrderBy(r => r.CheckIn).Select(r => _mapper.Map<ReservationDtoGet>(r));
        return dtoEnumerable ?? [];
    }
}