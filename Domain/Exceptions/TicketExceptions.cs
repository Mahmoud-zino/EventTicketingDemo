namespace Domain.Exceptions;

public class InsufficientTicketsException(int requestedQuantity, int availableQuantity) : DomainException(
    $"Cannot reserve {requestedQuantity} ticket(s). Only {availableQuantity} available.");
    
public class InvalidTicketQuantityException(int quantity) : DomainException(
    $"Invalid ticket quantity: {quantity}. Quantity must be greater than 0.");