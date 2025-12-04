using MediatR;

namespace Application.Commands;

public record CreateEventCommand: IRequest<string>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Venue { get; init; }
    public required DateTime EventDate { get; init; }
    public required DateTime SalesStartDate { get; init; }
    public required DateTime SalesEndDate { get; init; }
    public required string OrganizerId { get; init; }
}