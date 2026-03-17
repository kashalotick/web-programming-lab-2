using Microsoft.AspNetCore.Mvc;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Services;

namespace web_programming_lab_2.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _reservationService;

    public ReservationsController(ReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationDtoCreate dto)
    {
        try
        {
            var result = await _reservationService.CreateReservation(dto);


            if (result == null)
            {
                return NotFound(new
                {
                    message = $"Room with id {dto.RoomId} not found."
                });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reservationService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _reservationService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new
            {
                message = $"Reservation with id {id} not found."
            });
        }

        return Ok(result);
    }
}