# US-108 — Foundation Extraction

## Intent

As a **developer**, I want the reusable architectural foundation (CQRS pipelineThe file export hit a wall — I couldn't get it to deliver downloadable files. Here are all four files inline for you to copy. I've used four-backtick outer fences on the story files since they contain inner code blocks.

---

## 1. `US-108-foundation-extraction.md`

````markdown
# US-108 — Foundation Extraction

## Intent

As a **developer**, I want the reusable architectural foundation (CQRS pipeline, DI conventions, EF base classes, endpoint auto-discovery) separated from product-specific code into dedicated SharedKernel projects,, DI conventions, EF base classes, endpoint auto-discovery) separated from product-specific code into dedicated SharedKernel projects, with self-enforcing boundary guardrails — so that a new product can reference the foundation without inheriting Camp Fit Fur Dogs business logic, boundary violations are caught at both test time and runtime with self-enforcing boundary guardrails — so that a new product can reference the foundation without inheriting Camp Fit Fur Dogs business logic, boundary violations are caught at both without writing product-specific guardrail tests, and product projects contain only product code.

## Value

- **Reusable chassis** — test time and runtime without writing product-specific guardrail tests, and product projects contain only product code.

## Value

- **Reusable chassis** — a new product references three SharedKernel projects and gets DDD building blocks, CQRS pipeline, DI scanning, EF base configuration, and endpoint auto-discovery out a new product references three SharedKernel projects and gets DDD building blocks, CQRS pipeline, DI scanning, EF base configuration, and endpoint auto-discovery out of the box.
- **Clean product projects** — Application, Infrastructure, and Api contain only Camp Fit Fur Dogs handlers, readers, repositories, endpoints, and configuration of the box.
- **Clean product projects** — Application, Infrastructure, and Api contain only Camp Fit Fur Dogs handlers, readers, repositories, endpoints, and configuration. No framework noise.
- **Portable foundation** — SharedKernel stays ORM-agnostic and host-agnostic. EF Core and ASP.NET Core dependencies. No framework noise.
- **Portable foundation** — SharedKernel stays ORM-agnostic and host-agnostic. EF Core and ASP.NET Core dependencies are isolated in companion projects that a different product can swap.
- **Self-enforcing boundaries** — SharedKernel.Testing provides inheritable guardrails and `AddSharedKernel()` validates boundaries at startup. A new product gets are isolated in companion projects that a different product can swap.
- **Self-enforcing boundaries** — SharedKernel.Testing provides inheritable guardrails and `AddSharedKernel()` validates boundaries at startup. A new product gets 18 architecture tests by providing 4 assembly markers.
- **Onboarding clarity** — "SharedKernel is 18 architecture tests by providing 4 assembly markers.
- **Onboarding clarity** — "SharedKernel is the framework. Everything else is the product." One sentence, no ambiguity.

## Problem

Today, foundation code and product code are mixed in the same projects the framework. Everything else is the product." One sentence, no ambiguity.

## Problem

Today, foundation code and product code are mixed in the same projects:

| Project | Foundation files | Product files | Mixed? |
|---------|-----------------|---------------|--------|
| SharedKernel | 6 | 0 | Clean |
| Domain:

| Project | Foundation files | Product files | Mixed? |
|---------|-----------------|---------------|--------|
| SharedKernel | 6 | 0 | Clean |
| Domain | 0 | 13 | Clean |
| Application | 16 | 10 | Mixed |
| Infrastructure | 3 | 10+ migrations | Mixed |
| Api | 2 | 8 | Mixed |

A developer | 0 | 13 | Clean |
| Application | 16 | 10 | Mixed |
| Infrastructure | 3 | 10+ migrations | Mixed |
| Api | 2 | 8 | Mixed |

A developer building a new product on this architecture would have to copy Application and Infrastructure, then delete Camp Fit Fur Dogs-specific files. The foundation types (dispatchers, CQRS interfaces, EF base configuration) are indistinguishable from product types in building a new product on this architecture would have to copy Application and Infrastructure, then delete Camp Fit Fur Dogs-specific files. The foundation types (dispatchers, CQRS interfaces, EF base configuration) are indistinguishable from product types in the same project.

Additionally, architecture guardrails are product-specific test classes. A new product built on this foundation would have to rewrite all 15 guardrail tests from the same project.

Additionally, architecture guardrails are product-specific test classes. A new product built on this foundation would have to rewrite all 15 guardrail tests from scratch.

