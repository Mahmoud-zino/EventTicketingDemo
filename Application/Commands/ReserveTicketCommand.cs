using MediatR;

namespace Application.Commands;

public record ReserveTicketCommand: IRequest<string>
{
    public required string EventId { get; init; }
    public required string TicketId { get; init; }
    public required string UserId { get; init; }
    public required int Quantity { get; init; }
}