# Dependency Injection Architecture

This guide describes how dependency injection works across the entire Camp Fit Fur Dogs system.  
It explains the role of **Frank as the DI orchestrator**, how **attribute‑based auto‑registration** works, and how **Application** and **Infrastructure** participate in the DI pipeline.

This version is fully aligned with:

- The **new DI boundaries**  
- The **new auto‑registration engine**  
- The **new guardrail rules**  
- The **new test harness**  
- The **recent refactors** (CurrentUser simplification, removal of Scrutor, consolidation of DI responsibilities)

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
- Enforcing DI purity rules (no Scrutor, no suffix‑based scanning)  

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

This ensures **all layers participate in DI**, but **only Frank performs scanning**.

---

# 3. Auto‑Registration via `[AutoRegister]`

Interfaces that should be automatically registered must be decorated with:

```csharp
[AutoRegister(ServiceLifetime.Scoped)]
public interface IMyService { }
```

The attribute controls:

- **Lifetime** (`Singleton`, `Scoped`, `Transient`)  
- **MinRegistrationCount** (default: `0`)  
- **MaxRegistrationCount** (default: `int.MaxValue`)  
- **RegisterConcreteType** (optional)  

If the number of implementations falls outside the allowed range, startup fails with a detailed error.

This replaces:

- Scrutor  
- Suffix‑based scanning  
- Manual registration of slice services  

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

This ensures validators from **API**, **Application**, and **Infrastructure** are all discovered.

---

# 5. Categories of Auto‑Registered Interfaces (Aligned)

The following interface categories participate in auto‑registration:

## 5.1 Repositories (Infrastructure)
- `ICustomerRepository`  
- `IDogRepository`  
- `IRepository<T>`  

Repositories are **Infrastructure**, not Domain.

## 5.2 Readers (Infrastructure)
- `IGetDogProfileReader`  
- `IListDogsByOwnerReader`  
- `IFindCustomerByExternalIdReader`  

Readers are **Infrastructure**.  
Application depends on abstractions; Infrastructure provides implementations.

## 5.3 Command and Query Dispatching (Frank.Abstractions)
- `ICommandDispatcher`  
- `IQueryDispatcher`  

## 5.4 Command and Query Handlers (Application)
- `ICommandHandler<TCommand>`  
- `ICommandHandler<TCommand, TResponse>`  
- `IQueryHandler<TQuery, TResponse>`  

Handlers are auto‑registered and validated.

## 5.5 Domain Event Dispatching (Frank.Events)
- `IDomainEventDispatcher`  
- `IDomainEventHandler<TEvent>`  

## 5.6 Cross‑Cutting Services
- `ICurrentUser` (new name; replaces ICurrentUserService)  
- `IUnitOfWork`  
- `IClock`  
- `IIdentityResolver`  

All of these are auto‑registered via `[AutoRegister]`.

---

# 6. EF Core Configuration Auto‑Discovery

Frank provides EF Core helpers that:

- Discover all `IEntityTypeConfiguration<T>` implementations  
- Apply them automatically  
- Enforce DbContext guardrails  
- Ensure consistent EF Core configuration across slices  

Infrastructure does **not** manually apply configurations.

---

# 7. Application DI (Aligned)

`AddApplication()` registers **application‑level services** explicitly:

- Authentication pipeline steps  
- Authentication callback service  
- Strongly‑typed configuration (`OidcOptions`)  
- Application‑level orchestrators  

Application does **not** register:

- Handlers  
- Validators  
- Repositories  
- Readers  
- EF Core configurations  

These are handled by Frank’s auto‑registration.

---

# 8. Infrastructure DI (Aligned)

`AddInfrastructure()` registers **infrastructure‑level services** explicitly:

- `AppDbContext`  
- `AddFrankEfCore<AppDbContext>()`  
- External identity resolver  
- HttpClient integrations  
- Audit logging  
- Hosting provider abstractions  

Infrastructure does **not** register:

- Repositories  
- Readers  
- EF Core configurations  
- Handlers  
- Validators  

These are handled by Frank.

---

# 9. Contributor Guidelines (Aligned)

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
- Register handlers manually  
- Register repositories manually  

Frank owns all auto‑registration.

---

# 10. Relationship to Shared Kernel (Aligned)

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

This enforces **Shared Kernel purity**.

---

# 11. Related Documents

- **Shared Kernel Guide**  
- **Domain Events Architecture**  
- **Dispatcher Pipeline Guide**  
- **API Endpoint Purity Guide**
