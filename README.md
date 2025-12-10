# Event Ticketing REST API

A production-ready REST API demonstrating Clean Architecture and CQRS patterns through a real-world event ticketing system. Built to showcase enterprise-level software design principles and modern .NET development practices.

## Project Overview

This API handles concurrent ticket reservations with optimistic concurrency control, implements time-limited booking windows, and maintains clear separation between write and read operations. The system prevents double-booking through version-based locking and provides comprehensive error handling with domain-specific exceptions.

**Built to demonstrate:**
- Clean Architecture with clear separation of concerns
- CQRS pattern with command/query separation
- Domain-Driven Design with rich domain models
- Optimistic concurrency handling
- Object-reference-based domain modeling
- Proper exception handling and API error responses
- Repository pattern with persistence abstraction
- RESTful API design with Swagger documentation

---

## Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────┐
│            API Layer                    │
│  (Controllers, Middleware, Program.cs)  │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│        Application Layer                │
│  (Commands, Queries, DTOs, Handlers)    │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│          Domain Layer                   │
│   (Entities, Events, Exceptions)        │
└─────────────────────────────────────────┘
                    ↑
┌─────────────────────────────────────────┐
│      Infrastructure Layer               │
│  (Repositories, MongoDB, Persistence)   │
└─────────────────────────────────────────┘
```

### CQRS Implementation

**Commands (Write Operations):**
- CreateEvent
- ReserveTickets
- ConfirmReservation
- CancelReservation

**Queries (Read Operations):**
- GetEventById
- GetAvailableEvents
- GetUserReservations
- GetReservationById

---

## 🛠️ Technology Stack

**Core:**
- .NET 8
- ASP.NET Core Web API
- C# 12

**Persistence:**
- MongoDB 7.0+
- MongoDB.Driver 2.28.0

**Patterns & Libraries:**
- Mediator -> (MediatR free alternative)
- Clean Architecture
- Repository Pattern
- Domain Events

**Documentation:**
- Swashbuckle (Swagger/OpenAPI)

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) (or Docker)
- IDE: Visual Studio 2022, JetBrains Rider, or VS Code

### MongoDB Setup

**Option 1: Local Installation**
```bash
# Install MongoDB and start the service
# Default connection: mongodb://localhost:27017
```

**Option 2: Docker**
```bash
docker run -d -p 27017:27017 --name mongodb mongo:7.0
```

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/event-ticketing-api.git
cd event-ticketing-api
```

2. **Configure MongoDB connection**

Update `API/appsettings.json`:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "EventTicketingDb"
  }
}
```

3. **Restore dependencies**
```bash
dotnet restore
```

4. **Build the solution**
```bash
dotnet build
```

5. **Run the API**
```bash
cd API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7172`
- HTTP: `http://localhost:5137`
- Swagger UI: `https://localhost:7172/`

---

## 📚 API Endpoints

### Events

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/events` | Get all available events |
| GET | `/api/events/{id}` | Get event details by ID |
| POST | `/api/events` | Create a new event |

### Reservations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/reservations/my?userId={userId}` | Get user's reservations |
| GET | `/api/reservations/{id}` | Get reservation by ID |
| POST | `/api/reservations` | Reserve tickets (15-min hold) |
| PUT | `/api/reservations/{id}/confirm` | Confirm reservation |
| DELETE | `/api/reservations/{id}` | Cancel reservation |

### Example Requests

**Create Event:**
```json
POST /api/events
{
  "name": "Rock Concert 2026",
  "description": "Amazing rock concert featuring top bands",
  "venue": "Vienna Arena",
  "eventDate": "2026-06-15T20:00:00Z",
  "salesStartDate": "2025-12-15T00:00:00Z",
  "salesEndDate": "2026-06-14T23:59:59Z",
  "organizerId": "organizer-123",
  "tickets": [
    {
      "name": "VIP",
      "description": "VIP seating with backstage access",
      "price": 150.00,
      "totalQuantity": 50,
      "seatSection": "A"
    },
    {
      "name": "Regular",
      "description": "Standard seating",
      "price": 50.00,
      "totalQuantity": 200
    }
  ]
}
```

**Reserve Tickets:**
```json
POST /api/reservations
{
  "eventId": "event-id-here",
  "ticketId": "ticket-id-here",
  "userId": "user@example.com",
  "quantity": 2
}
```

**Confirm Reservation:**
```json
PUT /api/reservations/{reservationId}/confirm
{
  "paymentIntentId": "pi_1234567890"
}
```

---

## 🎯 Key Features Demonstrated

### 1. **Clean Architecture**
- **Domain Layer**: business logic, no external dependencies
- **Application Layer**: Use cases and orchestration
- **Infrastructure Layer**: Database access and external concerns
- **API Layer**: HTTP endpoints and presentation

### 2. **CQRS Pattern**
- Separate models for reads and writes
- Commands modify state, Queries retrieve data
- Mediator pattern for request handling
- Clear separation of concerns

### 3. **Domain-Driven Design**
- Rich domain models with behavior
- Domain events for decoupled side effects
- Value objects and entity aggregates

### 4. **Optimistic Concurrency Control**
```csharp
// Version-based locking prevents lost updates
var result = await collection.ReplaceOneAsync(
    e => e.Id == event.Id && e.Version == currentVersion,
    event
);
if (result.MatchedCount == 0)
    throw new ConcurrencyException();
```

