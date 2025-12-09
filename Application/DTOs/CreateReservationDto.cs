namespace Application.DTOs;

public record CreateReservationDto
{
    public required string EventId { get; init; }
    public required string TicketId { get; init; }
    public required string UserId { get; init; }
    public required int Quantity { get; init; }
}