## Solution

### Part A: Expand SharedKernel (Tier 0-2 — pure C scratch.

## Solution

### Part A: Expand SharedKernel (Tier 0-2 — pure C# + M.E.DI.Abstractions + Scrutor)

Move all product-agnostic CQRS interfaces# + M.E.DI.Abstractions + Scrutor)

Move all product-agnostic CQRS interfaces, dispatchers, and DI conventions from Application into SharedKernel:

```
src/CampFitFurDogs.SharedKernel/
  Domain/                , dispatchers, and DI conventions from Application into SharedKernel:

```
src/CampFitFurDogs.SharedKernel/
  Domain/                              (existing)
    Entity.cs
    AggregateRoot.cs
    ValueObject.cs
    IDomainEvent.cs
    IRepository.cs
  Application/                         (NEW subfolder)
    ICommand.cs
    ICommandHandler.cs
    ICommandDispatcher.cs
    IQuery.cs
    IQueryHandler.cs
    IQueryDispatcher.cs
    IDomainEventDisp              (existing)
    Entity.cs
    AggregateRoot.cs
    ValueObject.cs
    IDomainEvent.cs
    IRepository.cs
  Application/                         (NEW subfatcher.cs
    IDomainEventHandler.cs
    IUnitOfWork.cs
    ICurrentUserService.cs
    CommandDispatcher.cs
    QueryDispatcher.cs
    DomainEventDispatcher.cs
    DependencyInjection.cs             (foundation scanningolder)
    ICommand.cs
    ICommandHandler.cs
    ICommandDispatcher.cs
    IQuery.cs
    IQueryHandler.cs
    IQueryDispatcher.cs
    IDomainEventDispatcher.cs
    IDomainEventHandler.cs
    IUnitOfWork.cs
    ICurrentUserService.cs
    CommandDispatcher.cs
    QueryDispatcher.cs
    DomainEventDispatcher.cs
    DependencyInjection.cs             (foundation scanning half)
    DomainEventRegistration.cs
    AssemblyMarker.cs
```

New dependencies for SharedKernel: `Microsoft.Extensions.DependencyInjection.Abstractions`, `Scrutor`.

### Part B: Create SharedKernel.Infrastructure (Tier 3 — EF Core)

```
src/CampFitFurDogs.SharedKernel.Infrastructure/
  AggregateConfiguration.cs            (from Infrastructure) half)
    DomainEventRegistration.cs
    AssemblyMarker.cs
```

New dependencies for SharedKernel: `Microsoft.Extensions.DependencyInjection.Abstractions`, `Scrutor`.

### Part B: Create SharedKernel.Infrastructure (Tier 3 — EF Core)

```
src/CampFitFurDogs.SharedKernel.Infrastructure/
  AggregateConfiguration.cs            (from Infrastructure)
  EfUnitOfWork.cs                      (from Infrastructure)
```

Dependencies: `Microsoft.EntityFrameworkCore`. References: `SharedKernel`.

### Part C: Create SharedKernel.Api (Tier 4 — ASP.NET Core routing)

```
src/CampFitFurDogs.SharedKernel.Api/
  IEndpoint.cs                         (from Api)
  Endpoints.cs                         (from Api)
  EfUnitOfWork.cs                      (from Infrastructure)
```

Dependencies: `Microsoft.EntityFrameworkCore`. References: `SharedKernel`.

### Part C: Create SharedKernel.Api (Tier 4 — ASP.NET Core routing)

```
src/CampFitFurDogs.SharedK
```

Dependencies: `Microsoft.AspNetCore.Routing.Abstractions`. References: none.

### Part D: Create SharedKernel.Testing

SharedKernel ships an abstract test base class that products inheriternel.Api/
  IEndpoint.cs                         (from Api)
  Endpoints.cs                         (from Api)
```

Dependencies: `Microsoft.AspNetCore.Routing.Abstractions`. References: none.

### Part D: Create SharedKernel.Testing

SharedKernel ships an abstract test base class that products inherit. Four assembly markers, 18 guardrails, zero per-product test logic:

```csharp
public abstract class CleanArchitectureGuardrails
{
    protected. Four assembly markers, 18 guardrails, zero per-product test logic:

```csharp
public abstract class CleanArchitectureGuardrails
{
    protected abstract Assembly DomainAssembly { get; }
    protected abstract Assembly ApplicationAssembly { get; }
    protected abstract Assembly InfrastructureAssembly { get; }
    protected abstract Assembly ApiAssembly { get; }

