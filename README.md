# Event Ticketing REST API

A production-ready REST API demonstrating Clean Architecture and CQRS patterns through a real-world event ticketing system. Built to showcase enterprise-level software design principles and modern .NET development practices.

## Project Overview

This API handles concurrent ticket reservations with optimistic concurrency control, implements time-limited booking windows, and maintains clear separation between write and read operations. 
The system prevents double-booking through version-based locking and provides comprehensive error handling with domain-specific exceptions.

**Built to demonstrate:**
- Clean Architecture with clear separation of concerns
- CQRS pattern with command/query separation
- Domain-Driven Design with rich domain models
- Optimistic concurrency handling
- Proper exception handling and API error responses
- Repository pattern with persistence abstraction
- RESTful API design with Swagger documentation
- Professional testing practices with NUnit, NSubstitute, and AwesomeAssertions

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
- Mediator
- Clean Architecture
- Repository Pattern
- Domain Events

**Testing:**
- NUnit
- NSubstitute
- AwesomeAssertions

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

5. **Run the tests**
```bash
dotnet test
```

6. **Run the API**
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
---

## 🎯 Key Features Demonstrated

### 1. **Clean Architecture**
- **Domain Layer**: Pure business logic, no external dependencies
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

### 6. **Optimistic Concurrency Control**
```csharp
// Version-based locking prevents lost updates
var result = await collection.ReplaceOneAsync(
    e => e.Id == event.Id && e.Version == currentVersion,
    event
);
if (result.MatchedCount == 0)
    throw new ConcurrencyException();
```

### 7. **Domain Events**
```csharp
// Events raised by domain entities
reservation.Confirm(confirmationCode);
// → Raises ReservationConfirmedEvent
// → Can trigger email notifications, analytics, etc.
```

### 8. **Comprehensive Error Handling**
- Domain-specific exceptions with context
- Global exception middleware
- RFC 7807 Problem Details responses
- Proper HTTP status codes

### 9. **Time-Limited Reservations**
- 15-minute confirmation window
- Automatic inventory release on expiration
- State machine for reservation status

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

## 🧪 Testing

This project includes comprehensive unit tests demonstrating professional testing practices used in production environments. 
While not aiming for 100% coverage, the tests focus on critical business logic and proper testing principles.

### Testing Stack
- **NUnit** - Testing framework
- **NSubstitute** - Mocking library for dependencies
- **AwesomeAssertions** - Readable and expressive assertions

### Testing Principles Demonstrated

1. **Arrange-Act-Assert Pattern** - Clear test structure for readability
2. **Test Isolation** - Each test is independent and can run in any order
3. **Mocking External Dependencies** - Using NSubstitute for repository mocking
4. **Descriptive Test Names** - Tests clearly describe what they verify
5. **Edge Case Coverage** - Testing both happy paths and error scenarios
6. **Domain Logic Testing** - Verifying business rules are enforced
7. **Exception Testing** - Ensuring proper error handling
8. **Domain Events Verification** - Checking event-driven architecture works correctly

### Running Tests
```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~TicketTests"

# Run tests with coverage (if coverlet is installed)
dotnet test /p:CollectCoverage=true
```