### 5. **Domain Events**
```csharp
// Events raised by domain entities
reservation.Confirm(confirmationCode);
// → Raises ReservationConfirmedEvent
// → Can trigger email notifications, analytics, etc.
```

### 6. **Comprehensive Error Handling**
- Domain-specific exceptions with context
- Global exception middleware
- RFC 7807 Problem Details responses
- Proper HTTP status codes

### 7. **Time-Limited Reservations**
- 15-minute confirmation window
- Automatic inventory release on expiration
- State machine for reservation status

---

## 🗄️ Database Design

### Collections

**events:**
```javascript
{
  "_id": "event-123",
  "name": "Rock Concert 2026",
  "description": "...",
  "venue": "Vienna Arena",
  "eventDate": ISODate("2026-06-15T20:00:00Z"),
  "salesStartDate": ISODate("2025-12-15T00:00:00Z"),
  "salesEndDate": ISODate("2026-06-14T23:59:59Z"),
  "organizerId": "organizer-123",
  "status": "Published",
  "tickets": [
    {
      "_id": "ticket-456",
      "eventId": "event-123",
      "name": "VIP",
      "price": 150.00,
      "totalQuantity": 50,
      "availableQuantity": 45,
      "reservedQuantity": 5,
      "version": 3
    }
  ],
  "version": 5,
  "createdAt": ISODate("2025-12-10T10:00:00Z"),
  "updatedAt": ISODate("2025-12-10T15:30:00Z")
}
```

**reservations:**
```javascript
{
  "_id": "reservation-789",
  "eventId": "event-123",
  "ticketId": "ticket-456",
  "userId": "user@example.com",
  "quantity": 2,
  "unitPrice": 150.00,
  "status": "Pending",
  "createdAt": ISODate("2025-12-10T15:30:00Z"),
  "expiresAt": ISODate("2025-12-10T15:45:00Z"),
  "confirmedAt": null,
  "confirmationCode": null,
  "version": 0
}
```

### Indexes

**Events:**
- `{ status: 1, eventDate: 1 }` - Query published events by date
- `{ organizerId: 1 }` - Find events by organizer

**Reservations:**
- `{ userId: 1 }` - User's reservations
- `{ eventId: 1 }` - Event reservations
- `{ status: 1, expiresAt: 1 }` - Cleanup expired reservations

---

## 🧪 Testing with Swagger

1. Navigate to `https://localhost:7172/`
2. Try the following workflow:

**Step 1: Create an Event**
```
POST /api/events
```

**Step 2: View Available Events**
```
GET /api/events
```

**Step 3: Get Event Details**
```
GET /api/events/{eventId}
```

**Step 4: Reserve Tickets**
```
POST /api/reservations
```
Note the `reservationId` from the response.

**Step 5: View Your Reservations**
```
GET /api/reservations/my?userId=your-email@example.com
```

**Step 6: Confirm Reservation (within 15 minutes)**
```
PUT /api/reservations/{reservationId}/confirm
```

**Step 7: Try Cancellation**
```
DELETE /api/reservations/{reservationId}
```

---

## 🎓 Design Decisions

### Why Clean Architecture?
- **Testability**: Business logic independent of frameworks
- **Maintainability**: Clear boundaries and responsibilities
- **Flexibility**: Easy to swap out infrastructure components
- **Scalability**: Well-organized for team collaboration

### Why CQRS?
- **Performance**: Optimize reads and writes separately
- **Scalability**: Scale read and write operations independently
- **Complexity Management**: Separate models for different use cases
- **Clear Intent**: Commands represent user intentions

### Why Mediator Over Direct Injection?
- **Decoupling**: Controllers don't know about handlers
- **Single Responsibility**: One handler per use case
- **Cross-cutting Concerns**: Easy to add logging, validation pipelines
- **Testability**: Mock mediator instead of individual handlers

### Why MongoDB?
- **Flexible Schema**: Event/Ticket aggregate fits naturally
- **Document Model**: Matches domain aggregates
- **Performance**: Fast reads for event listings
- **Scalability**: Horizontal scaling capabilities

---

## 🔒 Concurrency Handling

The system uses **optimistic concurrency control** to handle race conditions:

### Scenario: Two Users Reserve Last Ticket

**User A and User B both try to reserve the last ticket:**

1. Both read `AvailableQuantity = 1`, `Version = 5`
2. User A reserves → `Version = 6`
3. User B tries to reserve → Fails with `ConcurrencyException`
4. User B's update is rejected because version doesn't match

```csharp
var result = await collection.ReplaceOneAsync(
    e => e.Id == event.Id && e.Version == currentVersion,
    event
);
if (result.MatchedCount == 0)
    throw new ConcurrencyException();
```

---

## Notes

### Educational Purpose
This is a portfolio/demonstration project showcasing enterprise software design patterns and clean code principles. While production-ready in architecture, additional features (authentication, authorization, payment processing, etc.) would be needed for a real-world deployment.

---

## 👨‍💻 Author

**Mahmoud** - Backend Developer specializing in C#/.NET and Clean Architecture

- 6+ years of .NET experience
- Focus on scalable microservices and domain-driven design
- Located in Austria

---

## 🙏 Acknowledgments

- Clean Architecture principles by Robert C. Martin
- CQRS pattern by Greg Young
- MongoDB documentation and best practices
- ASP.NET Core team for excellent framework
- Mediator library by Martin Costello