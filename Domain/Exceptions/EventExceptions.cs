using Domain.Enums;

namespace Domain.Exceptions;

public class EventNotFoundException(string eventId)
    : DomainException($"Event with ID '{eventId}' was not found.");

public class EventNotPublishedException(string eventId, EventStatus eventStatus)
    : DomainException($"Event '{eventId}' cannot be published because its status is '{eventStatus}'.");
    
public class EventAlreadyCancelledException(string eventId)
    : DomainException($"Event '{eventId}' is already cancelled and cannot be cancelled again.");
    
public class EventSalesNotStartedException(string eventId, DateTime salesStartDate)
    : DomainException($"Event '{eventId}' ticket sales have not started. Sales start date is '{salesStartDate}'.");
    
public class EventSalesEndedException(string eventId, DateTime salesEndDate)
    : DomainException($"Event '{eventId}' ticket sales have ended. Sales end date was '{salesEndDate}'.");