using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetAll()
    {
        var result = await _roomService.GetAllAsync();
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _roomService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new
            {
                message = $"Room with id {id} not found."
            });
        }
        return Ok(result);
    }
    [HttpGet("{id}/reservations")]
    public async Task<IActionResult> GetReservations(int id)
    {
        var result = await _roomService.GetReservationsAsync(id);
        if (result == null)
        {
            return NotFound(new
            {
                message = $"Room with id {id} not found."
            });
        }
        //
        // if (result.Count() == 0)
        // {
        //     return NoContent();
        // }
        return Ok(result);
    }

}