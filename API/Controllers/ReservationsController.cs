using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IMediator mediator): ControllerBase
{
    /// <summary>
    /// Get user's reservations
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(List<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReservationDto>>> GetMyReservations(
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserReservationQuery { UserId = userId };
        var reservations = await mediator.Send(query, cancellationToken);
        return Ok(reservations);
    }

    /// <summary>
    /// Get reservation by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> GetReservationById(string id, CancellationToken ct)
    {
        var query = new GetReservationByIdQuery { ReservationId = id };
        var reservation = await mediator.Send(query, ct);
        return Ok(reservation);
    }

    /// <summary>
    /// Reserve tickets
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreatedResponse>> ReserveTickets(
        [FromBody] CreateReservationDto dto,
        CancellationToken ct)
    {
        var command = new ReserveTicketCommand
        {
            EventId = dto.EventId,
            TicketId = dto.TicketId,
            UserId = dto.UserId,
            Quantity = dto.Quantity
        };

        var reservationId = await mediator.Send(command, ct);

        var response = new CreatedResponse
        {
            Id = reservationId,
            Message = "Tickets reserved successfully. Please confirm within 15 minutes."
        };

        return CreatedAtAction(nameof(GetReservationById), new { id = reservationId }, response);
    }

    /// <summary>
    /// Confirm a reservation
    /// </summary>
    [HttpPut("{id}/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<ActionResult> ConfirmReservation(
        string id,
        [FromBody] ConfirmReservationDto dto,
        CancellationToken ct)
    {
        var command = new ConfirmReservationCommand
        {
            ReservationId = id,
            PaymentId = dto.PaymentId
        };

        await mediator.Send(command, ct);
        return Ok(new { message = "Reservation confirmed successfully" });
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CancelReservation(string id, CancellationToken ct)
    {
        var command = new CancelReservationCommand { ReservationId = id };
        await mediator.Send(command, ct);
        return Ok(new { message = "Reservation cancelled successfully" });
    }
}