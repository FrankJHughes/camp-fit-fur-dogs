# API Endpoint Purity

This guide describes the rules for keeping API endpoints thin, focused, and free of business logic. Endpoints should translate HTTP requests into application commands/queries and return HTTP responses — nothing more.

---

## 1. Goals

- Keep HTTP concerns in the API layer.
- Keep business logic in Application and Domain.
- Make endpoints easy to read and test.
- Avoid duplication of business rules.
- Ensure all business logic flows through the dispatcher pipeline.

---

## 2. Responsibilities of API Endpoints

API endpoints **should**:

- Parse and validate HTTP input shape (not business rules).
- Map HTTP requests to commands or queries (from Abstractions).
- Call `ICommandDispatcher` or `IQueryDispatcher`.
- Map results to HTTP responses.
- Set appropriate status codes.
- Remain extremely small and predictable.

API endpoints **must not**:

- Contain business logic.
- Call repositories directly.
- Instantiate or manipulate domain entities.
- Bypass the dispatcher pipeline.
- Reference Application handlers or validators directly.
- Reference Infrastructure types.

---

## 3. Typical Endpoint Flow

A clean endpoint follows this pattern:

1. Receive HTTP request.
2. Map request body/route/query to a command or query.
3. Call dispatcher:
   - `commandDispatcher.DispatchAsync(command, cancellationToken)`
   - `queryDispatcher.DispatchAsync(query, cancellationToken)`
4. Map result to response DTO.
5. Return appropriate HTTP status code.

Example (conceptual):

```csharp
var command = new RegisterDogCommand(request.Name, request.Breed);

var result = await commandDispatcher.DispatchAsync(command, cancellationToken);

return Results.Created($"/dogs/{result.DogId}", result);
```

Notice what’s missing:

- No domain logic.
- No repository calls.
- No handler instantiation.
- No validation logic (handled by pipeline).
- No event dispatching (handled by pipeline).

---

## 4. Purity Rules

### 4.1 Endpoints depend only on Abstractions  
Endpoints may reference:

- Commands
- Queries
- Result DTOs
- Dispatcher interfaces
- ASP.NET primitives

Endpoints must **not** reference:

- Application handlers
- Validators
- Domain entities
- Infrastructure repositories
- EF Core DbContexts

### 4.2 Endpoints must use dispatchers  
Endpoints must call:

- `ICommandDispatcher` for commands  
- `IQueryDispatcher` for queries  

Direct handler invocation is forbidden.

### 4.3 Endpoints must not contain business logic  
Business logic belongs in:

- Domain (invariants, rules)
- Application handlers (use cases)

Endpoints only orchestrate HTTP.

### 4.4 Endpoints must not raise domain events  
Domain events are raised inside domain entities and dispatched by the Application pipeline.

---

## 5. DTO Purity

DTOs used by endpoints must:

- Contain only data (no behavior).
- Not reference domain entities.
- Not reference Application internals.
- Not reference Infrastructure types.

DTOs should be simple records or classes.

---

## 6. Mapping Rules

Endpoints may:

- Map request DTO → command/query
- Map result → response DTO

Endpoints must not:

- Map domain entities directly to HTTP responses
- Perform business transformations

Mapping must remain mechanical and predictable.

---

## 7. Contributor Guidelines

When adding a new endpoint:

1. Define the command/query and result in Abstractions.
2. Implement the handler in Application.
3. Add validators if needed.
4. Use the dispatcher from the endpoint.
5. Keep endpoint logic limited to mapping and HTTP concerns.
6. Do not bypass the dispatcher pipeline.
7. Do not reference Application internals or Infrastructure.

If an endpoint starts growing beyond ~10–20 lines, it’s a signal that logic is leaking into the wrong layer.

