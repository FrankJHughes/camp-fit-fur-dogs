# Dispatcher Pipeline Guide

This guide describes the **command and query dispatcher pipeline** in Camp Fit Fur Dogs: how requests flow, where validation happens, and how handlers are invoked.  
It belongs to the **architecture** category because it governs cross‑cutting behavior across all vertical slices.

---

# 1. Goals

- Centralize command and query execution  
- Apply cross‑cutting concerns consistently (validation, domain events, audit logging)  
- Keep API endpoints thin and free of business logic  
- Make handlers easy to test in isolation  
- Enforce strict layering and dependency direction  
- Ensure all business logic flows through the dispatcher pipeline  

---

# 2. Abstractions

## 2.1 ICommandDispatcher

````  
public interface ICommandDispatcher
{
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default);
}
````

## 2.2 IQueryDispatcher

````  
public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default);
}
````

These abstractions live in Application and are consumed by API endpoints and other application services.

Handlers, validators, and dispatchers are registered via Frank’s **[Auto‑Registration](ca://s?q=Open_dependency_injection_architecture)** system.

---

# 3. Command Pipeline

The command pipeline performs the following steps in strict order:

## 3.1 Validation

- Resolve all `IValidator<TCommand>` via auto‑registration  
- Execute validation  
- Fail fast on validation errors  
- Validation errors are shaped by API error boundary rules  

Validation is a pipeline concern — not a handler concern.

---

## 3.2 Handler Resolution

- Resolve `ICommandHandler<TCommand, TResult>` via `[AutoRegister]`  
- Ensure exactly one handler exists (enforced by Frank guardrails)  
- Handlers must not be invoked directly by endpoints  

---

## 3.3 Handler Execution

- Invoke `HandleAsync`  
- Handlers orchestrate domain behavior  
- Handlers must not contain HTTP, Infrastructure, or UI logic  
- Handlers must not raise domain events directly  

---

## 3.4 Persistence (Unit of Work)

- Handlers inject `IUnitOfWork`  
- Repositories stage changes on the DbContext change tracker  
- Handlers call `CommitAsync` after domain mutations  
- DbContext is never flushed directly  
- Unit of Work dispatches domain events after commit  
- See **ADR‑0017** for full persistence semantics  

This enforces **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

## 3.5 Domain Events

- Aggregates raise domain events internally  
- Unit of Work collects domain events  
- `IDomainEventDispatcher` dispatches them after commit  
- Domain event handlers are auto‑registered via `[AutoRegister]`  
- Domain events must not cross the API boundary  

This enforces **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**.

---

## 3.6 Result

- The handler returns a DTO or primitive  
- The dispatcher returns it to the API endpoint  
- Endpoints shape the HTTP response  

Commands must not return domain entities.

---

# 4. Query Pipeline

The query pipeline performs:

## 4.1 Validation

- Resolve all `IValidator<TQuery>`  
- Execute validation  

Queries must not mutate state.

---

## 4.2 Handler Execution

- Resolve `IQueryHandler<TQuery, TResult>` via `[AutoRegister]`  
- Invoke `HandleAsync`  
- Queries may use read‑only repositories or readers  

---

## 4.3 Result

- Return DTO or primitive  
- Queries must not return domain entities  

Queries generally do not raise domain events.

---

# 5. Handlers

Handlers implement:

````  
public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface IQueryHandler<in TQuery, TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
````

## Handler Conventions

- Class names end with `Handler`  
- Handlers live in Application slice folders  
- Handlers are auto‑registered via `[AutoRegister]`  
- Handlers must not depend on Infrastructure types  
- Handlers must not depend on API types  
- Handlers must not perform validation manually  
- Handlers must not raise domain events directly  

This enforces **[Code Conventions](ca://s?q=Open_code_conventions)**.

---

# 6. Validation

Validators implement:

````  
public interface IValidator<in T>
{
    Task ValidateAsync(T instance, CancellationToken cancellationToken = default);
}
````

## Validator Conventions

- Class names end with `Validator`  
- Validators live in Application slice folders  
- Validators are auto‑registered via `[AutoRegister]`  
- Validators must not contain business logic  
- Validators must not depend on Infrastructure  

Validation is part of the dispatcher pipeline — not the handler.

---

# 7. Purity Rules

## 7.1 API Endpoint Purity

Endpoints must:

- Call dispatchers, not handlers  
- Map DTOs only  
- Perform shape validation only  
- Never contain business logic  

See **[API Endpoint Purity Guide](ca://s?q=Generate_API_Endpoint_Purity_Guide)**.

---

## 7.2 Handler Purity

Handlers must:

- Contain no HTTP logic  
- Contain no Infrastructure logic  
- Contain no UI logic  
- Contain no domain event dispatching logic  
- Use repositories and domain services only  

---

## 7.3 Domain Purity

Domain must:

- Raise domain events internally  
- Contain business rules  
- Contain invariants  
- Never depend on Application or Infrastructure  

See **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

# 8. Contributor Guidelines

When adding a new command or query:

1. Define the request type in Application Abstractions  
2. Implement a handler in the corresponding slice  
3. Add validators if needed  
4. Inject `IUnitOfWork` into command handlers  
5. Call `CommitAsync` after repository operations  
6. Use `ICommandDispatcher` / `IQueryDispatcher` from API endpoints  
7. Do not bypass the dispatcher pipeline  
8. Do not reference Infrastructure or API types  
9. Ensure domain events are raised inside aggregates only  
10. Ensure the handler is auto‑registered via `[AutoRegister]`  

If a handler grows beyond ~30 lines, logic is leaking into the wrong layer.

---

# Related Documents

- **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**  
- **[Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)**  
- **[Authentication Architecture](ca://s?q=Generate_Authentication_Architecture_Guide)**  
- **[Session Management](ca://s?q=Generate_Session_Management_Guide)**  
- **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**  
- **[Architecture Governance](ca://s?q=Open_architecture_governance)**  
