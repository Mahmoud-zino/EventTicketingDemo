using Application.Commands;
using Application.DTOs;
using Application.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(IMediator mediator): ControllerBase
{
    /// <summary>
    /// Get all available events
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EventSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EventSummaryDto>>> GetAvailableEvents(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableEventsQuery
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var events = await mediator.Send(query, cancellationToken);
        return Ok(events);
    }

    /// <summary>
    /// Get event by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EventDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDetailsDto>> GetEventById(string id, CancellationToken cancellationToken = default)
    {
        var query = new GetEventQuery { EventId = id };
        var eventDetails = await mediator.Send(query, cancellationToken);
        return Ok(eventDetails);
    }

    /// <summary>
    /// Create a new event
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResponse>> CreateEvent(
        [FromBody] CreateEventDto dto,
        CancellationToken cancellationToken)
    {
        var command = new CreateEventCommand
        {
            Name = dto.Name,
            Description = dto.Description,
            Venue = dto.Venue,
            EventDate = dto.EventDate,
            SalesStartDate = dto.SalesStartDate,
            SalesEndDate = dto.SalesEndDate,
            OrganizerId = dto.OrganizerId,
            Tickets = dto.Tickets
        };

        var eventId = await mediator.Send(command, cancellationToken);

        var response = new CreatedResponse
        {
            Id = eventId,
            Message = "Event created successfully"
        };

        return CreatedAtAction(nameof(GetEventById), new { id = eventId }, response);
    }
}