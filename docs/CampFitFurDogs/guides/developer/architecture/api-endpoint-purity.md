# API Endpoint Purity Guide  
Product‑specific architectural rules for Camp Fit Fur Dogs

API endpoints are the outermost layer of the Camp Fit Fur Dogs backend.  
They are intentionally thin, predictable, and free of business logic.

Endpoints translate HTTP requests into commands/queries, invoke the Frank dispatcher, and translate results into HTTP responses. All business logic lives in the Application and Domain layers.

This document defines the **product‑specific purity rules** for API endpoints.

---

# 1. Purpose

Endpoints exist to:

- Bind HTTP requests  
- Perform syntactic (shape) validation  
- Map request DTOs to commands/queries  
- Dispatch commands/queries through Frank  
- Map results to response DTOs  
- Return HTTP responses  

Endpoints orchestrate — they do not execute logic.

---

# 2. High‑Level Flow

```
HTTP Request
    ↓
API Endpoint
    ↓
Command / Query
    ↓
Frank Dispatcher Pipeline
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

---

# 3. Responsibilities of API Endpoints

### Endpoints DO:

- Bind route, query, and body parameters  
- Validate request **shape**  
- Map request DTO → command/query  
- Call the Frank dispatcher  
- Map result → response DTO  
- Return appropriate HTTP status codes  
- Use Frank endpoint discovery attributes  
- Rely on Frank middleware (security headers, CORS, error shaping)  

### Endpoints DO NOT:

- Contain business logic  
- Call repositories  
- Instantiate or manipulate domain entities  
- Bypass the dispatcher pipeline  
- Invoke handlers directly  
- Reference Application internals  
- Reference Infrastructure types  
- Perform domain mutations  
- Resolve identity manually  
- Perform authorization logic  
- Access configuration or environment  
- Perform HTTP/JSON/ZIP operations  
- Interact with hosting providers  

---

# 4. Typical Endpoint Pattern

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

---

# 5. Dependency Rules

Endpoints may depend on:

- Commands  
- Queries  
- Result DTOs  
- Frank dispatcher interfaces  
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

Dependency direction remains:

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

DTOs must:

- Contain only data  
- Not reference domain entities  
- Not reference Application internals  
- Not reference Infrastructure types  
- Not contain behavior  

DTOs are transport shapes only.

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

- `ICurrentUser` (Application abstraction)

Authorization is enforced by:

- Frank authorization seams  
- Application rules  

Endpoints simply declare:

```csharp
[Authorize]
```

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

1. Create request/response DTOs  
2. Create command/query  
3. Create handler  
4. Create validator  
5. Add endpoint using Frank’s dispatcher  
6. Emit product‑specific observability events  
7. Add tests  
8. Keep endpoint logic limited to mapping and HTTP concerns  
9. Do not bypass the dispatcher pipeline  
10. Ensure endpoint follows API purity rules  

If an endpoint grows beyond ~10–20 lines, logic is leaking into the wrong layer.

---

# 12. Summary

- Endpoints are pure orchestrators  
- They handle HTTP concerns only  
- They never contain business logic  
- They always use the dispatcher pipeline  
- They rely on Frank for middleware, discovery, and security  
- They keep the system clean, predictable, and maintainable  

---

# Related Documents

- CFFD Developer Guide  
- Validation Conventions  
- Mapping Conventions  
- Frank Dispatcher Pipeline Guide  
