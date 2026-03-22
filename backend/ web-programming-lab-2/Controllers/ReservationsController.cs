using Microsoft.AspNetCore.Mvc;
using web_programming_lab_2.Entities;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.HeaderAttributes;
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
    
    [HttpPost("reserve-room")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservationDtoGet>> ReserveRoom([FromBody] ReservationCreateRequest dto)
    {
        dto.Reservation.GrandTotal = null;
        var result = await _reservationService.ReserveRoom(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }


    
    [AdminHeader]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservationDtoGet>> Create([FromBody] ReservationCreateRequest dto)
    {
        var result = await _reservationService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    [AdminHeader]
    [HttpPut("{id}")]    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReservationDtoGet>> Update(int id, [FromBody] ReservationDtoUpdate dto)
    {
        var result = await _reservationService.Update(id, dto);
        return Ok(result);
    }

    [AdminHeader]
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Cancel(int id)
    {
        await _reservationService.CancelReservation(id);
        return NoContent();
    }
    [AdminHeader]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        await _reservationService.RemoveReservation(id);
        return NoContent();
    }

    
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDtoGet>> GetById(int id)
    {
        var result = await _reservationService.GetByIdAsync(id);
        return Ok(result);
    }
    [HttpGet("{id}/full")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDtoGet>> GetFullById(int id)
    {
        var result = await _reservationService.GetFullByIdAsync(id);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReservationDtoGet>>> GetAll([FromQuery] TimePeriodFilter rangeFilter, [FromQuery] ReservationFilter filter)
    {
        var result = await _reservationService.GetAllAsync(rangeFilter, filter);
        return Ok(result);
    }


}