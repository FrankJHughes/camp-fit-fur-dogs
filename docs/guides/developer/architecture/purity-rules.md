# Purity Rules

> Architectural purity rules for Camp Fit Fur Dogs.
> Authoritative companion to [ADR-0002](../adr/0002-ddd-layered-architecture.md).
> Violations are bugs — treat them with the same urgency as a failing test.

---

## Table of Contents

- [Dependency Direction](#dependency-direction)
- [Domain Purity](#domain-purity)
- [Application Purity](#application-purity)
- [Infrastructure Purity](#infrastructure-purity)
- [API Purity](#api-purity)
- [SharedKernel Purity](#sharedkernel-purity)
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

## Dependency Direction

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
| **Api** | Application, Infrastructure | Domain (direct use of entities — see [API Purity](#api-purity)) |
| **SharedKernel** | Nothing | All four layers |

**The golden rule:** dependencies point inward. No layer may reference a layer above it. The compiler enforces this through `.csproj` `<ProjectReference>` entries — never add a reference that violates this table.

---

## Domain Purity

The Domain project is the innermost layer. It has **zero** project references and **zero** NuGet package references beyond the base SDK.

### Allowed

- Entities, aggregate roots, value objects
- Domain events (POCOs — no framework base classes)
- Repository interfaces (e.g., `IDogRepository`)
- Domain service interfaces (pure business logic contracts)
- Enumerations and constants representing domain concepts
- Custom domain exceptions (e.g., `DogNotFoundException`)
- Guard clauses and invariant enforcement inside entity methods
- Pure C# — `System.*` namespaces only

### Forbidden

- **No NuGet packages.** No EF Core attributes, no FluentValidation, no MediatR, no JSON serialization attributes. Zero external dependencies.
- **No `using` statements** referencing Application, Infrastructure, or Api namespaces.
- **No DTOs.** Entities are not shaped for transport — that's the Application layer's job.
- **No static service locators** or ambient context patterns (e.g., `ServiceLocator.Get<T>()`).
- **No file I/O, HTTP calls, database access, or logging frameworks.** The Domain has no knowledge of the outside world.
- **No dependency injection attributes** (`[Inject]`, `[FromServices]`). Domain objects are constructed explicitly.
- **No `async/await`.** Domain logic is synchronous and deterministic. Asynchronous orchestration belongs in Application.

### Aggregate Boundary Rule

All mutations go through aggregate root methods. External code never mutates child entities or value objects directly.

```csharp
// ✅ Correct — mutation through aggregate root
dog.UpdateName("Buddy");

// ❌ Violation — reaching into the aggregate
dog.Name = "Buddy";
```

---

## Application Purity

The Application layer orchestrates use cases. It references **Domain only**.

### Allowed

- Command and query objects (`ICommand`, `IQuery<TResponse>`)
- Command and query handlers (`ICommandHandler`, `IQueryHandler`)
- Dispatcher interfaces (`ICommandDispatcher`, `IQueryDispatcher`)
- DTOs (request/response objects for the API boundary)
- Mapping logic (entity ↔ DTO)
- Abstraction interfaces for infrastructure concerns (e.g., `ICurrentUserService`, `IEmailService`)
- FluentValidation validators for commands/queries
- `CancellationToken` propagation on handler signatures
- Application-layer exceptions (e.g., `ValidationException`, `UnauthorizedAccessException`)

### Forbidden

- **No concrete infrastructure types.** No `DbContext`, no `HttpClient`, no `SmtpClient`. Only interfaces.
- **No `using` statements** referencing Infrastructure or Api namespaces.
- **No direct entity persistence.** Handlers call repository interfaces — they never instantiate `DbContext` or call `SaveChanges()`.
- **No HTTP concepts.** No `HttpContext`, no status codes, no route attributes. The Application layer doesn't know it's behind a web server.
- **No user identity from parameters.** Handlers receive identity through `ICurrentUserService`, never from command/query properties that the caller could forge.

### Abstractions Folder Convention

Infrastructure-facing interfaces live in `Application/Abstractions/`. This is the seam where Infrastructure plugs in without the Application knowing the concrete type.

```
Application/
├── Abstractions/
│   ├── ICurrentUserService.cs
│   └── IEmailService.cs
├── Dogs/
│   ├── RegisterDog/
│   │   ├── RegisterDogCommand.cs
│   │   └── RegisterDogCommandHandler.cs
│   └── GetDogById/
│       ├── GetDogByIdQuery.cs
│       └── GetDogByIdQueryHandler.cs
└── DTOs/
    ├── RegisterDogRequest.cs
    └── DogResponse.cs
```

---

## Infrastructure Purity

The Infrastructure layer implements interfaces defined in Domain and Application. It references **Application and Domain**.

### Allowed

- EF Core `DbContext` and entity configurations (`IEntityTypeConfiguration<T>`)
- Concrete repository implementations (implementing interfaces from Domain)
- Concrete service implementations (implementing interfaces from Application)
- Database migrations
- External service integrations (HTTP clients, message queues, file storage)
- NuGet packages for data access, serialization, and external SDKs

### Forbidden

- **No business logic.** Repository methods perform CRUD — they don't enforce invariants. That's the Domain's job.
- **No `using` statements** referencing Api namespaces.
- **No endpoint or controller awareness.** Infrastructure doesn't know about routes, middleware, or HTTP.
- **No domain event raising.** Events are raised by aggregates inside Domain. Infrastructure dispatches them — it doesn't create them.
- **No new public interfaces.** Infrastructure implements interfaces; it doesn't define them for other layers to consume.

### Repository Implementation Rule

Repositories implement Domain interfaces and return Domain entities. They never return DTOs, `IQueryable`, or EF-tracked change sets to the caller.

```csharp
// ✅ Correct — returns a Domain entity
public async Task<Dog?> GetByIdAsync(Guid id, CancellationToken ct)
    => await _context.Dogs.FirstOrDefaultAsync(d => d.Id == id, ct);

// ❌ Violation — leaking IQueryable to caller
public IQueryable<Dog> GetAll() => _context.Dogs;
```

---

## API Purity

The API layer is the outermost shell. It references **Application and Infrastructure** (Infrastructure only for DI wiring in `Program.cs`).

### Allowed

- Minimal API endpoint definitions in `Endpoints.cs` using the `MapGroup` pattern
- Middleware registration and pipeline configuration
- DI container setup (`Program.cs` / extension methods)
- Authentication/authorization configuration
- Request DTO binding from HTTP (route, query, body)
- Mapping HTTP status codes to handler results

### Forbidden

- **No business logic.** Endpoints are thin — they bind a request, dispatch a command/query, and return a response. Nothing else.
- **No direct Domain entity usage in request/response models.** Always use DTOs from the Application layer.
- **No direct `DbContext` usage.** The API never talks to the database directly — it goes through Application handlers.
- **No user identity from request bodies.** Endpoints inject `ICurrentUserService` and pass the resolved identity into commands. The request DTO must never contain `OwnerId` or equivalent fields.
- **No Infrastructure type references outside `Program.cs`.** Endpoint files must not `using` Infrastructure namespaces. Only the composition root wires concrete types.

### Identity Resolution Pattern

```csharp
// ✅ Correct — identity resolved server-side
app.MapPost("/dogs", async (
    RegisterDogRequest request,
    ICurrentUserService currentUser,
    ICommandDispatcher dispatcher) =>
{
    var command = new RegisterDogCommand(
        currentUser.UserId,  // Server-resolved, not from request body
        request.Name,
        request.Breed);
    await dispatcher.DispatchAsync(command);
    return Results.Created();
});

// ❌ Violation — identity accepted from the client
app.MapPost("/dogs", async (RegisterDogCommand command, ICommandDispatcher dispatcher) =>
{
    await dispatcher.DispatchAsync(command);  // OwnerId came from the request!
    return Results.Created();
});
```

---

## SharedKernel Purity

SharedKernel contains cross-cutting building blocks reused by multiple layers. It has **zero** project references.

### Allowed

- Base entity class (e.g., `Entity<TId>`)
- Value object base class
- Domain event base interface/class
- Common result/error types
- Guard clause utilities

### Forbidden

- **No layer-specific logic.** SharedKernel is a library of primitives — it must not contain business rules, persistence logic, or HTTP concepts.
- **No NuGet packages** beyond the base SDK. SharedKernel stays as lean as Domain.
- **No "everything drawer" drift.** If a type is used by only one layer, it belongs in that layer, not SharedKernel.

---

## DI Purity

Dependency injection wiring happens **exclusively** in the API layer's composition root (`Program.cs` or dedicated extension methods in the Api project).

### Rules

1. **Registration lives in the Api project.** Each layer may expose a convenience extension method (e.g., `services.AddInfrastructure()`), but the Api project calls them.
2. **Handlers and validators are registered by convention**, not manually. Use assembly scanning to discover `ICommandHandler`, `IQueryHandler`, and `IValidator<T>` implementations.
3. **Lifetime defaults:**
   - Handlers → **Scoped** (one instance per request)
   - Validators → **Scoped**
   - Repositories → **Scoped** (tied to `DbContext` lifetime)
   - `DbContext` → **Scoped** (EF Core default)
   - `ICurrentUserService` → **Scoped** (reads from `HttpContext` per-request)
   - Dispatchers → **Scoped** or **Singleton** (stateless)
4. **Never resolve services inside a constructor using `IServiceProvider` directly.** Use constructor injection. Service locator is an anti-pattern.
5. **No DI attributes on Domain types.** Domain objects are created via constructors, factories, or aggregate methods — never injected.

---

## DTO Purity

DTOs exist at the Application layer boundary. They are transport shapes — nothing more.

### Rules

1. **DTOs are plain C# records or classes.** No inheritance from Domain entities. No shared base class with entities.
2. **DTOs carry no behavior.** No methods, no validation logic, no computed properties with side effects. They are data bags.
3. **Request DTOs and response DTOs are separate types**, even when the shapes overlap. This allows them to evolve independently.
4. **Domain entities are never exposed through the API.** Every endpoint returns a DTO or a primitive — never an entity.
5. **Mapping between entities and DTOs is explicit.** Use manual mapping methods or a lightweight mapper in the Application layer. The mapping direction is always entity → response DTO (outbound) or request DTO → command/query (inbound).
6. **DTOs must not contain `OwnerId` or equivalent identity fields** in request shapes. Identity is resolved server-side via `ICurrentUserService`.

---

## Handler Purity

Handlers are the workhorses of the Application layer. Each handler executes exactly one use case.

### Rules

1. **One handler per command or query.** No multi-purpose handlers. The class name matches the operation: `RegisterDogCommandHandler` handles `RegisterDogCommand`.
2. **Handlers depend only on abstractions.** Constructor parameters are interfaces (`IDogRepository`, `ICurrentUserService`), never concrete types.
3. **Handlers don't call other handlers.** If two use cases share logic, extract it into a domain service or a shared domain method — don't chain handlers.
4. **Handlers don't catch exceptions for flow control.** Let exceptions propagate to middleware. Handlers throw domain or application exceptions; the API layer maps them to HTTP responses.
5. **Handlers are stateless.** All state comes from the injected dependencies and the incoming command/query. No instance fields that accumulate state across calls.
6. **Handlers own the `CancellationToken`.** Every async handler method accepts and propagates `CancellationToken`.

---

## Validator Purity

Validators run before handlers and enforce structural/input correctness.

### Rules

1. **Validators validate commands and queries, not entities.** Entity invariants are enforced inside the Domain (aggregate methods, constructors, value object creation).
2. **Validators live in the Application layer** alongside their corresponding command/query.
3. **Validators use FluentValidation** (or a similar declarative library). No manual if/throw blocks for input validation.
4. **Validators don't access the database.** Uniqueness checks and cross-record validations happen inside handlers where the repository is available. Validators enforce shape: required fields, string lengths, format patterns.
5. **One validator per command/query.** Named consistently: `RegisterDogCommandValidator` validates `RegisterDogCommand`.

---

## Repository Purity

Repositories are the persistence gateway between Domain and Infrastructure.

### Rules

1. **Interfaces live in Domain.** Implementations live in Infrastructure. The Domain defines *what* it needs; Infrastructure decides *how*.
2. **Repository methods return Domain entities**, never DTOs, anonymous types, or `IQueryable`.
3. **Repositories don't contain business logic.** They perform data access operations: get, add, update, delete. Business rules live in the entity or domain service.
4. **One repository per aggregate root.** Child entities are accessed through their aggregate — they don't get their own repository.
5. **Repositories accept `CancellationToken`** on all async methods.
6. **No `SaveChangesAsync()` inside repository methods.** Unit of work (commit) is managed at the handler level or by a pipeline behavior. The repository adds/updates entities; the handler decides when to commit.

---

## Cross-Layer Dependency Rules

These rules govern what may cross a layer boundary and in what form.

| From → To | What Crosses | Form |
|-----------|-------------|------|
| API → Application | User intent | Command/Query objects (DTOs → Command mapping in endpoint) |
| Application → Domain | Business operations | Method calls on aggregate roots, repository interface calls |
| Application → Infrastructure | Data access, external services | Via abstraction interfaces only (defined in Application or Domain) |
| Infrastructure → Domain | Entity materialization | EF Core maps database rows to Domain entities |
| Domain → Nothing | Nothing leaves Domain | Domain has no outbound dependencies |

### Prohibited Crossings

- **Entities never cross the API boundary.** Always map to DTOs.
- **`DbContext` never appears outside Infrastructure.** Not in Application, not in Api endpoints.
- **HTTP types never appear inside Application or Domain.** No `HttpContext`, `HttpRequest`, `IActionResult`, or status codes.
- **Configuration objects (`IOptions<T>`, `IConfiguration`)** are consumed in Infrastructure or Api — never in Domain, and only by abstractions in Application.

---

## Circular Dependency Rules

Circular dependencies are architectural defects. They break testability, create hidden coupling, and make the dependency graph unmaintainable.

### Prevention Rules

1. **No circular project references.** The `.csproj` dependency graph is a DAG (directed acyclic graph). If adding a reference creates a cycle, the design is wrong — introduce an interface or restructure.
2. **No circular namespace dependencies within a project.** If `Dogs/` depends on `Owners/` and `Owners/` depends on `Dogs/`, extract the shared concept into a common namespace or into SharedKernel.
3. **No circular handler invocations.** Handler A must not dispatch a command/query that routes to Handler B, which dispatches back to Handler A. If the logic is shared, extract it to a domain service.
4. **No bidirectional entity navigation for cross-aggregate relationships.** An aggregate may hold a foreign key (value) to another aggregate, but not a navigation property. Navigation within an aggregate (root → children) is fine.

### Detection

- The compiler prevents circular `.csproj` references at build time.
- Code review should catch circular namespace or handler dependencies.
- Future: consider an architecture test (e.g., NetArchTest) to automate detection within projects.

---

## Guardrail Enforcement

Purity rules are only useful if they're enforced. Enforcement is layered — automated where possible, human where necessary.

### Layer 1: Compiler

- `.csproj` `<ProjectReference>` entries enforce the dependency direction. If it doesn't compile, it doesn't merge.
- This is the strongest guardrail. No PR can bypass it.

### Layer 2: CI Pipeline

- `Build & Test` CI runs on every PR. A broken build blocks the merge.
- Future enhancement: add architecture tests (e.g., NetArchTest or ArchUnitNET) as a CI step to enforce rules that the compiler can't catch (namespace dependencies, naming conventions, forbidden `using` statements).

### Layer 3: Pre-Push Hook

- The local `hooks/pre-push` hook runs `make test` before any push reaches the remote.
- Not foolproof (hooks can be bypassed), but catches obvious violations early.

### Layer 4: Code Review

- PR reviewers check for purity violations as part of the self-review and merge checklist.
- The merge checklist in `.github/PULL_REQUEST_TEMPLATE.md` includes a docs and review step — reviewers should flag any purity concern.

### Layer 5: This Document

- This file is the reference. When in doubt, check here.
- Disagreements with a rule go through a PR to update this file — not through silent violation.

### Future Guardrails

| Guardrail | Status | Purpose |
|-----------|--------|---------|
| NetArchTest suite | Planned | Automated purity checks: forbidden references, naming conventions, layer boundary enforcement |
| Roslyn analyzer | Considered | Compile-time warnings for common violations (e.g., entity in API response) |
| `dotnet format` | Active | Code style consistency (not purity, but adjacent) |

---

## Contributor Guidance

### Before You Write Code

1. **Read [ADR-0002](../adr/0002-ddd-layered-architecture.md).** Understand the four layers and why they exist.
2. **Read this document.** Understand what goes where.
3. **Read [copilot-instructions.md](../../.github/copilot-instructions.md).** Understand TDD discipline, commit style, and PR conventions.

### Quick Decision Tree

```
Where does this code belong?

Is it a business rule or invariant?
  → Domain (entity method, value object, domain service)

Is it orchestrating a use case (coordinating entities + repos)?
  → Application (handler)

Is it validating input shape (required, length, format)?
  → Application (validator)

Is it talking to a database, API, or file system?
  → Infrastructure (concrete implementation of a Domain/Application interface)

Is it binding HTTP requests or returning HTTP responses?
  → Api (endpoint in Endpoints.cs)

Is it wiring up DI registrations?
  → Api (Program.cs or extension methods)

Is it a reusable building block (base class, guard clause)?
  → SharedKernel
```

### Common Mistakes and Fixes

| Mistake | Fix |
|---------|-----|
| Adding EF Core attributes to a Domain entity | Use `IEntityTypeConfiguration<T>` in Infrastructure instead |
| Returning an entity from an API endpoint | Create a response DTO in Application and map the entity |
| Accepting `OwnerId` in a request DTO | Remove it; inject `ICurrentUserService` in the endpoint and pass the resolved ID into the command |
| Adding business logic to a repository | Move the logic into the aggregate root or a domain service |
| Calling `SaveChangesAsync()` inside a repository method | Let the handler or a pipeline behavior manage the unit of work |
| Catching domain exceptions in a handler to return a status code | Let exceptions propagate; map them to HTTP in Api middleware |
| Adding a `DbContext` dependency to an Application handler | Define an interface in `Application/Abstractions/` and implement it in Infrastructure |
| Creating a circular reference between feature folders | Extract the shared concept into a common namespace or SharedKernel |
| Using `IServiceProvider.GetService<T>()` in a constructor | Use constructor injection — service locator hides dependencies |
| Putting a validator in the Domain layer | Validators validate input DTOs in Application; domain invariants are enforced by entity methods |

### TDD and Purity

TDD naturally enforces purity. When you write a Domain test, you'll immediately feel the pain of a forbidden dependency — the test project can't reference Infrastructure, so you can't accidentally use `DbContext` in domain logic.

**Red-green-refactor layer by layer:**

1. **Domain test (red):** Write a test for the entity behavior. It fails because the method doesn't exist yet.
2. **Domain code (green):** Implement the method with minimum code to pass.
3. **Application test (red):** Write a test for the handler. Mock the repository. It fails because the handler doesn't exist.
4. **Application code (green):** Implement the handler.
5. **Infrastructure test (red):** Write an integration test for the repository. It fails because the implementation doesn't exist.
6. **Infrastructure code (green):** Implement the repository with EF Core.
7. **API test (red):** Write an integration test for the endpoint. It fails because the endpoint doesn't exist.
8. **API code (green):** Wire up the endpoint in `Endpoints.cs`.

If at any point a test requires you to violate a purity rule to make it pass, the design is wrong — not the rule.

### Updating This Document

This document follows the same maintenance protocol as `copilot-instructions.md`:

- **Convention-changing PRs** update this file in the same diff.
- **Sprint retrospectives** review it for staleness.
- **"We got burned" moments** add lessons immediately.

The owner of the change owns the update. PR reviewers should flag purity-rule changes that lack a corresponding update here.

---

*Last updated: 2026-04-17*
