# Event Ticketing REST API
A production-ready REST API demonstrating clean architecture principles and CQRS pattern through a real-world event ticketing system. The API handles concurrent ticket reservations, prevents double-booking through optimistic concurrency control, and maintains separate write and read models for optimal performance.
Built to showcase:

---

Clean Architecture: Clear separation of concerns with Domain, Application, Infrastructure, and API layers
CQRS Pattern: Distinct command and query paths with MediatR handlers
Domain-Driven Design: Rich domain models with business rule enforcement
Concurrency Handling: Optimistic locking to prevent race conditions in high-traffic scenarios
Proper Error Handling: Custom middleware mapping domain exceptions to appropriate HTTP responses
Test Coverage: Unit tests focusing on business logic and command handlers

---

## Key Features

- Create and manage events with configurable ticket capacity
- Time-limited ticket reservations (15-minute confirmation window)
- Automatic reservation cleanup for expired bookings
- Availability queries optimized for read performance
- Comprehensive API documentation with Swagger/OpenAPI

---

## Technical Stack

- .NET 8, ASP.NET Core Web API
- MongoDB for persistence
- MediatR for CQRS implementation
- FluentValidation for request validation
- NUnit, NSubstitute, FluentAssertions for testing
- Swashbuckle for API documentation

---

## Real-world considerations addressed

- Prevents overbooking through version-based optimistic concurrency
- Handles reservation timeouts to free up inventory
- Validates state transitions (Pending → Confirmed/Cancelled)
- Provides clear error responses with problem details format

---

> This project simulates patterns I've implemented in production microservices previously, adapted to a standalone demonstration context.