    [Fact]
    public void Domain_must_not_reference abstract Assembly DomainAssembly { get; }
    protected abstract Assembly ApplicationAssembly { get; }
    protected abstract Assembly InfrastructureAssembly { get; }
    protected abstract Assembly ApiAssembly { get; }

    [Fact]
    public void Domain_must_not_reference_application() { /* ... */ }

    [Fact]
    public void Domain_must_not_reference_infrastructure() { /* ... */ }

    [Fact]
    public void Application_must_not_reference_infrastructure() { /* ... */ }

    // ... all 18 guardrails including_application() { /* ... */ }

    [Fact]
    public void Domain_must_not_reference_infrastructure() { /* ... */ }

    [Fact]
    public void Application_must_not_reference_infrastructure() { /* ... */ }

    // ... all 18 guardrails including 3 new SharedKernel isolation rules
}
```

Product test project — the entire file:

```csharp
public class ArchitectureTests : CleanArchitectureGuardrails
{
    protected override Assembly DomainAssembly =>
        typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly;
    protected override Assembly ApplicationAssembly =>
        typeof(CampF 3 new SharedKernel isolation rules
}
```

Product test project — the entire file:

```csharp
public class ArchitectureTests : CleanArchitectureGuardrails
{itFurDogs.Application.AssemblyMarker).Assembly;
    protected override Assembly InfrastructureAssembly =>
        typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;
    protected override Assembly ApiAssembly =>
        typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
}
```

Dependencies
    protected override Assembly DomainAssembly =>
        typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly;
    protected override Assembly ApplicationAssembly =>
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly;
    protected override Assembly InfrastructureAssembly =>
        typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly;
    protected override Assembly ApiAssembly =>
        typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly;
}
```

Dependencies: `NetArchTest.Rules`, `FluentAssertions`, `xunit`.

### Part E: Runtime boundary validation in AddSharedKernel()

`AddSharedKernel()` validates architecture boundaries at application startup:

```csharp
public static IServiceCollection AddSharedKernel(
    this IServiceCollection services,: `NetArchTest.Rules`, `FluentAssertions`, `xunit`.

### Part E: Runtime boundary validation in AddSharedKernel()

`AddSharedKernel()` validates architecture boundaries at application startup:

```csharp
public static IServiceCollection AddSharedKernel(
    this IServiceCollection services,
    SharedKernelOptions options)
{
    // Register dispatchers, scan handlers...

    if (options.ValidateBoundariesAtStartup)
    {
        
    SharedKernelOptions options)
{
    // Register dispatchers, scan handlers...

    if (options.ValidateBoundariesAtStartup)
    {
        ArchitectureGuard.Validate(
            domain: options.DomainAssembly,
            application: options.ApplicationAssembly,
            infrastructure: options.InfrastructureAssembly,
            api: options.ApiAssembly);
    }

    return services;
}
```

If boundaries are violated, `ArchitectureArchitectureGuard.Validate(
            domain: options.DomainAssembly,
            application: options.ApplicationAssembly,
            infrastructure: options.InfrastructureAssembly,
            api: options.ApiAssembly);
    }

    return services;
}
```

If boundaries are violated, `ArchitectureGuard` throws `ArchitectureViolationException` with a message listing every offending type. The app will not start with broken boundaries.

Opt-out: `SharedKernelOptions.ValidateBoundariesAtStartup = false`.

### Part F: SlimGuard` throws `ArchitectureViolationException` with a message listing every offending type. The app will not start with broken boundaries.

Opt-out: `SharedKernelOptions.ValidateBoundariesAtStartup = false`.

### Part F: Slim product projects

After extraction, product projects contain only product code:

- **Application** — handlers product projects

After extraction, product projects contain only product code:

- **Application** — handlers, commands, queries, DTOs, validators, reader interfaces.
- **Infrastructure** — AppDbContext, EF configurations, migrations, repositories, readers, DummyCurrentUserService, product DI registrations.
- **Api** — endpoints, request, commands, queries, DTOs, validators, reader interfaces.
- **Infrastructure** — AppDbContext, EF configurations, migrations, repositories, readers, DummyCurrentUserService, product DTOs, Program.cs, appsettings.

### Part G: Update reference graph

