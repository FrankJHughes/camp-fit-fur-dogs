# Dependency Injection Architecture

This guide describes how dependency injection works across the Camp Fit Fur Dogs system.  
It defines the responsibilities of Frank as the DI orchestrator, the rules for auto‑registration, and how Application and Infrastructure participate in the DI pipeline.

The goal of the DI architecture is to provide:

- Predictable, rule‑driven service registration  
- Strict enforcement of architectural boundaries  
- Minimal manual DI wiring  
- Automatic registration of slice‑level services  
- Explicit registration of cross‑cutting services  
- Consistent behavior across all layers  

Frank provides the DI engine for the entire system.

---

# 1. Frank as the DI Orchestrator

Frank is responsible for all assembly scanning and auto‑registration.  
No other layer performs scanning.

Frank handles:

- Discovery of interfaces decorated with `[AutoRegister]`
- Discovery of implementing classes
- Validation of implementation counts
- Registration of services with the correct lifetime
- Registration of validators from all assemblies
- Discovery and application of EF Core configurations
- Enforcement of DI purity rules

In `Program.cs`:

```csharp
builder.Services.AddFrank([
    typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly
]);
```

This ensures all layers participate in DI, but only Frank performs scanning.

---

# 2. Auto‑Registration via `[AutoRegister]`

Interfaces intended for automatic registration must be decorated with:

```csharp
[AutoRegister(ServiceLifetime.Scoped)]
public interface IMyService { }
```

The attribute defines:

- **Lifetime** (`Singleton`, `Scoped`, `Transient`)
- **MinRegistrationCount**
- **MaxRegistrationCount**
- **RegisterConcreteType** (optional)

If the number of implementations violates the constraints, startup fails with a descriptive error.

This replaces:

- Scrutor
- Suffix‑based scanning
- Manual registration of slice services

---

# 3. Auto‑Registration Pipeline

Frank’s auto‑registration pipeline consists of four stages:

## 3.1 Scanner
Finds:

- All interfaces with `[AutoRegister]`
- All concrete classes implementing those interfaces

## 3.2 Planner
Builds a registration plan for each interface.

## 3.3 Validator
Ensures:

- Implementation count ≥ `MinRegistrationCount`
- Implementation count ≤ `MaxRegistrationCount`

Violations cause startup failure.

## 3.4 Registrar
Registers:

- `Interface → Implementation`
- Optionally `Implementation → Implementation`

## 3.5 Validator Registration
Frank registers validators from all assemblies:

```csharp
services.AddValidatorsFromAssembly(assembly);
```

This ensures validators from API, Application, and Infrastructure are all discovered.

---

# 4. Categories of Auto‑Registered Interfaces

The following interface categories participate in auto‑registration.

## 4.1 Repositories (Infrastructure)
Examples:

- `ICustomerRepository`
- `IDogRepository`

Repositories belong to Infrastructure.

## 4.2 Readers (Infrastructure)
Examples:

- `IGetDogProfileReader`
- `IListDogsByOwnerReader`
- `IFindCustomerByExternalIdReader`

Readers are Infrastructure implementations of Application abstractions.

## 4.3 Command and Query Dispatching (Frank.Abstractions)
- `ICommandDispatcher`
- `IQueryDispatcher`

## 4.4 Command and Query Handlers (Application)
- `ICommandHandler<TCommand>`
- `ICommandHandler<TCommand, TResponse>`
- `IQueryHandler<TQuery, TResponse>`

Handlers are auto‑registered and validated.

## 4.5 Domain Event Dispatching (Frank.Events)
- `IDomainEventDispatcher`
- `IDomainEventHandler<TEvent>`

## 4.6 Cross‑Cutting Services
Examples:

- `ICurrentUser`
- `IUnitOfWork`
- `IClock`
- `IIdentityResolver`

All are auto‑registered via `[AutoRegister]`.

---

# 5. EF Core Configuration Auto‑Discovery

Frank automatically discovers and applies:

- All `IEntityTypeConfiguration<T>` implementations
- All EF Core configurations in Infrastructure

Infrastructure does not manually apply configurations.

---

# 6. Application DI Responsibilities

`AddApplication()` registers **application‑level services** explicitly:

- Authentication pipeline behaviors
- Authentication callback orchestrators
- Strongly‑typed configuration (`OidcOptions`)
- Application‑level orchestrators

Application does **not** register:

- Handlers  
- Validators  
- Repositories  
- Readers  
- EF Core configurations  

These are handled by Frank.

---

# 7. Infrastructure DI Responsibilities

`AddInfrastructure()` registers **infrastructure‑level services** explicitly:

- `AppDbContext`
- `AddFrankEfCore<AppDbContext>()`
- External identity resolver
- HttpClient integrations
- Audit logging
- Hosting abstractions

Infrastructure does **not** register:

- Repositories  
- Readers  
- EF Core configurations  
- Handlers  
- Validators  

These are handled by Frank.

---

# 8. Contributor Guidelines

## 8.1 Use `[AutoRegister]` when:
- The service is slice‑specific
- The service is an interface with one or more implementations
- The service belongs in Domain, Application, or Infrastructure
- The service should be automatically registered

Example:

```csharp
[AutoRegister(ServiceLifetime.Scoped, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface IRegisterDogService { }
```

## 8.2 Use explicit registration when:
- The service is part of a pipeline (e.g., `IAuthCallbackStep`)
- The service is a DbContext
- The service is an HttpClient integration
- The service is cross‑cutting infrastructure

## 8.3 Do not:
- Register slice‑specific services manually
- Add Scrutor or suffix‑based scanning
- Add DI logic to Program.cs
- Add DI logic to slices
- Register handlers manually
- Register repositories manually

Frank owns all auto‑registration.

---

# 9. Relationship to the Shared Kernel

Frank provides:

- DI auto‑registration
- Validator scanning
- EF Core configuration scanning
- Hosting abstractions
- Domain primitives
- Domain abstractions

Frank must not depend on:

- Domain
- Application
- Infrastructure
- API

This preserves Shared Kernel purity.

---

# 10. Related Documents

- Shared Kernel Guide  
- Domain Events Architecture  
- Dispatcher Pipeline Guide  
- API Endpoint Purity Guide  
