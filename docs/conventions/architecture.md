# Architecture Conventions

This document defines the architectural layering, implementation patterns, hosting model, and cross‑cutting conventions for the Camp Fit Fur Dogs system.  
It describes **how the architecture is implemented**, not governance, policy, or security posture.

For architectural *rules*, see `architecture-governance.md`.  
For security posture, see `security-governance.md`.  
For operational rules, see `operations-governance.md`.

---

# 1. Layered Architecture (Implementation)

The backend follows a strict layered architecture enforced by guardrail tests.

```
Api → Application → Domain
Application → Infrastructure
All layers → Frank
```

## 1.1 Allowed Dependencies

- **Api** → Application, Domain (primitives only), Frank  
- **Application** → Domain, Frank  
- **Infrastructure** → Application, Domain, Frank  
- **Domain** → Frank only  
- **Frank** → no product dependencies  

## 1.2 Implementation Conventions

- Api contains **no business logic**  
- Application contains **use‑case orchestration only**  
- Domain contains **business rules**  
- Infrastructure contains **persistence and external integrations**  
- Frank contains **cross‑cutting primitives, abstractions, and DI infrastructure**  

Guardrail tests enforce these boundaries.

---

# 2. Dependency Injection Architecture (Implementation)

Dependency injection is implemented through **Frank’s auto‑registration engine**.

## 2.1 Auto‑Registration via `[AutoRegister]`

Interfaces intended for DI must be decorated with:

````  
[AutoRegister(ServiceLifetime.Scoped)]
public interface IMyService { }
````

Frank:

- Scans assemblies for attributed interfaces  
- Discovers implementing classes  
- Validates min/max implementation counts  
- Registers services with the correct lifetime  
- Optionally registers concrete types  
- Registers validators from all assemblies  
- Applies EF Core configurations  

Slices do **not** perform their own DI scanning.

## 2.2 Explicit Registration

Application and Infrastructure register only:

- Pipeline steps  
- DbContext  
- HttpClient integrations  
- Cross‑cutting services (e.g., audit logging)  

Slice‑specific services (repositories, readers, handlers) are **never** registered manually.

## 2.3 Program.cs

Program.cs must contain only:

````  
builder.Services.AddFrank([...]);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
````

No slice‑specific DI is allowed here.

---

# 3. CQRS Implementation Conventions

Commands and queries follow a consistent pipeline.

## 3.1 Commands

- Implement `ICommand<TResponse>`  
- Handled by `ICommandHandler<TCommand, TResponse>`  
- Handlers are auto‑registered via `[AutoRegister]`  
- Dispatched via `ICommandDispatcher`  
- Must not return domain entities  
- Must not perform queries  

## 3.2 Queries

- Implement `IQuery<TResponse>`  
- Handled by `IQueryHandler<TQuery, TResponse>`  
- Handlers are auto‑registered via `[AutoRegister]`  
- Dispatched via `IQueryDispatcher`  
- Must not mutate state  

## 3.3 Dispatcher Behavior

The dispatcher:

- Resolves validators  
- Runs validation  
- Throws on validation failure  
- Invokes handler  
- Ensures consistent pipeline behavior  

Handlers must **never** be invoked directly.

---

# 4. Domain Events Architecture (Implementation)

Domain events follow a strict pipeline:

- Raised inside aggregates  
- Collected by Application  
- Dispatched via `IDomainEventDispatcher`  
- Handled by `IDomainEventHandler<T>`  
- Handlers are auto‑registered via `[AutoRegister]`  

Domain events must not cross the Api boundary.

---

# 5. Domain Model Conventions

The domain model uses:

- **Value Objects** — immutable, equality by components  
- **Entities** — identity by Id  
- **Aggregate Roots** — consistency boundaries  
- **AggregateId** — value object wrapping a Guid  
- **Domain Events** — raised inside aggregates  

## 5.1 Domain Rules

- Domain must not depend on Application, Infrastructure, or Api  
- Business rules live inside aggregates, value objects, and domain services  
- Domain events must not cross the Api boundary  
- Domain types must not be returned from endpoints  

---

# 6. Repository & Persistence Conventions

Repositories provide persistence for aggregates:

- `GetByIdAsync`  
- `AddAsync`  
- `Update`  
- `Delete`  

`IUnitOfWork` coordinates saving changes.

## 6.1 Infrastructure Responsibilities

Infrastructure must:

- Implement repositories using EF Core  
- Implement Unit of Work using `DbContext`  
- Ensure `CommitAsync` is called after successful command handling  
- Never expose EF Core types to Application or Api  

Domain must not reference repositories.

---

# 7. EF Core Mapping Conventions

Infrastructure provides base configurations for aggregates.