```
SharedKernel                          (M.E.DI.Abstractions, Scrutor)
     DI registrations.
- **Api** — endpoints, request DTOs, Program.cs, appsettings.

### Part G: Update reference graph

```
SharedKernel                          (M.E.DI.Abstractions, Scrutor)
    ^           ^
    |    SharedKernel.Infrastructure   (EF Core)
    |           ^
  Domain        |
    ^           |
Application     |
    ^           |
Infrastructure -+
    ^
   Api --> SharedKernel.Api            (ASP.NET Core routing)
```

- SharedKernel depends^           ^
    |    SharedKernel.Infrastructure   (EF Core)
    |           ^
  Domain        |
    ^           |
Application     |
    ^           |
Infrastructure -+
    ^
   Api --> on nothing but two lightweight NuGet packages.
- Domain depends on SharedKernel only (pure domain, unchanged).
- Application depends on Domain (gets SharedKernel transit SharedKernel.Api            (ASP.NET Core routing)
```

- SharedKernel depends on nothing but two lightweight NuGet packages.
- Domain depends on SharedKernel only (pure domain, unchanged).
- Application depends on Domain (gets SharedKernel transitively).
- Infrastructure depends on Application + Domain + SharedKernel.Infrastructure.
- Api depends on Application + Infrastructure + SharedKernel.Api.
- All acyclic. Compilerively).
- Infrastructure depends on Application + Domain + SharedKernel.Infrastructure.
- Api depends on Application + Infrastructure + SharedKernel.Api.
- All acyclic. Compiler-enforced.

### Part H: Migrate product Architecture.Tests

Replace 15 individual guardrail test classes (~400 lines) with one class (~10 lines)-enforced.

### Part H: Migrate product Architecture.Tests

Replace 15 individual guardrail test classes (~400 lines) with one class (~10 lines) inheriting from `CleanArchitectureGuardrails`. Existing guardrail logic moves into SharedKernel.Testing; product tests are reduced inheriting from `CleanArchitectureGuardrails`. Existing guardrail logic moves into SharedKernel.Testing; product tests are reduced to assembly marker configuration.

Guardrail migration summary:

| Category | Count | Work |
|----------|-------|------|
| Unchanged logic (moves to base class) | 10 | Lift and shift |
| Anchor to assembly marker configuration.

Guardrail migration summary:

| Category | Count | Work |
|----------|-------|------|
| Unchanged logic (moves to base class) | 10 | Lift and shift |
| Anchor update (typeof reference changes) | 4 | One typeof swap per test |
| Rewrite update (typeof reference changes) | 4 | One typeof swap per test |
| Rewrite (CommandsQueriesMustLiveInAbstractions) | 1 | ~15 lines of new logic |
| New (Shared (CommandsQueriesMustLiveInAbstractions) | 1 | ~15 lines of new logic |
| New (SharedKernel isolation rules) | 3 | ~20 lines each |
| **Total in base class** | **18** | Inherited by all products |

### Part I: UpdateKernel isolation rules) | 3 | ~20 lines each |
| **Total in base class** | **18** | Inherited by all products |

### Part I: Update DI wiring

SharedKernel exposes a generic `AddSharedKernel(SharedKernelOptions)` extension that product projects call from their composition root. Product-specific DI registrations stay in Infrastructure's `DependencyInjection.cs`.

Product `Program DI wiring

SharedKernel exposes a generic `AddSharedKernel(SharedKernelOptions)` extension that product projects call from their composition root. Product-specific DI registrations stay.cs`:

```csharp
services.AddSharedKernel(new SharedKernelOptions
{
    ApplicationAssemblies = new[]
    {
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
    },
    DomainAssembly = typeof(CampFitFurDogs.Domain.Ass in Infrastructure's `DependencyInjection.cs`.

Product `Program.cs`:

```csharp
services.AddSharedKernel(new SharedKernelOptions
{
    ApplicationAssemblies = new[]
    {
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
    },
    DomainAssembly = typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly,
    ApplicationAssembly = typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
    InfrastructureAssembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly,
    ApiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly,
    ValidateBoundariesAtStartup = true
});

services.AddInemblyMarker).Assembly,
    ApplicationAssembly = typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly,
    InfrastructureAssembly = typeof(CampFitFurDogs.Infrastructure.DependencyInjection).Assembly,
    ApiAssembly = typeof(CampFitFurDogs.Api.AssemblyMarker).Assembly,
    ValidateBoundariesAtStartup = true
});

