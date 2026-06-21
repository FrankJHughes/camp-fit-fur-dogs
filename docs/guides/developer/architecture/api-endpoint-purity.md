# API Endpoint Purity Guide  
*A developer‑facing architecture guide for building clean, predictable, and maintainable API endpoints.*

API endpoints are the outermost layer of the backend.  
Their job is simple and consistent:

- Accept HTTP requests  
- Validate **shape** (not business rules)  
- Map to commands/queries  
- Invoke the dispatcher pipeline  
- Map results to response DTOs  
- Return HTTP responses  

Endpoints are intentionally thin.  
All business logic flows through **Application** and **Domain**, never through the API layer.

This guide explains how endpoints fit into the architecture and how to keep them pure, predictable, and easy to maintain.

---

# 1. Purpose

Endpoints exist to:

- Bind HTTP requests  
- Perform syntactic validation  
- Translate requests into commands/queries  
- Dispatch those commands/queries  
- Translate results into HTTP responses  

Endpoints are **orchestrators**, not logic containers.

---

# 2. High‑Level Flow

A request flows through the system like this:

```
HTTP Request
    ↓
API Endpoint
    ↓
Command / Query
    ↓
Dispatcher Pipeline
    ↓
Application Handler
    ↓
Domain
    ↓
Result
    ↓
API Endpoint
    ↓
HTTP Response
```

Endpoints are the glue between HTTP and the application pipeline.

---

# 3. Responsibilities of API Endpoints

Endpoints handle **HTTP concerns only**.

### Endpoints DO:

- Bind route, query, and body parameters  
- Validate request **shape** (syntactic validation)  
- Map request DTO → command/query  
- Call the dispatcher  
- Map result → response DTO  
- Return appropriate HTTP status codes  
- Participate in Frank endpoint discovery  
- Rely on Frank middleware (security headers, CORS, error boundary)  

### Endpoints DO NOT:

- Contain business logic  
- Call repositories  
- Instantiate or manipulate domain entities  
- Bypass the dispatcher pipeline  
- Invoke handlers directly  
- Reference Application handlers or validators  
- Reference Infrastructure types  
- Perform domain mutations  
- Perform identity resolution  
- Perform authorization logic  
- Access configuration or environment  
- Perform HTTP/JSON/ZIP operations  
- Interact with hosting providers  

Endpoints orchestrate — they do not execute logic.

---

# 4. Typical Endpoint Flow

A clean endpoint follows this pattern:

```csharp
var command = new RegisterDogCommand(request.Name, request.Breed);

var result = await commandDispatcher.DispatchAsync(command, cancellationToken);

return Results.Created($"/dogs/{result.DogId}", result);
```

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

This is the ideal endpoint shape.

---

# 5. Dependency Rules

Endpoints depend only on:

- Commands  
- Queries  
- Result DTOs  
- Dispatcher interfaces  
- ASP.NET primitives  
- Frank endpoint discovery attributes  

Endpoints must **not** depend on:

- Application handlers  
- Validators  
- Domain entities  
- Infrastructure types  
- EF Core DbContexts  
- Hosting providers  
- Environment abstractions  

This keeps dependency direction clean:

```
Api → Application → Domain
Infrastructure → Application → Domain
All layers → Frank
```

---

# 6. Dispatcher Usage

Endpoints must use:

- `ICommandDispatcher` for commands  
- `IQueryDispatcher` for queries  

Direct handler invocation is not allowed.

This ensures:

- Validation runs automatically  
- Domain events are collected and dispatched  
- Cross‑cutting behaviors (logging, metrics, etc.) run consistently  

---

# 7. DTO Purity

DTOs used by endpoints must:

- Contain only data  
- Not reference domain entities  
- Not reference Application internals  
- Not reference Infrastructure types  
- Not contain behavior  

DTOs are **transport shapes**, nothing more.

---

# 8. Mapping Rules

Endpoints may:

- Map request DTO → command/query  
- Map result → response DTO  

Endpoints must not:

- Map domain entities directly to HTTP responses  
- Perform business transformations  
- Perform validation beyond shape validation  

Mapping must remain mechanical and predictable.

---

# 9. Identity & Authorization

Endpoints do **not**:

- Accept identity from request bodies  
- Parse identity from headers  
- Trust client‑provided IDs  
- Perform authorization checks manually  

Identity is resolved via:

- `ICurrentUserService` (Application abstraction)

Authorization is enforced by:

- Frank authorization seams  
- Application rules  

Endpoints simply declare:

```csharp
[Authorize]
```

…and let the system handle the rest.

---

# 10. Middleware Integration

Endpoints rely on Frank middleware for:

- Error shaping  
- Security headers  
- CORS  
- Rate limiting  
- Session validation  

Endpoints must not implement these concerns manually.

---

# 11. Contributor Guidelines

When adding a new endpoint:

1. Define the command/query and result in Application Abstractions  
2. Implement the handler in Application  
3. Add validators if needed  
4. Use the dispatcher from the endpoint  
5. Keep endpoint logic limited to mapping and HTTP concerns  
6. Do not bypass the dispatcher pipeline  
7. Do not reference Application internals or Infrastructure  
8. Ensure endpoint is discovered via Frank endpoint discovery  
9. Ensure endpoint follows API security rules  
10. Ensure endpoint relies on Frank middleware  

If an endpoint grows beyond ~10–20 lines, logic is leaking into the wrong layer.

---

# 12. Summary

- Endpoints are **pure orchestrators**  
- They handle HTTP concerns only  
- They never contain business logic  
- They always use the dispatcher pipeline  
- They rely on Frank for middleware, discovery, and security  
- They keep the system clean, predictable, and maintainable  

---

# Related Documents

- Dispatcher Pipeline Guide  
- Domain Events Architecture  
- Validation Boundaries  
- Authentication Overview  
- Architecture Overview  
