# Abstractions Contract  
**Aligned With Vertical Slice Architecture, Exclusive OIDC Authentication, and De‑featured Local Identity**

The `Abstractions` folder defines the **public surface area** of the Application layer.  
It is the only part of Application that **API**, **Infrastructure**, and **Tests** may reference.

Abstractions contain **contracts**, not behavior.  
They define the stable API of the Application layer.

---

# 1. Purpose

The `Abstractions` folder provides a **stable, dependency‑safe API** for all other layers.  
It contains the types that external layers are allowed to reference:

- **Commands**  
- **Queries**  
- **Result/Response DTOs**  
- **Reader interfaces** (e.g., `IGetDogProfileReader`)  
- **Service interfaces** (e.g., `ICurrentUser`)  
- **Dispatcher interfaces** (`ICommandDispatcher`, `IQueryDispatcher`)  
- **Domain event abstractions** (`IDomainEventDispatcher`, `IDomainEventHandler<T>`)  
- **Cross‑cutting interfaces** (`IUnitOfWork`)  

Everything in Abstractions is intentionally:

- **Pure**  
- **Stable**  
- **Safe to reference**  
- **Free of implementation details**  

Abstractions define **what** the Application layer exposes — not **how** it works.

---

# 2. Folder Structure

A typical structure looks like:

````text
src/CampFitFurDogs.Application/Abstractions/
  Customers/
    CreateCustomerCommand.cs
    CreateCustomerResult.cs

  Dogs/
    RegisterDogCommand.cs
    RegisterDogResult.cs
    GetDogProfileQuery.cs
    GetDogProfileResult.cs
    IGetDogProfileReader.cs

  ICommandDispatcher.cs
  IQueryDispatcher.cs
  ICurrentUser.cs
  IDomainEventDispatcher.cs
  IDomainEventHandler.cs
  IUnitOfWork.cs
````

Each vertical slice has its own subfolder.

---

# 3. Rules

## 3.1 Commands and Queries Live in Abstractions

Commands and queries **must not** live in slice implementation folders.

**Correct:**

````text
Application/Abstractions/Dogs/RegisterDogCommand.cs
````

**Incorrect:**

````text
Application/Dogs/RegisterDog/RegisterDogCommand.cs
````

Commands and queries define the **public API** of a slice — they must be stable and discoverable.

---

## 3.2 API Depends Only on Abstractions

API endpoints may reference:

- Commands  
- Queries  
- Result DTOs  
- Dispatchers  

API endpoints must **not** reference:

- Handlers  
- Validators  
- Application internals  
- Domain entities  
- Infrastructure types  

This enforces **API Endpoint Purity**.

---

## 3.3 Infrastructure May Depend on Abstractions

Infrastructure may reference:

- `ICurrentUser`  
- Dispatcher interfaces  
- Domain event abstractions  
- Reader interfaces (Infrastructure implements these)

Infrastructure must **not** reference:

- Application handlers  
- Validators  
- Commands/queries directly (except for mapping boundaries)  
- Any Application implementation details  

This enforces **Architecture Governance**.

---

## 3.4 Abstractions Must Not Depend on Application Internals

Abstractions must remain pure:

- No references to handler implementations  
- No references to validators  
- No references to API or Infrastructure  
- No references to EF Core or ASP.NET  
- No DI attributes  
- No business logic  
- No pipeline logic  
- No session logic  
- No OIDC logic  

Abstractions define **contracts**, not behavior.

---

# 4. Why This Matters

The Abstractions folder:

- Makes the Application layer’s public API explicit  
- Prevents accidental coupling between layers  
- Allows Application internals to evolve safely  
- Supports clean layering and purity rules  
- Enables guardrail tests to enforce architectural boundaries  
- Houses reader interfaces so query handlers depend on stable contracts, not Infrastructure (**ADR‑0021**)  
- Ensures vertical slices expose only their **command/query API**, not their internals  

Abstractions are the backbone of the **vertical slice architecture**.

---

# 5. Contributor Guidelines

When adding a new vertical slice:

1. **Define commands/queries** in `Abstractions/<Feature>/`.  
2. **Define result types** (DTOs) in the same folder.  
3. **Define reader interfaces** in the same folder (query slices only).  
4. **Implement handlers** in `Application/<Feature>/Handlers/`.  
5. **Implement validators** in `Application/<Feature>/Validators/`.  
6. **Implement readers** in `Infrastructure/<Feature>/` (query slices only).  
7. **Use only Abstractions** from API and Infrastructure.  
8. **Do not reference internal handler types** from outside Application.  
9. If a type is referenced across layers, it **belongs in Abstractions**.  

Vertical slices expose **commands, queries, and results** — nothing else.

---

# Related Documents

- **[Dispatcher Pipeline Guide](ca://s?q=Open_dispatcher_pipeline_guide)**  
- **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**  
- **[API Endpoint Purity Guide](ca://s?q=Generate_API_Endpoint_Purity_Guide)**  
- **[Architecture Governance](ca://s?q=Open_architecture_governance)**  
- **[Dependency Injection Architecture](ca://s?q=Open_dependency_injection_architecture)**  