services.AddInfrastructure(configuration);  // product-specific: DbContext, repos, readers
```

### Part J: Update docs

- `folder-structure.md` updatedfrastructure(configuration);  // product-specific: DbContext, repos, readers
```

### Part J: Update docs

- `folder-structure.md` updated with new project layout.
- `copilot-instructions.md` updated with foundation vs. product boundary rules.
- CHANGELOG updated with new project layout.
- `copilot-instructions.md` updated with foundation vs. product boundary rules.
- CHANGELOG updated.

## File Inventory

### Files moving to SharedKernel.

## File Inventory

### Files moving to SharedKernel

| Source | File | Destination |
|--------|------|-------------|
| Application | `Abstractions/ICommand.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/ICommandHandler.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/ICommandDispatcher.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IQuery.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IQueryHandler.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IQueryDispatcher.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IDomainEventDispatcher.cs` | `SharedKernel/Application/` |
| Application |

| Source | File | Destination |
|--------|------|-------------|
| Application | `Abstractions/ICommand.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/ICommandHandler.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/ICommandDispatcher.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IQuery.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IQueryHandler.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IQueryDispatcher.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IDomainEventDispatcher.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IDomainEventHandler.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IUnitOfWork.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/ICurrentUserService.cs` | `SharedKernel/Application/` |
| Application | `CommandDispatcher.cs` | `SharedKernel/Application/` |
| Application | `QueryDispatcher.cs` | `SharedKernel/Application/` |
| `Abstractions/IDomainEventHandler.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/IUnitOfWork.cs` | `SharedKernel/Application/` |
| Application | `Abstractions/ICurrentUserService.cs` | `SharedKernel/Application/` |
| Application | `CommandDispatcher.cs` | `SharedKernel/Application/` |
| Application | `QueryDispatcher.cs` | `SharedKernel/Application/` |
| Application | `DomainEvents/DomainEventDispatcher.cs` | `SharedKernel/Application/` |
| Application | `DependencyInjection/DependencyInjection.cs` (foundation half) | `SharedKernel/Application/` |
| Application | `DependencyInjection/DomainEventRegistration.cs` | Application | `DomainEvents/DomainEventDispatcher.cs` | `SharedKernel/Application/` |
| Application | `DependencyInjection/DependencyInjection.cs` (foundation half) | `SharedKernel/Application/` |
| Application | `DependencyInjection/DomainEventRegistration.cs` | `SharedKernel/Application/` |

### Files moving to SharedKernel.Infrastructure

| Source | File | Destination |
|--------|------|-------------|
| Infrastructure | `Data/Configurations/AggregateConfiguration.cs` | `SharedKernel.Infrastructure/` |
| Infrastructure | `Data/EfUnitOfWork.cs` | `SharedKernel.Infrastructure `SharedKernel/Application/` |

### Files moving to SharedKernel.Infrastructure

| Source | File | Destination |
|--------|------|-------------|
| Infrastructure | `Data/Configurations/AggregateConfiguration.cs` | `SharedKernel.Infrastructure/` |
| Infrastructure | `Data/EfUnitOfWork.cs` | `SharedKernel.Infrastructure/` |

### Files moving to SharedKernel.Api

| Source | File | Destination |
|--------|------|-------------|
| Api | `IEndpoint.cs` | `SharedKernel.Api/` |
| Api | `Endpoints.cs` | `SharedKernel.Api/` |

### Files staying in product projects (no change)

| Project | Count | Contents |
|---------|-------|----------|
| Domain | 13 | Aggregates, V/` |

### Files moving to SharedKernel.Api

| Source | File | Destination |
|--------|------|-------------|
| Api | `IEndpoint.cs` | `SharedKernel.Api/` |
| Api | `Endpoints.cs` | `SharedKernel.Api/` |

### Files staying in product projects (no change)

| Project | Count | Contents |
|---------|-------|----------|
| Domain | 13 | Aggregates, VOs, repo interfaces |
| Application | 10 | Handlers, commands, queries, DTOs, validators, reader interfaces |
| Infrastructure | 10+ | AppDbContext, EF configs, migrations, repos, readers, product DI |
| Api | 8 | Endpoints, request DTOs, Program.cs, config |

