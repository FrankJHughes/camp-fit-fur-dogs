# Abstractions Contract

This guide explains the purpose and rules of the `Abstractions` folder in the Application layer. It defines what belongs here, what does not, and how other layers should interact with these types.

---

## 1. Purpose

The `Abstractions` folder defines the **public surface area** of the Application layer.  
It contains the types that other layers (API, Infrastructure, Tests) are allowed to reference.

These include:

- **Commands**
- **Queries**
- **Result/Response DTOs**
- **Service interfaces** (e.g., `ICurrentUserService`)
- **Dispatcher interfaces** (`ICommandDispatcher`, `IQueryDispatcher`)
- **Domain event abstractions** (`IDomainEventDispatcher`, `IDomainEventHandler<T>`)

Everything in Abstractions is intentionally stable and dependency‑safe.

---

## 2. Folder Structure

A typical structure looks like:

```
src/CampFitFurDogs.Application/Abstractions/
  Customers/
    CreateCustomerCommand.cs
    CreateCustomerResult.cs
  Dogs/
    RegisterDogCommand.cs
    RegisterDogResult.cs
    GetDogProfileQuery.cs
    GetDogProfileResult.cs

  ICommandDispatcher.cs
  IQueryDispatcher.cs
  ICurrentUserService.cs
  IDomainEventDispatcher.cs
  IDomainEventHandler.cs
```

Each feature has its own subfolder.

---

## 3. Rules

### 3.1 Commands and Queries Live in Abstractions  
Commands and queries **must not** live in slice implementation folders.

Correct:

```
Application/Abstractions/Dogs/RegisterDogCommand.cs
```

Incorrect:

```
Application/Dogs/RegisterDog/RegisterDogCommand.cs
```

### 3.2 API Depends Only on Abstractions  
Endpoints must reference:

- Commands
- Queries
- Result DTOs
- Dispatchers

They must **not** reference:

- Handlers  
- Validators  
- Application internals  

### 3.3 Infrastructure May Depend on Abstractions  
Infrastructure can reference:

- `ICurrentUserService`
- Dispatcher interfaces (if needed)
- Domain event abstractions

Infrastructure must **not** reference:

- Application handlers  
- Validators  
- Commands/queries directly (except for mapping or persistence boundaries)

### 3.4 Abstractions Must Not Depend on Application Internals  
Abstractions must remain pure:

- No references to handler implementations  
- No references to validators  
- No references to API or Infrastructure  
- No references to EF Core or ASP.NET  

---

## 4. Why This Matters

The Abstractions folder:

- Makes the Application layer’s public API explicit  
- Prevents accidental coupling between layers  
- Allows Application internals to evolve without breaking API or Infrastructure  
- Supports clean layering and purity rules  
- Enables guardrail tests to enforce architectural boundaries  

This is the backbone of the vertical slice architecture.

---

## 5. Contributor Guidelines

When adding a new feature:

1. **Define commands/queries** in `Abstractions/<Feature>/`.
2. **Define result types** (DTOs) in the same folder.
3. **Implement handlers** in `Application/<Feature>/Handlers/`.
4. **Implement validators** in `Application/<Feature>/Validators/`.
5. **Use only Abstractions** from API and Infrastructure.
6. **Do not reference internal handler types** from outside Application.

If a type is referenced across layers, it probably belongs in Abstractions.

