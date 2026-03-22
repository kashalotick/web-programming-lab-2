using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<IEnumerable<RoomDtoGet>>> GetAll()
    {
        var result = await _roomService.GetAllAsync();
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDtoGet>> GetById(int id)
    {
        var result = await _roomService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/reservations")]
    public async Task<ActionResult<IEnumerable<ReservationDtoGet>>> GetReservations(int id)
    {
        var result = await _roomService.GetReservationsAsync(id);
        return Ok(result);
    }
}