- Aggregate root maps to a table  
- Id is the key and is **not** value‑generated  
- Domain events are ignored  
- Value objects are mapped as owned types  
- Navigation properties must be explicit  
- Migrations run only in CI/CD  

Tests must not apply migrations.

---

# 8. Frank Architecture

Frank contains cross‑cutting building blocks:

- CQRS abstractions  
- Validation pipeline integration  
- **DI auto‑registration engine**  
- Domain primitives (`ValueObject`, `Entity`, `AggregateRoot`, `AggregateId`)  
- Domain event abstractions and dispatcher  
- EF Core base classes  
- Endpoint discovery infrastructure  
- Hosting provider infrastructure  
- Authentication/session abstractions  
- HttpClient test seam  
- Audit logging abstractions  

## 8.1 Conventions

- Product layers must not reimplement Frank primitives  
- Frank is the only allowed cross‑layer dependency  
- Frank must remain product‑agnostic  

---

# 9. Hosting Provider Architecture (Implementation)

Hosting providers run **before DI is built** and configure the hosting environment.

## 9.1 Conventions

- Providers implement `IHostingProvider`  
- Providers expose `IsActive()` and `ConfigureAsync()`  
- Providers must be deterministic  
- Providers must validate required configuration  
- Providers must throw on misconfiguration  
- Providers must not silently skip configuration  
- Providers must not depend on product‑specific logic  

## 9.2 Provider Selection

- Providers are evaluated in order  
- The **first active provider wins**  
- All others are skipped  

## 9.3 Test Seam

Hosting providers must be testable via:

- Fake environment variable providers  
- Fake GitHub artifact providers  
- Fake HttpClient handlers  

Tests must not rely on real hosting environments.

---

# 10. Authentication & Session Architecture (Implementation)

Authentication uses OIDC (Auth0) and issues secure session cookies.

## 10.1 Conventions

- Login initiation endpoint redirects to OIDC provider  
- Callback endpoint exchanges code for tokens  
- Userinfo is fetched via HttpClient  
- Identity is resolved via `IIdentityResolver`  
- Session cookie is issued via `ISessionService`  
- Audit logging is performed via `IAuditLogger`  

## 10.2 Session Cookie Conventions

- Cookie contains opaque session ID  
- Cookie is HttpOnly  
- Cookie is Secure in production  
- Cookie is validated on each request  
- Cookie is not a JWT  
- Cookie does not contain PII  

---

# 11. Endpoint Architecture

Endpoints implement `IEndpoint` and define a `Map` method.

## 11.1 Conventions

- Endpoints must use CQRS dispatchers  
- Endpoints must not contain business logic  
- Endpoints must not return domain entities  
- Endpoints must use validation pipeline  
- Endpoints must use error‑shaping conventions  
- Endpoints must resolve identity via `ICurrentUserService`  

## 11.2 Discovery

- Frank.Api scans assemblies for `IEndpoint` implementations  
- Api assembly registers itself for discovery  
- All endpoints are mapped automatically at startup  

---

# 12. Test Seam Architecture

The system includes explicit test seams for deterministic testing.

## 12.1 HttpClient Test Seam

- All external HTTP calls must use named HttpClients  
- Tests replace HttpClient with FakeHttpMessageHandler  
- No real network calls are allowed in tests  

## 12.2 Hosting Provider Test Seam

- Environment variables are injected via test seam  
- GitHub artifacts are injected via test seam  
- Providers must be fully testable without real infrastructure  

## 12.3 Identity Resolver Test Seam

- Identity resolution is abstracted  
- Tests use FakeIdentityResolver  

## 12.4 Audit Logger Test Seam

- Audit logging is abstracted  
- Tests use FakeAuditLogger  

---

# 13. Frontend Architecture

The frontend mirrors backend aggregate grouping.

## 13.1 Structure

```
layer/aggregate/filename
```

## 13.2 Layers

- `api/` — server‑call functions  
- `components/` — presentational components  
- `lib/` — pure logic  
- `hooks/` — behavioral hooks  
- `app/` — Next.js routing  

## 13.3 Rules

- Slice subfolders appear only when an aggregate accumulates 10+ files  
- `test/` mirrors `src/` exactly  
- Shared infrastructure lives in `lib/api`, `lib/hooks`, `lib/components`  

---

# 14. Summary

This document codifies:

- Layered architecture implementation  
- Dependency injection architecture  
- CQRS implementation  
- Domain modeling conventions  
- Repository and EF Core conventions  
- Frank usage  
- Hosting provider architecture  
- Authentication/session architecture  
- Endpoint architecture  
- Test seam architecture  
- Frontend architectural structure  

All code must follow these conventions.
