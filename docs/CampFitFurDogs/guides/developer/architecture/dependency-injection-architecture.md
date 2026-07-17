
# Guides — Developer — Dependency Injection Architecture Guide  
Authoritative companion to ADR‑0002 (Layered Architecture)

This guide describes how dependency injection works across the Camp Fit Fur Dogs system under the **current Frank DI architecture**.

Frank provides the **Registration Engine**, which governs how services are discovered, validated, and registered.  
Application and Infrastructure participate in DI by providing implementations, but **Frank orchestrates the registration pipeline**.

The goals of the DI architecture are:

- Predictable, rule‑driven service registration  
- Strict enforcement of architectural boundaries  
- Minimal manual DI wiring  
- Deterministic discovery and validation  
- Explicit, capability‑driven registration  
- Consistent behavior across all layers  

Frank provides the DI engine for the entire system.

---

# 1. Frank as the DI Orchestrator

Frank is responsible for **all governed DI registration**.  
No other layer performs scanning or auto‑registration.

Frank handles:

- Discovery of governed interfaces (via `DiscoveryOptions.IncludeInterface`)  
- Discovery of implementing classes (via `IncludeImplementation`)  
- Validation of implementation counts  
- Registration of services with the correct lifetime  
- Registration of validators from all assemblies  
- Discovery and application of EF Core configurations  
- Enforcement of DI purity rules  

The **only** entry point into the DI engine is:

```csharp
Orchestrator.Orchestrate(services, assemblies, options);
```

Frank does **not** auto‑scan assemblies.  
Frank does **not** auto‑register slice services.  
Frank does **not** use `[AutoRegister]`.

All discovery is **explicit** and **capability‑driven**.

---

# 2. The `[Registration]` Attribute

Interfaces intended for governed registration must be decorated with:

```csharp
[Registration(ServiceLifetime.Scoped)]
public interface IMyService { }
```

The attribute defines:

- **Lifetime** (`Singleton`, `Scoped`, `Transient`)  
- **MinRegistrationCount**  
- **MaxRegistrationCount**  
- **RegisterConcreteType** (optional)  

The attribute does **not** cause auto‑registration.  
It is **metadata only**.

The interface is only registered if the capability’s `DiscoveryOptions` selects it.

If the number of implementations violates the constraints, startup fails with a descriptive error.

---

# 3. Registration Engine Pipeline

Frank’s Registration Engine consists of five stages:

## 3.1 Scanner
Finds governed interfaces and implementing classes based on `DiscoveryOptions`.

## 3.2 Planner
Builds a registration plan for each governed interface.

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

Validator discovery is deterministic and observable.

---

# 4. Capability‑Driven Registration

Each capability defines its own DI rules using `DiscoveryOptions`.

Examples:

### Query Dispatcher
```csharp
options.IncludeInterface(iface =>
    iface.IsGenericType &&
    iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

options.IncludeImplementation(impl =>
    impl.ImplementedInterfaces.Any(i =>
        i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));
```

### Endpoint Engine
```csharp
options.IncludeInterface(iface => iface == typeof(IEndpoint));
options.IncludeImplementation(impl => typeof(IEndpoint).IsAssignableFrom(impl));
```

### Domain Event Dispatcher
```csharp
options.IncludeInterface(iface =>
    iface.IsGenericType &&
    iface.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

options.IncludeImplementation(impl =>
    impl.ImplementedInterfaces.Any(i =>
        i.IsGenericType &&
        i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)));
```

Capabilities decide:

- What interfaces are governed  
- What implementations are eligible  
- What lifetimes apply  
- What constraints apply  

Frank executes the pipeline.

---

# 5. EF Core Configuration Discovery

Frank automatically discovers and applies:

- All `IEntityTypeConfiguration<T>` implementations  
- All EF Core configurations in Infrastructure assemblies  

Infrastructure does **not** manually apply configurations.

This ensures:

- Deterministic EF Core setup  
- No configuration drift  
- No accidental omission  
- Full observability of EF Core configuration

---

# 6. Application DI Responsibilities

`AddApplication()` registers **application‑level services** explicitly:

- Authentication pipeline behaviors  
- Authentication callback orchestrators  
- Strongly‑typed configuration (`OidcOptions`)  
- Application‑level orchestrators  
- Application‑level helpers  

Application does **not** register:

- Handlers  
- Validators  
- Repositories  
- Readers  
- EF Core configurations  

These are governed by Frank’s Registration Engine.

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

These are governed by Frank’s Registration Engine.

---

# 8. Contributor Guidelines

## 8.1 Use `[Registration]` when:
- The interface is governed by a capability  
- The interface has one or more implementations  
- The interface belongs in Domain, Application, or Infrastructure  
- The capability will select it via `DiscoveryOptions`

Example:

```csharp
[Registration(ServiceLifetime.Scoped, MinRegistrationCount = 1, MaxRegistrationCount = 1)]
public interface IRegisterDogService { }
```

## 8.2 Use explicit registration when:
- The service is part of a pipeline (e.g., `IAuthCallbackStep`)  
- The service is a DbContext  
- The service is an HttpClient integration  
- The service is cross‑cutting infrastructure  

## 8.3 Do not:
- Register governed interfaces manually  
- Add Scrutor or suffix‑based scanning  
- Add DI logic to Program.cs  
- Add DI logic to slices  
- Register handlers manually  
- Register repositories manually  

Frank owns governed registration.

---

# 9. Relationship to the Shared Kernel

Frank provides:

- Registration Engine  
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
- Architecture Purity Rules  
- Frank Governance  

