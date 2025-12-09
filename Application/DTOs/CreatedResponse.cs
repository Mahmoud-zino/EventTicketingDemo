namespace Application.DTOs;

public record CreatedResponse
{
    public required string Id { get; init; }
    public required string Message { get; init; }
}