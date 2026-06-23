# Architecture Purity Rules  
Authoritative companion to ADR‑0002 (Layered Architecture)

Purity rules define the **non‑negotiable architectural boundaries** of Camp Fit Fur Dogs.  
Violations are treated with the same urgency as failing tests.

These rules ensure:

- Predictable dependency direction  
- Isolation of business logic  
- Testability  
- Maintainability  
- Governance alignment  
- Deterministic observability  
- Prevention of architectural drift  

Purity is a contract. Violations are defects.

---

# 1. Dependency Direction

Project references encode the dependency rules:

```
API → Application → Domain
         ↓
   Infrastructure
```

| Project | May Reference | Must Never Reference |
|---------|---------------|----------------------|
| **Domain** | Nothing | Application, Infrastructure, Api |
| **Application** | Domain | Infrastructure, Api |
| **Infrastructure** | Application, Domain | Api |
| **Api** | Application, Infrastructure | Domain (entities must not cross boundary) |
| **Frank** | Nothing | All product layers |

**Golden rule:** dependencies point inward.  
No layer may reference a layer above it.

---

# 2. Domain Purity

The Domain layer is the innermost, most protected layer.

### Allowed

- Entities, aggregate roots, value objects  
- Domain events (POCOs only)  
- Repository interfaces  
- Domain service interfaces  
- Enumerations and constants  
- Guard clauses and invariants  
- Pure C# (`System.*` only)

### Forbidden

- NuGet packages (EF Core, FluentValidation, MediatR, JSON attributes, etc.)  
- References to Application, Infrastructure, or Api  
- DTOs  
- I/O, HTTP, DB, logging  
- Observability emission (`ITraceEvents`, `IMetrics`)  
- DI attributes  
- Async/await  
- Service locators  

### Aggregate Boundary Rule

All mutations go through aggregate root methods.

````csharp
// Correct
dog.UpdateName("Buddy");

// Violation
dog.Name = "Buddy";
````

Domain is **observable through** Application/Infrastructure, not directly.

---

# 3. Application Purity

The Application layer orchestrates use cases.  
It references **Domain only**.

### Allowed

- Commands, queries  
- Handlers  
- Dispatchers  
- DTOs  
- Mapping logic  
- Infrastructure abstractions  
- Validators  
- CancellationToken propagation  
- Application exceptions  
- Observability emission at use‑case boundaries (`ITraceEvents`, `IMetrics`)  
- Propagation of `IObservabilityContext`  

### Forbidden

- Infrastructure types (`DbContext`, `HttpClient`, etc.)  
- Api references  
- Direct persistence  
- HTTP concepts  
- Identity from request bodies  
- Manual correlation ID creation  
- Ad‑hoc logging  

### Abstractions Folder Convention

Infrastructure‑facing interfaces live in:

```
Application/Abstractions/
```

---

# 4. Infrastructure Purity

Infrastructure implements interfaces from Application and Domain.

### Allowed

- EF Core  
- Repositories  
- Readers  
- External service integrations  
- Migrations  
- SDKs and NuGet packages  
- Observability emission for external calls, retries, failures  
- Correlation propagation to external systems  

### Forbidden

- Business logic  
- Api references  
- Endpoint awareness  
- Domain event creation  
- New public interfaces  
- Stopwatch timing  
- Vendor‑specific logging/metrics APIs  

### Repository Rule

Repositories return **Domain entities**, not DTOs or IQueryable.

---

# 5. API Purity

The API layer is the outermost shell.

### Allowed

- Minimal API endpoints  
- Middleware  
- DI setup  
- Auth configuration  
- Request binding  
- Response shaping  
- Observability emission for request boundaries  
- Use of Frank correlation middleware  

### Forbidden

- Business logic  
- Domain entities in responses  
- DbContext  
- Identity in request bodies  
- Infrastructure references in endpoints  
- Manual correlation ID creation  
- Ad‑hoc logging  

### Identity Resolution Pattern

````csharp
// Correct — identity resolved server-side
app.MapPost("/dogs", async (
    RegisterDogRequest request,
    ICurrentUserService currentUser,
    ICommandDispatcher dispatcher) =>
{
    var command = new RegisterDogCommand(
        currentUser.UserId,
        request.Name,
        request.Breed);

    await dispatcher.DispatchAsync(command);
    return Results.Created();
});

// Violation — identity accepted from the client
app.MapPost("/dogs", async (RegisterDogCommand command, ICommandDispatcher dispatcher) =>
{
    await dispatcher.DispatchAsync(command);
    return Results.Created();
});
````

---

# 6. Frank Purity

Frank is the **shared kernel** and **cross‑cutting backbone** of the system.  
It must remain **pure, minimal, and product‑agnostic**.

### Allowed

- Base entity/value object classes  
- Domain event primitives  
- Result/error primitives  
- Guard utilities  
- Endpoint discovery  
- DI auto‑registration  
- EF Core configuration scanning  
- Hosting abstractions  
- Security header + CORS middleware  
- Validation pipeline primitives  
- Observability primitives (`IObservabilityContext`, `ITraceEvents`, `IMetrics`)  
- Correlation propagation middleware  

### Forbidden

- Layer‑specific logic  
- Product‑specific logic  
- NuGet packages beyond base SDK  
- References to Domain, Application, Infrastructure, Api  
- Business rules  
- Persistence logic  
- HTTP/JSON/ZIP operations  
- Observability exporters or vendor integrations  

Frank is the lowest‑level dependency.

---

# 7. DI Purity

Dependency injection wiring happens **exclusively** in the API composition root.

### Rules

