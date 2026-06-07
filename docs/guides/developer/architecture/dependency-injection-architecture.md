# Dependency Injection Architecture

This guide describes how dependency injection works across the entire Camp Fit Fur Dogs system.  
It explains the role of Frank as the DI orchestrator, how attribute‑based auto‑registration works, and how Application and Infrastructure participate in the DI pipeline.

---

# 1. Purpose

The DI architecture ensures:

- Predictable, rule‑driven service registration  
- Strict enforcement of architectural boundaries  
- Minimal manual DI wiring in slices  
- Automatic registration of slice‑level services  
- Explicit registration of cross‑cutting services  
- Consistent behavior across Domain, Application, and Infrastructure  

The system is built around **Frank’s auto‑registration engine**, which performs attribute‑driven scanning and validation.

---

# 2. Frank as the DI Orchestrator

Frank provides the **core DI infrastructure** for the entire application.  
It is responsible for:

- Discovering interfaces decorated with `[AutoRegister]`  
- Discovering implementing classes  
- Validating min/max implementation counts  
- Registering services with the correct lifetime  
- Registering validators from all assemblies  
- Applying EF Core configurations  
- Providing hosting and environment abstractions  

Frank is the **only layer** that performs assembly scanning.

In `Program.cs`:

```csharp
builder.Services.AddFrank([
    typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Infrastructure.AssemblyMarker).Assembly,
    typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly // request dto validators
]);
```

---

# 3. Auto‑Registration via `[AutoRegister]`

Interfaces that should be automatically registered must be decorated with:

```csharp
[AutoRegister(ServiceLifetime.Scoped)]
public interface IMyService
{
}
```

The attribute controls:

- **Lifetime** (`Singleton`, `Scoped`, `Transient`)  
- **MinRegistrationCount** (default: `0`)  
- **MaxRegistrationCount** (default: `int.MaxValue`)  
- **RegisterConcreteType** (optional)  

If the number of implementations falls outside the allowed range, startup fails with a detailed error.

---

# 4. How Auto‑Registration Works

Frank’s auto‑registration pipeline consists of:

## 4.1 Scanner  
Finds:

- All interfaces with `[AutoRegister]`  
- All concrete classes implementing those interfaces  

## 4.2 Planner  
Builds a `Plan` for each `(interface → implementations)` group.

## 4.3 Validator  
Checks:

- `MinRegistrationCount`  
- `MaxRegistrationCount`  

Violations cause startup failure.

## 4.4 Registrar  
Registers:

- `ImplementedInterface → ImplementingClass`  
- Optionally `ImplementingClass → ImplementingClass`  

## 4.5 FluentValidation Integration  
Frank also registers validators via:

```csharp
services.AddValidatorsFromAssembly(assembly);
```

---

# 5. Categories of Auto‑Registered Interfaces

The following interface categories participate in auto‑registration:

## 5.1 Repositories (Domain)
- `ICustomerRepository`  
- `IDogRepository`  
- `IRepository<T>`  

## 5.2 Readers (Application Abstractions)
- `IGetDogProfileReader`  
- `IListDogsByOwnerReader`  
- `IFindCustomerByExternalIdReader`  

## 5.3 Command and Query Dispatching (Frank.Abstractions)
- `ICommandDispatcher`  
- `IQueryDispatcher`  

## 5.4 Command and Query Handlers
- `ICommandHandler<TCommand>`  
- `ICommandHandler<TCommand, TResponse>`  
- `IQueryHandler<TQuery, TResponse>`  

## 5.5 Domain Event Dispatching (Frank.Events)
- `IDomainEventDispatcher`  
- `IDomainEventHandler<TEvent>`  

## 5.6 Cross‑Cutting Services
- `ICurrentUserService`  
- `IUnitOfWork`  

All of these are auto‑registered via `[AutoRegister]`.

---

# 6. EF Core Configuration Auto‑Discovery

Frank provides EF Core helpers that:

- Discover all `IEntityTypeConfiguration<T>` implementations  
- Apply them automatically  
- Enforce DbContext guardrails  

This ensures consistent EF Core configuration across slices.

---

# 7. Application DI

`AddApplication()` registers **application‑level services** explicitly:

- Authentication pipeline steps  
- Authentication callback service  
- Strongly‑typed configuration (`OidcOptions`)  

Application does **not** register:

- Handlers  
- Validators  
- Repositories  
- Readers  

These are handled by Frank’s auto‑registration.

---

# 8. Infrastructure DI

`AddInfrastructure()` registers **infrastructure‑level services** explicitly:

- `AppDbContext`  
- `AddFrankEfCore<AppDbContext>()`  
- External identity resolver  
- HttpClient integrations  
- Audit logging  

Infrastructure does **not** register:

- Repositories  
- Readers  
- EF Core configurations  

These are handled by Frank.

---

# 9. Contributor Guidelines

When adding a new service:

## 9.1 Use `[AutoRegister]` when:
- The service is slice‑specific  
- The service is an interface with one or more implementations  
- The service belongs in Domain, Application, or Infrastructure  
- The service should be automatically registered  

Example:

```csharp
[AutoRegister(ServiceLifetime.Scoped, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface IRegisterDogService { }
```

## 9.2 Use explicit registration when:
- The service is part of a pipeline (e.g., `IAuthCallbackStep`)  
- The service is a DbContext  
- The service is an HttpClient integration  
- The service is cross‑cutting infrastructure  

## 9.3 Do NOT:
- Register slice‑specific services manually  
- Add Scrutor or suffix‑based scanning  
- Add DI logic to Program.cs  
- Add DI logic to slices  

Frank owns all auto‑registration.

---

# 10. Relationship to Shared Kernel

Frank provides:

- DI auto‑registration  
- Validator scanning  
- EF Core configuration scanning  
- Hosting abstractions  
- Domain primitives  
- Domain abstractions  

Application and Infrastructure rely on Frank for:

- Automatic registration of slice services  
- Enforcement of DI rules  
- Consistent cross‑cutting behavior  

Frank must not depend on:

- Domain  
- Application  
- Infrastructure  
- API  

---

# 11. Related Documents

- **[Shared Kernel](ca://s?q=Generate_Shared_Kernel_Guide)**  
- **[Domain Events Architecture](ca://s?q=Generate_Domain_Events_Architecture_Guide)**  
- **[Dispatcher Pipeline](ca://s?q=Generate_Dispatcher_Pipeline_Guide)**  
- **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**
