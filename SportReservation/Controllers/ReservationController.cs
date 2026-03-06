using Microsoft.AspNetCore.Mvc;
using SportReservation.Services;

namespace SportReservation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _svc;

    public ReservationsController(ReservationService svc)
    {
        _svc = svc;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        // jednoduch� p��klad; roz�i�te dle pot�eby
        var r = await _svc.GetReservationAsync(id);
        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        // validace DTO atd.
        var res = await _svc.CreateReservationAsync(dto.UserId, dto.FacilityId, dto.StartAt, dto.EndAt);
        return CreatedAtAction(nameof(Get), new { id = res.Id }, res);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] Guid userId, [FromQuery] bool isAdmin = false)
    {
        await _svc.CancelReservationAsync(id, userId, isAdmin);
        return NoContent();
    }
}

public record CreateReservationDto(Guid UserId, Guid FacilityId, DateTime StartAt, DateTime EndAt);