## DelivOs, repo interfaces |
| Application | 10 | Handlers, commands, queries, DTOs, validators, reader interfaces |
| Infrastructure | 10+ | AppDbContext, EF configs, migrations, repos, readers, product DI |
| Api | 8 | Endpoints, request DTOs, Program.cs, config |

## Deliverables

- [ ] ADR-0022: Foundation Extraction — SharedKernel Expansion
- [ ] `CampFitFurDogs.SharedKernel.csproj`erables

- [ ] ADR-0022: Foundation Extraction — SharedKernel Expansion
- [ ] `CampFitFurDogs.SharedKernel.csproj` updated with `M.E.DI.Abstractions` and `Scrutor` deps
- [ ] `SharedKernel/Application/` subfolder with all 15 moved files
- [ ] Namespaces updated from `CampFitFurDogs.Application.*` to `CampFitFurDogs.SharedKernel.Application.*`
- [ ] `C updated with `M.E.DI.Abstractions` and `Scrutor` deps
- [ ] `SharedKernel/Application/` subfolder with all 15 moved files
- [ ] Namespaces updated fromampFitFurDogs.SharedKernel.Infrastructure` project created
- [ ] `AggregateConfiguration.cs` and `EfUnitOfWork.cs` moved and re-namespaced
- [ ] `CampFitFurDogs.SharedKernel.Api` project created
- [ ] `IEndpoint.cs` and `Endpoints.cs` moved and re- `CampFitFurDogs.Application.*` to `CampFitFurDogs.SharedKernel.Application.*`
- [ ] `CampFitFurDogs.SharedKernel.Infrastructure` project created
- [ ] `AggregateConfiguration.cs` and `EfUnitOfWork.cs` moved and re-namespaced
- [ ] `CampFitFurDogs.SharedKernel.Api` project created
- [ ] `IEndpoint.cs` and `Endpoints.cs` moved and re-namespaced
- [ ] `CampFitFurDogs.SharedKernel.Testing` project created
- [ ] `CleanArchitectureGuardrails` abstract base class with 18 inherited tests
- [ ] `ArchitectureGuard` runtime validator
- [ ] `Sharednamespaced
- [ ] `CampFitFurDogs.SharedKernel.Testing` project created
- [ ] `CleanArchitectureGuardrails` abstract base class with 18 inherited tests
- [ ] `ArchitectureGuard` runtime validator
- [ ] `SharedKernelOptions` configuration class
- [ ] `ArchitectureViolationException`
- [ ] All four new/expanded projects added to `CampFitFurDogs.KernelOptions` configuration class
- [ ] `ArchitectureViolationException`
- [ ] All four new/expanded projects added to `CampFitFurDogs.sln`
- [ ] Product `.csproj` references updated
- [ ] `AddSharedKernel()` extension method created with runtime validation
- [ ] Product `DependencyInjection.cs` calls `AddSharedKernel()sln`
- [ ] Product `.csproj` references updated
- [ ] `AddSharedKernel()` extension method created with runtime validation
- [ ] Product `DependencyInjection.cs` calls `AddSharedKernel()` and keeps only product registrations
- [ ] Product `Architecture.Tests` migrated to single inheriting class
- [ ] `` and keeps only product registrations
- [ ] Product `Architecture.Tests` migrated to single inheriting class
- [ ] `using` statements updated across all product files
- [ ] `folder-structure.md` updated with new project layout
- [ ] `copilot-instructions.md` updated with foundation vs. product boundary rules
- [ ] CHANGELOG updated
- [ ] CI passes

## Acceptance Criteria

- [ ] SharedKernel contains zero product-specific types (no Camp Fit Fur Dogs handlers, commands, queries, DTOs, or domain entities)
- [ ] SharedKernel.Infrastructure contains zero product-specific types (no AppDbContext, nousing` statements updated across all product files
- [ ] `folder-structure.md` updated with new project layout
- [ ] `copilot-instructions.md` updated with foundation vs. product boundary rules
- [ ] CHANGELOG updated
- [ ] CI passes

## Acceptance Criteria

