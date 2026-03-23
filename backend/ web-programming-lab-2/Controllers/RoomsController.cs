using Microsoft.AspNetCore.Mvc;
using web_programming_lab_2.Entities;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Entities.Rooms;
using web_programming_lab_2.Services;

namespace web_programming_lab_2.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly RoomService _roomService;

    public RoomsController(RoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDtoGet>>> GetAll()
    {
        var result = await _roomService.GetAllAsync();
        return Ok(result);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDtoGet>> GetById(int id)
    {
        var result = await _roomService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/reservations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservationDtoGet>>> GetReservations(int id)
    {
        var result = await _roomService.GetReservationsAsync(id);
        return Ok(result);
    }
    
    [HttpGet("{id}/availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservationDtoGet>>> GetReservations(int id, [FromQuery] TimePeriodFilter rangeFilter)
    {
        var result = await _roomService.GetAvailabilityAsync(id, rangeFilter);
        return Ok(result);
    }
}