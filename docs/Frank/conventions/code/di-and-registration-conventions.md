# Frank – Conventions – Code – DI and Registration Conventions

Frank provides cross‑cutting dependency injection behaviors that products consume.  
Frank must remain **product‑agnostic** and must never reference CampFitFurDogs code.

These conventions define how **discovery**, **registration**, and **DI orchestration** operate across the platform.

---

# Registration

Frank registers governed types such as:

- CQRS handlers  
- Validators  
- Endpoints  
- Hosting providers  
- Test seams  

Registration is **attribute‑driven** (via `[Registration]`) and **discovery‑rule‑driven** (via `DiscoveryOptions`).

Interfaces must explicitly opt in using `RegistrationAttribute`:

```csharp
[Registration(ServiceLifetime.Scoped, RegisterConcreteType = true)]
public interface ICommandHandler<TCommand> { ... }
```

Implementations do **not** require attributes.

Products must **not** manually register any type that Frank registers through the Registration Engine.

---

# Unified Discovery & Registration

Frank uses a unified pipeline:

```
DiscoveryOptions → Scanner → Planner → Validator → Registrar → DI Container
```

### DiscoveryOptions  
Defines **which interfaces** and **which implementations** are included:

- Inclusion predicates for interfaces  
- Exclusion predicates for interfaces  
- Inclusion predicates for implementations  
- Exclusion predicates for implementations  

Nothing is included unless explicitly included via `IncludeInterface` / `IncludeImplementation`.

### Scanner  
Inspects assemblies and matches interfaces to implementations using:

```csharp
Scanner.Scan(IEnumerable<Assembly> assemblies, DiscoveryOptions options)
```

### Orchestrator  
Coordinates the full pipeline:

```csharp
Orchestrator.Orchestrate(
    IServiceCollection services,
    IEnumerable<Assembly> assemblies,
    DiscoveryOptions options)
```

### Product Responsibilities  
Products must provide:

- **Assemblies to scan**  
- **DiscoveryOptions rules** (what to include/exclude)

Frank handles everything else.

All Frank subsystems (Command, Query, Event, Exception, Endpoint, Hosting) follow this model.

---

# Dispatcher Enforcement

Frank enforces strict dispatcher usage:

- All **commands** must go through `ICommandDispatcher`.  
- All **queries** must go through `IQueryDispatcher`.  
- Handlers must **never** be invoked directly.  
- Validators must be **discovered automatically** and must not be manually registered.

This ensures consistent pipeline behavior, validation, logging, and observability.

---

# Prohibitions

Frank must not:

- depend on product assemblies  
- register product services  
- reference product domain, application, or infrastructure layers  

Products must not:

- bypass Frank’s dispatchers  
- manually register handlers, validators, or any governed type  
- override or bypass the Registration Engine  

These boundaries preserve platform integrity and prevent cross‑layer coupling.

---
