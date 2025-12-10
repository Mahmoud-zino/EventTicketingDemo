using Application.CommandHandlers;
using Application.Commands;
using Application.Interfaces;
using AwesomeAssertions;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using NSubstitute;

namespace Application.Tests.CommandHandlers;

[TestFixture]
public class ReserveTicketsCommandHandlerTests
{
    private IEventRepository _eventRepository;
    private IReservationRepository _reservationRepository;
    private ReserveTicketsCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _eventRepository = Substitute.For<IEventRepository>();
        _reservationRepository = Substitute.For<IReservationRepository>();
        _handler = new ReserveTicketsCommandHandler(_eventRepository, _reservationRepository);
    }

    [Test]
    public async Task Handle_WithValidRequest_ShouldCreateReservation()
    {
        // Arrange
        var @event = new Event
        {
            Id = "event-1",
            Name = "Test Event",
            Description = "Test Description",
            Venue = "Test Venue",
            EventDate = DateTime.UtcNow.AddDays(30),
            SalesStartDate = DateTime.UtcNow.AddDays(-1),
            SalesEndDate = DateTime.UtcNow.AddDays(29),
            OrganizerId = "organizer-1",
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 50,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = [ticket];

        _eventRepository.GetByIdAsync("event-1", Arg.Any<CancellationToken>())
            .Returns(@event);
        
        _reservationRepository.AddAsync(Arg.Any<Reservation>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Reservation>().Id);

        var command = new ReserveTicketCommand
        {
            EventId = "event-1",
            TicketId = "ticket-1",
            UserId = "user@test.com",
            Quantity = 2
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();
        
        await _eventRepository.Received(1).UpdateAsync(@event, Arg.Any<CancellationToken>());
        await _reservationRepository.Received(1).AddAsync(
            Arg.Is<Reservation>(r => 
                r.Event == @event &&
                r.Ticket == ticket &&
                r.UserId == "user@test.com" &&
                r.Quantity == 2
            ), 
            Arg.Any<CancellationToken>()
        );
        
        ticket.AvailableQuantity.Should().Be(48);
        ticket.ReservedQuantity.Should().Be(2);
    }

    [Test]
    public async Task Handle_WithNonExistentEvent_ShouldThrowEventNotFoundException()
    {
        // Arrange
        _eventRepository.GetByIdAsync("event-1", Arg.Any<CancellationToken>())
            .Returns((Event?)null);

        var command = new ReserveTicketCommand
        {
            EventId = "event-1",
            TicketId = "ticket-1",
            UserId = "user@test.com",
            Quantity = 2
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EventNotFoundException>();
    }

    [Test]
    public async Task Handle_WithNonExistentTicket_ShouldThrowTicketNotFoundException()
    {
        // Arrange
        var @event = new Event
        {
            Id = "event-1",
            Name = "Test Event",
            Description = "Test Description",
            Venue = "Test Venue",
            EventDate = DateTime.UtcNow.AddDays(30),
            SalesStartDate = DateTime.UtcNow.AddDays(-1),
            SalesEndDate = DateTime.UtcNow.AddDays(29),
            OrganizerId = "organizer-1",
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0,
            Tickets = new List<Ticket>()
        };

        _eventRepository.GetByIdAsync("event-1", Arg.Any<CancellationToken>())
            .Returns(@event);

        var command = new ReserveTicketCommand
        {
            EventId = "event-1",
            TicketId = "ticket-nonexistent",
            UserId = "user@test.com",
            Quantity = 2
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TicketNotFoundException>();
    }

    [Test]
    public async Task Handle_WithInsufficientTickets_ShouldThrowInsufficientTicketsException()
    {
        // Arrange
        var @event = new Event
        {
            Id = "event-1",
            Name = "Test Event",
            Description = "Test Description",
            Venue = "Test Venue",
            EventDate = DateTime.UtcNow.AddDays(30),
            SalesStartDate = DateTime.UtcNow.AddDays(-1),
            SalesEndDate = DateTime.UtcNow.AddDays(29),
            OrganizerId = "organizer-1",
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 1,
            ReservedQuantity = 49,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = [ticket];

        _eventRepository.GetByIdAsync("event-1", Arg.Any<CancellationToken>())
            .Returns(@event);

        var command = new ReserveTicketCommand
        {
            EventId = "event-1",
            TicketId = "ticket-1",
            UserId = "user@test.com",
            Quantity = 5
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InsufficientTicketsException>();
    }

    [Test]
    public async Task Handle_WithDraftEvent_ShouldThrowEventNotPublishedException()
    {
        // Arrange
        var @event = new Event
        {
            Id = "event-1",
            Name = "Test Event",
            Description = "Test Description",
            Venue = "Test Venue",
            EventDate = DateTime.UtcNow.AddDays(30),
            SalesStartDate = DateTime.UtcNow,
            SalesEndDate = DateTime.UtcNow.AddDays(29),
            OrganizerId = "organizer-1",
            Status = EventStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 50,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = [ticket];

        _eventRepository.GetByIdAsync("event-1", Arg.Any<CancellationToken>())
            .Returns(@event);

        var command = new ReserveTicketCommand
        {
            EventId = "event-1",
            TicketId = "ticket-1",
            UserId = "user@test.com",
            Quantity = 2
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EventNotPublishedException>();
    }
}