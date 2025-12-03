using Domain.Enums;

namespace Domain.Exceptions;

public class ReservationExpiredException(string reservationId, DateTime expiredAt)
    : DomainException($"The reservation with ID '{reservationId}' has expired at {expiredAt}.");
    
public class InvalidReservationStatusException(string reservationId, ReservationStatus currentStatus, ReservationStatus expectedStatus)
    : DomainException($"Cannot perform operation on reservation '{reservationId}'. Current status: {currentStatus}, Required: {expectedStatus}.");
    
public class CannotCancelConfirmedReservationException(string reservationId)
    : DomainException($"Cannot cancel reservation '{reservationId}' because it is already confirmed.");