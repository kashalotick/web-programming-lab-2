using Microsoft.AspNetCore.Mvc;
using web_programming_lab_2.Entities.Guests;
using web_programming_lab_2.Entities.Reservations;
using web_programming_lab_2.Services;

namespace web_programming_lab_2.Controllers;

[ApiController]
[Route("api/guests")]
public class GuestsController : ControllerBase
{
    private readonly GuestService _guestsService;

    public GuestsController(GuestService guestsService)
    {
        _guestsService = guestsService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GuestDtoGet>>> GetAll()
    {
        var result = await _guestsService.GetAllAsync();
        return Ok(result);
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GuestDtoGet>> GetById(int id)
    {
        var result = await _guestsService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}/reservations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ReservationDtoGet>>> GetReservations(int id)
    {
        var result = await _guestsService.GetReservationsAsync(id);
        return Ok(result);
    }
}