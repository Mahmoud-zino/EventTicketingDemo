namespace Domain.Exceptions;

public class TicketNotFoundException(string ticketId)
    : DomainException($"Ticket with ID '{ticketId}' was not found.");

public class InsufficientTicketsException(int requestedQuantity, int availableQuantity) : DomainException(
    $"Cannot reserve {requestedQuantity} ticket(s). Only {availableQuantity} available.");
    
public class InvalidTicketQuantityException(int quantity) : DomainException(
    $"Invalid ticket quantity: {quantity}. Quantity must be greater than 0.");