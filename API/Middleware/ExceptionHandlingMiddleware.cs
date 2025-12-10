using System.Data;
using System.Net;
using System.Text.Json;
using Domain.Exceptions;

namespace API.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private static readonly JsonSerializerOptions SerializationOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            EventNotFoundException ex => (HttpStatusCode.NotFound, "Event Not Found", ex.Message),
            TicketNotFoundException ex => (HttpStatusCode.NotFound, "Ticket Not Found", ex.Message),
            ReservationNotFoundException ex => (HttpStatusCode.NotFound, "Reservation Not Found", ex.Message),
            InsufficientTicketsException ex => (HttpStatusCode.Conflict, "Insufficient Tickets", ex.Message),
            EventNotPublishedException ex => (HttpStatusCode.Conflict, "Event Not Published", ex.Message),
            EventSalesNotStartedException ex => (HttpStatusCode.Conflict, "Event Sales Not Started", ex.Message),
            EventSalesEndedException ex => (HttpStatusCode.Conflict, "Event Sales Ended", ex.Message),
            DBConcurrencyException ex => (HttpStatusCode.Conflict, "Database Concurrency Error", ex.Message),
            CannotCancelConfirmedReservationException ex => (HttpStatusCode.Conflict,
                "Cannot Cancel Confirmed Reservation", ex.Message),
            ReservationExpiredException ex => (HttpStatusCode.Gone, "Reservation Expired", ex.Message),
            InvalidReservationStatusException ex => (HttpStatusCode.BadRequest, "Invalid Reservation State",
                ex.Message),
            InvalidTicketQuantityException ex => (HttpStatusCode.BadRequest, "Invalid Ticket Price", ex.Message),
            DomainException ex => (HttpStatusCode.BadRequest, "Business Rule Violation", ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;
        
        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail,
            instance = context.Request.Path.ToString()
        };

        var json = JsonSerializer.Serialize(problemDetails, SerializationOptions);
        
        return  context.Response.WriteAsync(json);
    }
}