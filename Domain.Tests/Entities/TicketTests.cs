using AwesomeAssertions;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

[TestFixture]
public class TicketTests
{
    [Test]
    public void Reserve_WithSufficientTickets_ShouldDecreaseAvailableQuantity()
    {
        // Arrange
        var @event = CreateTestEvent();
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

        // Act
        ticket.Reserve(5);

        // Assert
        ticket.AvailableQuantity.Should().Be(45);
        ticket.ReservedQuantity.Should().Be(5);
    }

    [Test]
    public void Reserve_WithInsufficientTickets_ShouldThrowInsufficientTicketsException()
    {
        // Arrange
        var @event = CreateTestEvent();
        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 3,
            ReservedQuantity = 47,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var act = () => ticket.Reserve(5);

        // Assert
        act.Should().Throw<InsufficientTicketsException>()
            .Which.Message.Should().Be($"Cannot reserve 5 ticket(s). Only 3 available.");
    }

    [Test]
    public void Reserve_WithZeroQuantity_ShouldThrowInvalidTicketQuantityException()
    {
        // Arrange
        var @event = CreateTestEvent();
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

        // Act
        var act = () => ticket.Reserve(0);

        // Assert
        act.Should().Throw<InvalidTicketQuantityException>();
    }

    [Test]
    public void Reserve_WithNegativeQuantity_ShouldThrowInvalidTicketQuantityException()
    {
        // Arrange
        var @event = CreateTestEvent();
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

        // Act
        var act = () => ticket.Reserve(-5);

        // Assert
        act.Should().Throw<InvalidTicketQuantityException>();
    }

    [Test]
    public void ReleaseReservation_ShouldIncreaseAvailableQuantity()
    {
        // Arrange
        var @event = CreateTestEvent();
        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 45,
            ReservedQuantity = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        ticket.ReleaseReservation(5, "Cancelled");

        // Assert
        ticket.AvailableQuantity.Should().Be(50);
        ticket.ReservedQuantity.Should().Be(0);
    }

    [Test]
    public void Reserve_ShouldRaiseDomainEvent()
    {
        // Arrange
        var @event = CreateTestEvent();
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

        // Act
        ticket.Reserve(5);

        // Assert
        ticket.DomainEvents.Should().HaveCount(1);
        ticket.DomainEvents.First().Should().BeOfType<TicketsReservedEvent>();
    }

    [Test]
    public void CanReserve_WithSufficientTickets_ShouldReturnTrue()
    {
        // Arrange
        var @event = CreateTestEvent();
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

        // Act
        var result = ticket.CanReserve(5);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void CanReserve_WithInsufficientTickets_ShouldReturnFalse()
    {
        // Arrange
        var @event = CreateTestEvent();
        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 50,
            AvailableQuantity = 3,
            ReservedQuantity = 47,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var result = ticket.CanReserve(5);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Reserve_WhenSoldOut_ShouldRaiseTicketsSoldOutEvent()
    {
        // Arrange
        var @event = CreateTestEvent();
        var ticket = new Ticket
        {
            Id = "ticket-1",
            Event = @event,
            Price = 100m,
            TotalQuantity = 5,
            AvailableQuantity = 5,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        ticket.Reserve(5);

        // Assert
        ticket.DomainEvents.Should().HaveCount(2);
        ticket.DomainEvents.Should().ContainItemsAssignableTo<TicketsReservedEvent>();
        ticket.DomainEvents.Should().ContainItemsAssignableTo<TicketSoldOutEvent>();
    }
    
    private static Event CreateTestEvent()
    {
        return new Event
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
    }
}