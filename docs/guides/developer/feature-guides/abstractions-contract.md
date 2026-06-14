# Abstractions Contract

This guide explains the purpose and rules of the `Abstractions` folder in the Application layer.  
It defines what belongs here, what does not, and how other layers should interact with these types.

The Abstractions folder is the **public surface area** of the Application layer — the only part of Application that other layers may reference.

---

# 1. Purpose

The `Abstractions` folder defines the **stable, dependency‑safe API** of the Application layer.  
It contains the types that **API**, **Infrastructure**, and **Tests** are allowed to reference.

These include:

- **Commands**  
- **Queries**  
- **Result/Response DTOs**  
- **Reader interfaces** (e.g., `IGetDogProfileReader`)  
- **Service interfaces** (e.g., `ICurrentUserService`)  
- **Dispatcher interfaces** (`ICommandDispatcher`, `IQueryDispatcher`)  
- **Domain event abstractions** (`IDomainEventDispatcher`, `IDomainEventHandler<T>`)  
- **Cross‑cutting interfaces** (e.g., `IUnitOfWork`)  

Everything in Abstractions is intentionally **stable**, **pure**, and **safe to reference**.

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
  ICurrentUserService.cs
  IDomainEventDispatcher.cs
  IDomainEventHandler.cs
  IUnitOfWork.cs
````

Each feature has its own subfolder.

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

Endpoints must reference:

- Commands  
- Queries  
- Result DTOs  
- Dispatchers  

They must **not** reference:

- Handlers  
- Validators  
- Application internals  

This enforces **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**.

---

## 3.3 Infrastructure May Depend on Abstractions

Infrastructure can reference:

- `ICurrentUserService`  
- Dispatcher interfaces  
- Domain event abstractions  
- Reader interfaces (Infrastructure implements these)

Infrastructure must **not** reference:

- Application handlers  
- Validators  
- Commands/queries directly (except for mapping or persistence boundaries)

This enforces **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

## 3.4 Abstractions Must Not Depend on Application Internals

Abstractions must remain pure:

- No references to handler implementations  
- No references to validators  
- No references to API or Infrastructure  
- No references to EF Core or ASP.NET  
- No DI attributes  
- No business logic  

Abstractions define **contracts**, not behavior.

---

# 4. Why This Matters

The Abstractions folder:

- Makes the Application layer's public API explicit  
- Prevents accidental coupling between layers  
- Allows Application internals to evolve without breaking API or Infrastructure  
- Supports clean layering and purity rules  
- Enables guardrail tests to enforce architectural boundaries  
- Houses reader interfaces so query handlers depend on stable contracts, not Infrastructure (**ADR‑0021**)  

This is the backbone of the **vertical slice architecture**.

---

# 5. Contributor Guidelines

When adding a new feature:

1. **Define commands/queries** in `Abstractions/<Feature>/`.  
2. **Define result types** (DTOs) in the same folder.  
3. **Define reader interfaces** in the same folder (query slices only).  
4. **Implement handlers** in `Application/<Feature>/Handlers/`.  
5. **Implement validators** in `Application/<Feature>/Validators/`.  
6. **Implement readers** in `Infrastructure/<Feature>/` (query slices only).  
7. **Use only Abstractions** from API and Infrastructure.  
8. **Do not reference internal handler types** from outside Application.  

If a type is referenced across layers, it **belongs in Abstractions**.

---

# Related Documents

- **[Dispatcher Pipeline Guide](ca://s?q=Open_dispatcher_pipeline_guide)**  
- **[Domain Events Architecture](ca://s?q=Open_domain_events_guide)**  
- **[API Endpoint Purity Guide](ca://s?q=Generate_API_Endpoint_Purity_Guide)**  
- **[Architecture Governance](ca://s?q=Open_architecture_governance)**  
- **[Dependency Injection Architecture](ca://s?q=Open_dependency_injection_architecture)**  
