using AwesomeAssertions;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

[TestFixture]
public class EventTests
{
    [Test]
    public void IsSoldOut_WhenAllTicketsSoldOut_ShouldReturnTrue()
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
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket1 = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 0,
            ReservedQuantity = 50,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket2 = new Ticket
        {
            Id = "ticket-2",
            Event = @event,
            Price = 50m,
            TotalQuantity = 100,
            AvailableQuantity = 0,
            ReservedQuantity = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = [ticket1, ticket2];

        // Act
        var result = @event.IsSoldOut();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsSoldOut_WhenTicketsAvailable_ShouldReturnFalse()
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
            AvailableQuantity = 10,
            ReservedQuantity = 40,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = [ticket];

        // Act
        var result = @event.IsSoldOut();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void GetTotalAvailableTickets_ShouldSumAllTickets()
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
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket1 = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 30,
            ReservedQuantity = 20,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        var ticket2 = new Ticket
        {
            Id = "ticket-2",
            Event = @event,
            Price = 50m,
            TotalQuantity = 100,
            AvailableQuantity = 75,
            ReservedQuantity = 25,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        @event.Tickets = [ticket1, ticket2];

        // Act
        var total = @event.GetTotalAvailableTickets();

        // Assert
        total.Should().Be(105);
    }

    [Test]
    public void ValidateCanPurchaseTickets_WithPublishedEventDuringSales_ShouldNotThrow()
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

        // Act
        var act = () => @event.ValidateCanPurchaseTickets();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void ValidateCanPurchaseTickets_WithDraftStatus_ShouldThrowEventNotPublishedException()
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

        // Act
        var act = () => @event.ValidateCanPurchaseTickets();

        // Assert
        act.Should().Throw<EventNotPublishedException>();
    }

    [Test]
    public void ValidateCanPurchaseTickets_BeforeSalesStart_ShouldThrowEventSalesNotStartedException()
    {
        // Arrange
        var @event = new Event
        {
            Id = "event-1",
            Name = "Test Event",
            Description = "Test Description",
            Venue = "Test Venue",
            EventDate = DateTime.UtcNow.AddDays(30),
            SalesStartDate = DateTime.UtcNow.AddDays(1),
            SalesEndDate = DateTime.UtcNow.AddDays(29),
            OrganizerId = "organizer-1",
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var act = () => @event.ValidateCanPurchaseTickets();

        // Assert
        act.Should().Throw<EventSalesNotStartedException>();
    }

    [Test]
    public void ValidateCanPurchaseTickets_AfterSalesEnd_ShouldThrowEventSalesEndedException()
    {
        // Arrange
        var @event = new Event
        {
            Id = "event-1",
            Name = "Test Event",
            Description = "Test Description",
            Venue = "Test Venue",
            EventDate = DateTime.UtcNow.AddDays(30),
            SalesStartDate = DateTime.UtcNow.AddDays(-10),
            SalesEndDate = DateTime.UtcNow.AddDays(-1),
            OrganizerId = "organizer-1",
            Status = EventStatus.Published,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var act = () => @event.ValidateCanPurchaseTickets();

        // Assert
        act.Should().Throw<EventSalesEndedException>();
    }
}