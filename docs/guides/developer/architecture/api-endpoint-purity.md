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
- Use SharedKernel endpoint discovery  

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

---

# Typical Endpoint Flow

A clean endpoint follows this pattern:

1. Receive HTTP request  
2. Map request → command/query  
3. Dispatch  
4. Map result → response DTO  
5. Return HTTP response  

Example:

````  
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

---

# Purity Rules

## 1. Endpoints depend only on Abstractions

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
- Infrastructure services  
- Hosting providers  

This enforces **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

## 2. Endpoints must use dispatchers

Endpoints must call:

- `ICommandDispatcher` for commands  
- `IQueryDispatcher` for queries  

Direct handler invocation is forbidden.

This enforces **[Dispatcher Pipeline](ca://s?q=Open_dispatcher_pipeline_guide)**.

---

## 3. Endpoints must not contain business logic

Business logic belongs in:

- Domain (invariants, rules)  
- Application handlers (use cases)  

Endpoints only orchestrate HTTP.

This enforces **[Code Conventions](ca://s?q=Open_code_conventions)**.

---

## 4. Endpoints must not raise domain events

Domain events are:

- Raised inside aggregates  
- Collected by Application  
- Dispatched by the domain event dispatcher  

Endpoints must not interact with domain events.

This enforces **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**.

---

## 5. Endpoints must not perform identity resolution

Endpoints must not:

- Accept identity from request bodies  
- Parse identity from headers  
- Trust client‑provided IDs  

Identity must be resolved via:

- `ICurrentUserService` (Application abstraction)  

This enforces **[Security Governance](ca://s?q=Open_security_governance)**.

---

# DTO Purity

DTOs used by endpoints must:

- Contain only data  
- Not reference domain entities  
- Not reference Application internals  
- Not reference Infrastructure types  
- Not contain behavior  

DTOs should be simple records or classes.

This enforces **[API Governance](ca://s?q=Open_api_governance)**.

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
All layers → SharedKernel
```

Endpoints must not depend on:

- Infrastructure  
- EF Core  
- Domain entities  
- Application handlers  

This enforces **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

# DI & SharedKernel Rules

Endpoints participate in DI only through:

- Dispatcher abstractions  
- Request DTO validators (auto‑registered)  
- Endpoint discovery (SharedKernel)  

Endpoints must not:

- Register services manually  
- Use Scrutor or suffix scanning  
- Use Infrastructure services  

This enforces **[Dependency Injection Architecture](ca://s?q=Open_dependency_injection_architecture)**.

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
8. Ensure endpoint is discovered via SharedKernel endpoint discovery  
9. Ensure endpoint follows API security rules  
10. Ensure endpoint follows error‑shaping conventions  

If an endpoint grows beyond ~10–20 lines, logic is leaking into the wrong layer.

---

# Related Documents

- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
- **[Dispatcher Pipeline](ca://s?q=Open_dispatcher_pipeline_guide)**  
- **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**  
- **[Architecture Governance](ca://s?q=Open_architecture_governance)**  
- **[Security Governance](ca://s?q=Open_security_governance)**  
