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
public class ConfirmReservationCommandHandlerTest
{
    private IReservationRepository _reservationRepository;
    private ConfirmReservationCommandHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _reservationRepository = Substitute.For<IReservationRepository>();
        _handler = new ConfirmReservationCommandHandler(_reservationRepository);
    }

    [Test]
    public async Task Handle_WithValidPendingReservation_ShouldConfirm()
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
            AvailableQuantity = 48,
            ReservedQuantity = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

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

        _reservationRepository.GetByIdAsync("reservation-1", Arg.Any<CancellationToken>())
            .Returns(reservation);

        var command = new ConfirmReservationCommand
        {
            ReservationId = "reservation-1",
            PaymentId = "pi_123456"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.Should().NotBeNull();
        
        await _reservationRepository.Received(1).UpdateAsync(reservation, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_WithNonExistentReservation_ShouldThrowReservationNotFoundException()
    {
        // Arrange
        _reservationRepository.GetByIdAsync("reservation-1", Arg.Any<CancellationToken>())
            .Returns((Reservation?)null);

        var command = new ConfirmReservationCommand
        {
            ReservationId = "reservation-1",
            PaymentId = "pi_123456"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ReservationNotFoundException>();
    }

    [Test]
    public async Task Handle_WithExpiredReservation_ShouldThrowCannotConfirmExpiredReservationException()
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
            AvailableQuantity = 48,
            ReservedQuantity = 2,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 0
        };

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

        _reservationRepository.GetByIdAsync("reservation-1", Arg.Any<CancellationToken>())
            .Returns(reservation);

        var command = new ConfirmReservationCommand
        {
            ReservationId = "reservation-1",
            PaymentId = "pi_123456"
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ReservationExpiredException>();
    }
}