1. Registration lives in Api  
2. Handlers and validators are auto‑registered via Frank  
3. Lifetime defaults:  
   - Handlers → Scoped  
   - Validators → Scoped  
   - Repositories → Scoped  
   - DbContext → Scoped  
   - `ICurrentUserService` → Scoped  
4. No service locator  
5. No DI attributes in Domain  
6. No manual DI registration of slice services  
7. No Scrutor or suffix‑based scanning  
8. No DI logic in slices  
9. Frank owns all auto‑registration  

DI must be **observable** through structured startup events.

---

# 8. DTO Purity

DTOs are transport shapes only.

### Rules

1. Plain records/classes  
2. No behavior  
3. Request and response DTOs are separate  
4. Domain entities never cross the API boundary  
5. Mapping is explicit  
6. DTOs never contain identity fields like `OwnerId`  
7. DTOs never contain domain invariants  
8. DTOs never contain domain events  
9. DTOs never contain observability primitives  

---

# 9. Handler Purity

Handlers execute exactly one use case.

### Rules

1. One handler per command/query  
2. Depend only on abstractions  
3. Handlers never call other handlers  
4. No exception‑based flow control  
5. Stateless  
6. Propagate `CancellationToken`  
7. Do not raise domain events directly  
8. Do not perform validation  
9. Do not perform HTTP, I/O, or persistence directly  
10. Do not reference Infrastructure types  
11. Emit observability events/metrics only at use‑case boundaries  

---

# 10. Validator Purity

Validators enforce **shape correctness**, not business rules.

### Rules

1. Validate commands/queries, not entities  
2. Live in Application  
3. Use FluentValidation  
4. No database access  
5. One validator per command/query  
6. No Infrastructure references  
7. No domain logic  
8. No cross‑validator dependencies  
9. No observability emission  

---

# 11. Repository Purity

Repositories are the persistence gateway.

### Rules

1. Interfaces in Domain, implementations in Infrastructure  
2. Return Domain entities  
3. No business logic  
4. One repository per aggregate root  
5. Accept `CancellationToken`  
6. No `SaveChangesAsync()` inside repository methods  
7. No IQueryable exposure  
8. No DTOs returned  
9. No domain events raised  
10. Observability emission only for external calls, retries, failures  

---

# 12. Cross‑Layer Dependency Rules

| From → To | What Crosses | Form |
|-----------|--------------|------|
| API → Application | User intent | Commands/Queries |
| Application → Domain | Business operations | Aggregate methods, repository calls |
| Application → Infrastructure | Data access | Abstractions only |
| Infrastructure → Domain | Entity materialization | EF Core mapping |
| Domain → Nothing | — | — |

### Prohibited Crossings

- Entities never cross the API boundary  
- DbContext never leaves Infrastructure  
- HTTP types never enter Application or Domain  
- Configuration objects never enter Domain or Application  
- Domain events never cross the API boundary  
- Identity never comes from the client  
- Observability primitives never enter Domain  

---

# 13. Circular Dependency Rules

Circular dependencies are architectural defects.

### Prevention

1. No circular project references  
2. No circular namespaces  
3. No circular handler invocations  
4. No bidirectional entity navigation across aggregates  
5. No cross‑aggregate domain mutations  
6. No circular observability dependencies  

---

# 14. Guardrail Enforcement

Purity rules are enforced through:

### Layer 1: Compiler  
Project references enforce dependency direction.

### Layer 2: CI Pipeline  
Build & Test runs on every PR.

### Layer 3: Pre‑Push Hook  
Local `pre-push` runs `make test`.

### Layer 4: Code Review  
Reviewers check purity violations.

### Layer 5: Observability Tests (NEW)  
Validate correlation propagation, event emission, metric emission.

### Layer 6: This Document  
This file is the reference.

### Future Guardrails

| Guardrail | Status | Purpose |
|-----------|--------|---------|
| NetArchTest suite | Planned | Automated purity checks |
| Roslyn analyzer | Considered | Compile‑time warnings |
| `dotnet format` | Active | Style consistency |
| Observability analyzer | Considered | Enforce structured events/metrics |

---

# 15. Contributor Guidance

### Decision Tree

```
Is it a business rule or invariant?
  → Domain

Is it orchestrating a use case?
  → Application

Is it validating input shape?
  → Application (validator)

Is it talking to a database, API, or file system?
  → Infrastructure

Is it binding HTTP requests or responses?
  → Api

Is it wiring DI?
  → Api

Is it a reusable primitive?
  → Frank

Is it observability emission?
  → Application or Infrastructure only
```

### Common Mistakes and Fixes

| Mistake | Fix |
|---------|-----|
| EF Core attributes in Domain | Move to Infrastructure configuration |
| Returning entities from API | Map to DTO |
| Accepting `OwnerId` in request DTO | Remove; resolve via `ICurrentUserService` |
| Business logic in repository | Move to Domain |
| `SaveChangesAsync()` in repository | Move to handler/pipeline |
| Catching domain exceptions in handler | Let middleware map to HTTP |
| DbContext in Application | Use abstractions |
| Circular feature dependencies | Extract shared concept |
| Using service locator | Use constructor injection |
| Validator in Domain | Move to Application |
| Observability emission in Domain | Move to Application/Infrastructure |
| Stopwatch timing | Use Frank metrics timers |

### TDD and Purity

TDD naturally enforces purity:

1. Domain test → Domain code  
2. Application test → Handler  
3. Infrastructure test → Repository  
4. API test → Endpoint  
5. Observability test → Event/metric correctness  

If a test requires violating purity, the design is wrong.

### Updating This Document

- Convention‑changing PRs update this file  
- Reviewed during retrospectives  
- “We got burned” moments update immediately  
