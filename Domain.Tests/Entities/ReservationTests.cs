using AwesomeAssertions;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Tests.Entities;

[TestFixture]
public class ReservationTests
{
    [Test]
    public void Confirm_WithPendingStatus_ShouldConfirmReservation()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Version = 0
        };

        // Act
        reservation.Confirm();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.Should().NotBeNull();
        reservation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ReservationConfirmedEvent>();
    }

    [Test]
    public void Confirm_WithExpiredReservation_ShouldThrowCannotConfirmExpiredReservationException()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Expired 5 minutes ago
            Version = 0
        };

        // Act
        var act = () => reservation.Confirm();

        // Assert
        act.Should().Throw<ReservationExpiredException>();
    }

    [Test]
    public void Confirm_WithConfirmedStatus_ShouldThrowInvalidReservationStatusException()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            ConfirmedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var act = () => reservation.Confirm();

        // Assert
        act.Should().Throw<InvalidReservationStatusException>();
    }

    [Test]
    public void Cancel_WithPendingStatus_ShouldCancelReservation()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Version = 0
        };

        // Act
        reservation.Cancel();

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancelledAt.Should().NotBeNull();
        reservation.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ReservationCancelledEvent>();
    }

    [Test]
    public void Cancel_WithConfirmedStatus_ShouldThrowCannotCancelConfirmedReservationException()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            ConfirmedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var act = () => reservation.Cancel();

        // Assert
        act.Should().Throw<CannotCancelConfirmedReservationException>();
    }

    [Test]
    public void Cancel_ShouldReleaseTickets()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        ticket.Reserve(2); // Reserve 2 tickets first
        
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Version = 0
        };

        var initialAvailable = ticket.AvailableQuantity;

        // Act
        reservation.Cancel();

        // Assert
        ticket.AvailableQuantity.Should().Be(initialAvailable + 2);
    }

    [Test]
    public void IsExpired_WithPendingAndExpiredTime_ShouldReturnTrue()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5),
            Version = 0
        };

        // Act
        var result = reservation.IsExpired();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void IsExpired_WithPendingAndNotExpiredTime_ShouldReturnFalse()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Version = 0
        };

        // Act
        var result = reservation.IsExpired();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void IsExpired_WithConfirmedStatus_ShouldReturnFalse()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 2,
            PricePerTicket = 100m,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-5), // Even if expired time passed
            ConfirmedAt = DateTime.UtcNow,
            Version = 0
        };

        // Act
        var result = reservation.IsExpired();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void TotalPrice_ShouldCalculateCorrectly()
    {
        // Arrange
        var (@event, ticket) = CreateTestEventAndTicket();
        var reservation = new Reservation
        {
            Id = "reservation-1",
            Event = @event,
            Ticket = ticket,
            UserId = "user@test.com",
            Quantity = 3,
            PricePerTicket = 100m,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Version = 0
        };

        // Act
        var totalPrice = reservation.TotalPrice;

        // Assert
        totalPrice.Should().Be(300m);
    }
    
    private static (Event @event, Ticket ticket) CreateTestEventAndTicket()
    {
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
            AvailableQuantity = 50,
            ReservedQuantity = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

        return (@event, ticket);
    }
}