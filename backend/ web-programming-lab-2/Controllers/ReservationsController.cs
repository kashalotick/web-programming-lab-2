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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDtoGet>> Create([FromBody] ReservationDtoCreate dto)
    {
        var result = await _reservationService.CreateReservation(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Cancel(int id)
    {
        await _reservationService.CancelReservation(id);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDtoGet>>> GetAll()
    {
        var result = await _reservationService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDtoGet>> GetById(int id)
    {
        var result = await _reservationService.GetByIdAsync(id);
        return Ok(result);
    }
}