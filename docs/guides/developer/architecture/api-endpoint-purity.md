# API Endpoint Purity Guide

This guide defines the architectural rules for keeping API endpoints thin, predictable, and free of business logic.  
It belongs to the **architecture** category because it governs cross‑cutting behavior across all vertical slices.

---

# Purpose

API endpoints exist to:

- Translate HTTP requests into application commands/queries  
- Invoke the dispatcher pipeline  
- Translate results into HTTP responses  

Endpoints must remain **pure orchestrators**, not business logic containers.

---

# Goals

- Keep HTTP concerns in the API layer  
- Keep business logic in Application and Domain  
- Ensure all business logic flows through the dispatcher pipeline  
- Make endpoints easy to read, test, and maintain  
- Prevent duplication of business rules  
- Enforce strict layering and dependency direction  
- Ensure compliance with Frank security headers, CORS, and error boundary middleware  
- Ensure endpoints participate correctly in authentication and authorization governance  

---

# Responsibilities of API Endpoints

API endpoints **should**:

- Parse and validate HTTP input *shape* (not business rules)  
- Map HTTP requests to commands or queries  
- Call the dispatcher  
  - `commandDispatcher.DispatchAsync(command, cancellationToken)`  
  - `queryDispatcher.DispatchAsync(query, cancellationToken)`  
- Map results to response DTOs  
- Return appropriate HTTP status codes  
- Use Frank endpoint discovery  
- Rely on Frank’s security headers and CORS middleware  
- Allow Frank’s error boundary to shape errors  

API endpoints **must not**:

- Contain business logic  
- Call repositories directly  
- Instantiate or manipulate domain entities  
- Bypass the dispatcher pipeline  
- Invoke handlers directly  
- Reference Application handlers or validators  
- Reference Infrastructure types  
- Depend on Infrastructure assemblies  
- Perform domain mutations  
- Perform identity resolution  
- Perform authorization checks manually  
- Perform configuration access  
- Perform environment access  
- Perform HTTP/JSON/ZIP operations  
- Interact with hosting providers  

---

# Typical Endpoint Flow

A clean endpoint follows this pattern:

1. Receive HTTP request  
2. Map request → command/query  
3. Dispatch  
4. Map result → response DTO  
5. Return HTTP response  

Example:

````csharp
var command = new RegisterDogCommand(request.Name, request.Breed);

var result = await commandDispatcher.DispatchAsync(command, cancellationToken);

return Results.Created($"/dogs/{result.DogId}", result);
````

Notice what’s missing:

- No domain logic  
- No repository calls  
- No handler instantiation  
- No validation logic  
- No event dispatching  
- No Infrastructure references  
- No identity resolution  
- No authorization logic  
- No configuration access  

---

# Purity Rules

## 1. Endpoints depend only on Abstractions

Endpoints may reference:

- Commands  
- Queries  
- Result DTOs  
- Dispatcher interfaces  
- ASP.NET primitives  
- Frank endpoint discovery attributes  

Endpoints must **not** reference:

- Application handlers  
- Validators  
- Domain entities  
- Infrastructure repositories  
- EF Core DbContexts  
- Infrastructure services  
- Hosting providers  
- Environment abstractions  
- Artifact clients  

This enforces **Architecture Governance**.

---

## 2. Endpoints must use dispatchers

Endpoints must call:

- `ICommandDispatcher` for commands  
- `IQueryDispatcher` for queries  

Direct handler invocation is forbidden.

This enforces the **Dispatcher Pipeline**.

---

## 3. Endpoints must not contain business logic

Business logic belongs in:

- Domain (invariants, rules)  
- Application handlers (use cases)  

Endpoints only orchestrate HTTP.

This enforces **Code Conventions**.

---

## 4. Endpoints must not raise domain events

Domain events are:

- Raised inside aggregates  
- Collected by Application  
- Dispatched by the domain event dispatcher  

Endpoints must not interact with domain events.

This enforces **Domain Events Architecture**.

---

## 5. Endpoints must not perform identity resolution

Endpoints must not:

- Accept identity from request bodies  
- Parse identity from headers  
- Trust client‑provided IDs  

Identity must be resolved via:

- `ICurrentUserService` (Application abstraction)  

This enforces **Security Governance**.

---

## 6. Endpoints must not perform authorization logic

Endpoints must not:

- Check roles manually  
- Check permissions manually  
- Trust frontend authorization  

Authorization must be enforced by:

- Frank authorization seams  
- Application abstractions  

This enforces **Authorization Governance**.

---

## 7. Endpoints must rely on Frank middleware

Endpoints must not:

- Implement their own error handling  
- Implement their own security headers  
- Implement their own CORS logic  

Endpoints must rely on:

- Frank error boundary  
- Frank security headers middleware  
- Frank CORS policy enforcement  

This enforces **Security Headers Governance** and **CORS Governance**.

---

# DTO Purity

DTOs used by endpoints must:

- Contain only data  
- Not reference domain entities  
- Not reference Application internals  
- Not reference Infrastructure types  
- Not contain behavior  

DTOs should be simple records or classes.

This enforces **API Governance**.

---

# Mapping Rules

Endpoints may:

- Map request DTO → command/query  
- Map result → response DTO  

Endpoints must not:

- Map domain entities directly to HTTP responses  
- Perform business transformations  
- Perform validation beyond shape validation  

Mapping must remain mechanical and predictable.

---

# Dependency Direction Rules

Endpoints must follow strict dependency direction:

```
Api → Application → Domain
Infrastructure → Application → Domain
All layers → Frank
```

Endpoints must not depend on:

- Infrastructure  
- EF Core  
- Domain entities  
- Application handlers  

This enforces **Architecture Governance**.

---

# DI & Frank Rules

Endpoints participate in DI only through:

- Dispatcher abstractions  
- Request DTO validators (auto‑registered)  
- Endpoint discovery (Frank)  

Endpoints must not:

- Register services manually  
- Use Scrutor or suffix scanning  
- Use Infrastructure services  
- Use hosting provider abstractions  

This enforces **Dependency Injection Architecture**.

---

# Contributor Guidelines

When adding a new endpoint:

1. Define the command/query and result in Abstractions  
2. Implement the handler in Application  
3. Add validators if needed  
4. Use the dispatcher from the endpoint  
5. Keep endpoint logic limited to mapping and HTTP concerns  
6. Do not bypass the dispatcher pipeline  
7. Do not reference Application internals or Infrastructure  
8. Ensure endpoint is discovered via Frank endpoint discovery  
9. Ensure endpoint follows API security rules  
10. Ensure endpoint follows error‑shaping conventions  
11. Ensure endpoint relies on Frank security headers and CORS  
12. Ensure endpoint does not perform identity or authorization logic  

If an endpoint grows beyond ~10–20 lines, logic is leaking into the wrong layer.

---

# Related Documents

- Authentication Architecture  
- Identity Mapping  
- Session Management  
- Dispatcher Pipeline  
- Domain Events Architecture  
- Architecture Governance  
- Security Governance  
- API Governance  
- CI Governance  
- Operations Governance
