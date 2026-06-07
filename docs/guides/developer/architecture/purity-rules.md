# Purity Rules

> Architectural purity rules for Camp Fit Fur Dogs.  
> Authoritative companion to ADR‑0002 (DDD Layered Architecture).  
> Violations are bugs — treat them with the same urgency as a failing test.

---

# Table of Contents

- [Dependency Direction](#dependency-direction)
- [Domain Purity](#domain-purity)
- [Application Purity](#application-purity)
- [Infrastructure Purity](#infrastructure-purity)
- [API Purity](#api-purity)
- [Frank Purity](#frank-purity)
- [DI Purity](#di-purity)
- [DTO Purity](#dto-purity)
- [Handler Purity](#handler-purity)
- [Validator Purity](#validator-purity)
- [Repository Purity](#repository-purity)
- [Cross-Layer Dependency Rules](#cross-layer-dependency-rules)
- [Circular Dependency Rules](#circular-dependency-rules)
- [Guardrail Enforcement](#guardrail-enforcement)
- [Contributor Guidance](#contributor-guidance)

---

# Dependency Direction

The compiler is the first line of defense. Project references encode the dependency rules — if the `.csproj` doesn't allow it, the code can't compile.

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
| **Api** | Application, Infrastructure | Domain (direct use of entities — see API Purity) |
| **Frank** | Nothing | All product layers |

**Golden rule:** dependencies point inward.  
No layer may reference a layer above it.

---

# Domain Purity

The Domain project is the innermost layer. It has **zero** project references and **zero** NuGet dependencies beyond the base SDK.

### Allowed

- Entities, aggregate roots, value objects  
- Domain events (POCOs only)  
- Repository interfaces  
- Domain service interfaces  
- Enumerations and domain constants  
- Custom domain exceptions  
- Guard clauses and invariants  
- Pure C# (`System.*` only)

### Forbidden

- **No NuGet packages** (EF Core, FluentValidation, MediatR, JSON attributes, etc.)  
- **No references** to Application, Infrastructure, or Api  
- **No DTOs**  
- **No service locators**  
- **No I/O, HTTP, DB, logging**  
- **No DI attributes**  
- **No async/await**  

### Aggregate Boundary Rule

All mutations go through aggregate root methods.

```csharp
// ✅ Correct
dog.UpdateName("Buddy");

// ❌ Violation
dog.Name = "Buddy";
```

---

# Application Purity

The Application layer orchestrates use cases. It references **Domain only**.

### Allowed

- Commands, queries  
- Handlers  
- Dispatchers  
- DTOs  
- Mapping logic  
- Abstractions for infrastructure concerns  
- Validators  
- CancellationToken propagation  
- Application exceptions  

### Forbidden

- **No Infrastructure types** (`DbContext`, `HttpClient`, etc.)  
- **No Api references**  
- **No direct persistence**  
- **No HTTP concepts**  
- **No user identity from request bodies**  

### Abstractions Folder Convention

Infrastructure‑facing interfaces live in:

```
Application/Abstractions/
```

---

# Infrastructure Purity

Infrastructure implements interfaces from Application and Domain.

### Allowed

- EF Core  
- Repositories  
- Readers  
- External service integrations  
- Migrations  
- SDKs and NuGet packages  

### Forbidden

- **No business logic**  
- **No Api references**  
- **No endpoint awareness**  
- **No domain event creation**  
- **No new public interfaces**  

### Repository Rule

Repositories return **Domain entities**, not DTOs or IQueryable.

---

# API Purity

The API layer is the outermost shell.

### Allowed

- Minimal API endpoints  
- Middleware  
- DI setup  
- Auth configuration  
- Request binding  
- Response shaping  

### Forbidden

- **No business logic**  
- **No Domain entities in responses**  
- **No DbContext**  
- **No identity in request bodies**  
- **No Infrastructure references in endpoints**  

### Identity Resolution Pattern

```csharp
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

// ❌ Violation — identity accepted from the client
app.MapPost("/dogs", async (RegisterDogCommand command, ICommandDispatcher dispatcher) =>
{
    await dispatcher.DispatchAsync(command);
    return Results.Created();
});
```

---

# Frank Purity

Frank is the **shared kernel** and **cross‑cutting backbone** of the entire system.  
It must remain **pure, minimal, and product‑agnostic**.

### Allowed

- Base entity class (`Entity<TId>`)  
- Base value object class  
- Domain event base interface/class  
- Common result/error primitives  
- Guard clause utilities  
- Endpoint discovery primitives  
- DI auto‑registration engine  
- EF Core configuration scanning  
- Hosting provider abstractions  
- Security header + CORS middleware  
- Validation pipeline primitives  

### Forbidden

- **No layer‑specific logic**  
- **No product‑specific logic**  
- **No NuGet packages** beyond the base SDK  
- **No references to Domain, Application, Infrastructure, or Api**  
- **No “everything drawer” drift**  
- **No business rules**  
- **No persistence logic**  
- **No HTTP/JSON/ZIP operations**  

Frank is the **lowest‑level dependency** in the system.

---

# DI Purity

Dependency injection wiring happens **exclusively** in the API layer’s composition root.

### Rules

1. **Registration lives in Api**  
2. **Handlers and validators are auto‑registered** via Frank  
3. **Lifetime defaults:**  
   - Handlers → Scoped  
   - Validators → Scoped  
   - Repositories → Scoped  
   - DbContext → Scoped  
   - `ICurrentUserService` → Scoped  
4. **No service locator**  
5. **No DI attributes in Domain**  
6. **No manual DI registration of slice services**  
7. **No Scrutor or suffix‑based scanning**  
8. **No DI logic in slices**  
9. **Frank owns all auto‑registration**  

---

# DTO Purity

DTOs are **transport shapes only**.

### Rules

1. Plain records/classes  
2. No behavior  
3. Request and response DTOs are separate  
4. Domain entities never cross the API boundary  
5. Mapping is explicit and mechanical  
6. DTOs never contain identity fields like `OwnerId`  
7. DTOs never contain domain invariants  
8. DTOs never contain domain events  

---

# Handler Purity

Handlers execute exactly one use case.

### Rules

1. One handler per command/query  
2. Depend only on abstractions  
3. Handlers never call other handlers  
4. No exception‑based flow control  
5. Handlers are stateless  
6. Handlers propagate `CancellationToken`  
7. Handlers do not raise domain events directly  
8. Handlers do not perform validation  
9. Handlers do not perform HTTP, I/O, or persistence directly  
10. Handlers do not reference Infrastructure types  

---

# Validator Purity

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

---

# Repository Purity

Repositories are the **persistence gateway**.

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

---

# Cross‑Layer Dependency Rules

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

---

# Circular Dependency Rules

Circular dependencies are architectural defects.

### Prevention Rules

1. No circular project references  
2. No circular namespaces  
3. No circular handler invocations  
4. No bidirectional entity navigation across aggregates  
5. No cross‑aggregate domain mutations  

### Detection

- Compiler prevents circular `.csproj` references  
- Code review catches namespace/handler cycles  
- Architecture tests (future) will enforce this  

---

# Guardrail Enforcement

Purity rules are enforced through multiple layers.

### Layer 1: Compiler

- `.csproj` references enforce dependency direction  
- Cannot be bypassed  

### Layer 2: CI Pipeline

- Build & Test runs on every PR  
- Future: architecture tests  

### Layer 3: Pre‑Push Hook

- Local `pre-push` runs `make test`  

### Layer 4: Code Review

- Reviewers check purity violations  
- Merge checklist includes purity review  

### Layer 5: This Document

- This file is the reference  
- Changes require PR updates  

### Future Guardrails

| Guardrail | Status | Purpose |
|-----------|--------|---------|
| NetArchTest suite | Planned | Automated purity checks |
| Roslyn analyzer | Considered | Compile‑time warnings |
| `dotnet format` | Active | Style consistency |

---

# Contributor Guidance

### Before You Write Code

1. Read ADR‑0002  
2. Read this document  
3. Read `copilot-instructions.md`  

### Quick Decision Tree

```
Where does this code belong?

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

### TDD and Purity

TDD naturally enforces purity:

1. Domain test → Domain code  
2. Application test → Handler  
3. Infrastructure test → Repository  
4. API test → Endpoint  

If a test requires violating purity, the design is wrong.

### Updating This Document

- Convention‑changing PRs update this file  
- Reviewed during retrospectives  
- “We got burned” moments update immediately  