- [ ] SharedKernel contains zero product-specific types (no Camp Fit Fur Dogs handlers, commands, queries, DTOs, or domain entities)
- [ ] SharedKernel.Infrastructure contains zero product-specific types (no AppDbContext, no entity configurations, no repositories)
- [ ] SharedKernel.Api contains zero product-specific types (no product endpoints)
- [ ] Application, Infrastructure, and Api contain zero foundation types (no CQRS interfaces, no dispatchers, no DI scanning entity configurations, no repositories)
- [ ] SharedKernel.Api contains zero product-specific types (no product endpoints)
- [ ] Application, Infrastructure, and Api contain zero foundation types (no CQRS interfaces, no dispatchers, no DI scanning conventions, no AggregateConfiguration, no EfUnitOfWork, no IEndpoint/Endpoints)
- [ ] Domain project is unchanged
- [ ] Reference conventions, no AggregateConfiguration, no EfUnitOfWork, no IEndpoint/Endpoints)
- [ ] Domain project is unchanged
- [ ] Reference graph is acyclic — verified by Architecture.Tests
- [ ] All existing guardrail rules pass via inherited `CleanArchitectureGuardrails`
- [ ] All existing graph is acyclic — verified by Architecture.Tests
- [ ] All existing guardrail rules pass via inherited `CleanArchitectureGuardrails`
- [ ] All existing unit and integration tests pass without modification (beyond `using` statement updates)
- [ ] `AddSharedKernel()` accepts `SharedKernelOptions` and registers all dispatchers, handlers, validators unit and integration tests pass without modification (beyond `using` statement updates)
- [ ] `AddSharedKernel()` accepts `SharedKernelOptions` and registers all dispatchers, handlers, validators, and readers
- [ ] `AddSharedKernel()` throws `ArchitectureViolationException` on boundary violations when `ValidateBoundariesAtStartup` is `true`
- [ ] `AddSharedKernel()` skips validation when `ValidateBoundariesAtStartup` is `false`
- [ ] Product `Architecture.Tests` is a single class (~10 lines) providing, and readers
- [ ] `AddSharedKernel()` throws `ArchitectureViolationException` on boundary violations when `ValidateBoundariesAtStartup` is `true`
- [ ] `AddSharedKernel()` skips validation when `ValidateBoundariesAtStartup` is `false`
- [ ] Product `Architecture.Tests` is a single class (~10 lines) providing 4 assembly markers
- [ ] A new product gets 18 architecture tests by inheriting `CleanArchitectureGuardrails`
- [ ] A hypothetical new product could reference SharedKernel + SharedKernel.Infrastructure + SharedK 4 assembly markers
- [ ] A new product gets 18 architecture tests by inheriting `CleanArchitectureGuardrails`
- [ ] A hypothetical new product could reference SharedKernel + SharedKernel.Infrastructure + SharedKernel.Api and get the full CQRS + DI + endpoint + EF pipeline with zero Camp Fit Fur Dogs code
- [ ] ADR-0022 acceptedernel.Api and get the full CQRS + DI + endpoint + EF pipeline with zero Camp Fit Fur Dogs code
- [ ] ADR-0022 accepted and indexed
- [ ] CI passes

## Emotional Guarantees

- A developer reading a product project sees only product code — no framework noise, no "don't touch and indexed
- [ ] CI passes

## Emotional Guarantees

- A developer reading a product project sees only product code — no framework noise, no "don't touch this" files.
- A developer starting a new product on this chassis knows exactly what to reference and what to build themselves.
- A developer never this" files.
- A developer starting a new product on this chassis knows exactly what to reference and what to build themselves.
- A developer never confuses "is this mine to change?" with "is this shared infrastructure?" — the project boundary makes it obvious.
- A developer gets boundary enforcement for free by inheriting one test class — no confuses "is this mine to change?" with "is this shared infrastructure?" — the project boundary makes it obvious.
- A developer gets boundary enforcement for free by inheriting one test class — no guardrail test authoring required.

## Dependencies

- US-106 (Add-Only Slice Architecture) — completed (Sprint guardrail test authoring required.

## Dependencies

- US-106 (Add-Only Slice Architecture) — completed (Sprint 5)
- US-050 (Unit of Work) — completed (Sprint 5)
- US-051 (CQRS Pipelines) — completed (Sprint 4)

## Estimated Effort

~ 5)
- US-050 (Unit of Work) — completed (Sprint 5)
- US-051 (CQRS Pipelines) — completed (Sprint 4)

## Estimated Effort

~8 hours (4 new/expanded projects + file moves + namespace updates + DI refactor + SharedKernel.Testing + runtime guard + guardrail migration + docs)8 hours (4 new/expanded projects + file moves + namespace updates + DI refactor + SharedKernel.Testing + runtime guard + guardrail migration + docs)
