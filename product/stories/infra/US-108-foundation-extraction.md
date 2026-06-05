---
id: US-108
title: "Foundation Extraction"
epic: 
milestone: M0
status: shipped
domain: infra
urgency: 
importance: 
covey_quadrant: 
vertical_slice: false
emotional_guarantees:
legal_guarantees:
dependencies:
---
# US-108 — Foundation Extraction

## Intent

As a **developer**, I want the reusable architectural foundation (CQRS pipelineThe file export hit a wall — I couldn't get it to deliver downloadable files. Here are all four files inline for you to copy. I've used four-backtick outer fences on the story files since they contain inner code blocks.

---

## 1. `US-108-foundation-extraction.md`

````markdown
# US-108 — Foundation Extraction

## Intent

As a **developer**, I want the reusable architectural foundation (CQRS pipeline, DI conventions, EF base classes, endpoint auto-discovery) separated from product-specific code into dedicated Frank projects,, DI conventions, EF base classes, endpoint auto-discovery) separated from product-specific code into dedicated Frank projects, with self-enforcing boundary guardrails — so that a new product can reference the foundation without inheriting Camp Fit Fur Dogs business logic, boundary violations are caught at both test time and runtime with self-enforcing boundary guardrails — so that a new product can reference the foundation without inheriting Camp Fit Fur Dogs business logic, boundary violations are caught at both without writing product-specific guardrail tests, and product projects contain only product code.

## Value

- **Reusable chassis** — test time and runtime without writing product-specific guardrail tests, and product projects contain only product code.

## Value

- **Reusable chassis** — a new product references three Frank projects and gets DDD building blocks, CQRS pipeline, DI scanning, EF base configuration, and endpoint auto-discovery out a new product references three Frank projects and gets DDD building blocks, CQRS pipeline, DI scanning, EF base configuration, and endpoint auto-discovery out of the box.
- **Clean product projects** — Application, Infrastructure, and Api contain only Camp Fit Fur Dogs handlers, readers, repositories, endpoints, and configuration of the box.
- **Clean product projects** — Application, Infrastructure, and Api contain only Camp Fit Fur Dogs handlers, readers, repositories, endpoints, and configuration. No framework noise.
- **Portable foundation** — Frank stays ORM-agnostic and host-agnostic. EF Core and ASP.NET Core dependencies. No framework noise.
- **Portable foundation** — Frank stays ORM-agnostic and host-agnostic. EF Core and ASP.NET Core dependencies are isolated in companion projects that a different product can swap.
- **Self-enforcing boundaries** — Frank.Testing provides inheritable guardrails and `AddFrank()` validates boundaries at startup. A new product gets are isolated in companion projects that a different product can swap.
- **Self-enforcing boundaries** — Frank.Testing provides inheritable guardrails and `AddFrank()` validates boundaries at startup. A new product gets 18 architecture tests by providing 4 assembly markers.
- **Onboarding clarity** — "Frank is 18 architecture tests by providing 4 assembly markers.
- **Onboarding clarity** — "Frank is the framework. Everything else is the product." One sentence, no ambiguity.

## Problem

Today, foundation code and product code are mixed in the same projects the framework. Everything else is the product." One sentence, no ambiguity.

## Problem

Today, foundation code and product code are mixed in the same projects:

| Project | Foundation files | Product files | Mixed? |
|---------|-----------------|---------------|--------|
| Frank | 6 | 0 | Clean |
| Domain:

| Project | Foundation files | Product files | Mixed? |
|---------|-----------------|---------------|--------|
| Frank | 6 | 0 | Clean |
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

### Part A: Expand Frank (Tier 0-2 — pure C scratch.

## Solution

### Part A: Expand Frank (Tier 0-2 — pure C# + M.E.DI.Abstractions + Scrutor)

Move all product-agnostic CQRS interfaces# + M.E.DI.Abstractions + Scrutor)

Move all product-agnostic CQRS interfaces, dispatchers, and DI conventions from Application into Frank:

```
src/Frank/
  Domain/                , dispatchers, and DI conventions from Application into Frank:

```
src/Frank/
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

New dependencies for Frank: `Microsoft.Extensions.DependencyInjection.Abstractions`, `Scrutor`.

### Part B: Create Frank.Infrastructure (Tier 3 — EF Core)

```
src/Frank.Infrastructure/
  AggregateConfiguration.cs            (from Infrastructure) half)
    DomainEventRegistration.cs
    AssemblyMarker.cs
```

New dependencies for Frank: `Microsoft.Extensions.DependencyInjection.Abstractions`, `Scrutor`.

### Part B: Create Frank.Infrastructure (Tier 3 — EF Core)

```
src/Frank.Infrastructure/
  AggregateConfiguration.cs            (from Infrastructure)
  EfUnitOfWork.cs                      (from Infrastructure)
```

Dependencies: `Microsoft.EntityFrameworkCore`. References: `Frank`.

### Part C: Create Frank.Api (Tier 4 — ASP.NET Core routing)

```
src/Frank.Api/
  IEndpoint.cs                         (from Api)
  Endpoints.cs                         (from Api)
  EfUnitOfWork.cs                      (from Infrastructure)
```

Dependencies: `Microsoft.EntityFrameworkCore`. References: `Frank`.

### Part C: Create Frank.Api (Tier 4 — ASP.NET Core routing)

```
src/CampFitFurDogs.SharedK
```

Dependencies: `Microsoft.AspNetCore.Routing.Abstractions`. References: none.

### Part D: Create Frank.Testing

Frank ships an abstract test base class that products inheriternel.Api/
  IEndpoint.cs                         (from Api)
  Endpoints.cs                         (from Api)
```

Dependencies: `Microsoft.AspNetCore.Routing.Abstractions`. References: none.

### Part D: Create Frank.Testing

Frank ships an abstract test base class that products inherit. Four assembly markers, 18 guardrails, zero per-product test logic:

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

    // ... all 18 guardrails including 3 new Frank isolation rules
}
```

Product test project — the entire file:

```csharp
public class ArchitectureTests : CleanArchitectureGuardrails
{
    protected override Assembly DomainAssembly =>
        typeof(CampFitFurDogs.Domain.AssemblyMarker).Assembly;
    protected override Assembly ApplicationAssembly =>
        typeof(CampF 3 new Frank isolation rules
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

### Part E: Runtime boundary validation in AddFrank()

`AddFrank()` validates architecture boundaries at application startup:

```csharp
public static IServiceCollection AddFrank(
    this IServiceCollection services,: `NetArchTest.Rules`, `FluentAssertions`, `xunit`.

### Part E: Runtime boundary validation in AddFrank()

`AddFrank()` validates architecture boundaries at application startup:

```csharp
public static IServiceCollection AddFrank(
    this IServiceCollection services,
    FrankOptions options)
{
    // Register dispatchers, scan handlers...

    if (options.ValidateBoundariesAtStartup)
    {
        
    FrankOptions options)
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

Opt-out: `FrankOptions.ValidateBoundariesAtStartup = false`.

### Part F: SlimGuard` throws `ArchitectureViolationException` with a message listing every offending type. The app will not start with broken boundaries.

Opt-out: `FrankOptions.ValidateBoundariesAtStartup = false`.

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
Frank                          (M.E.DI.Abstractions, Scrutor)
     DI registrations.
- **Api** — endpoints, request DTOs, Program.cs, appsettings.

### Part G: Update reference graph

```
Frank                          (M.E.DI.Abstractions, Scrutor)
    ^           ^
    |    Frank.Infrastructure   (EF Core)
    |           ^
  Domain        |
    ^           |
Application     |
    ^           |
Infrastructure -+
    ^
   Api --> Frank.Api            (ASP.NET Core routing)
```

- Frank depends^           ^
    |    Frank.Infrastructure   (EF Core)
    |           ^
  Domain        |
    ^           |
Application     |
    ^           |
Infrastructure -+
    ^
   Api --> on nothing but two lightweight NuGet packages.
- Domain depends on Frank only (pure domain, unchanged).
- Application depends on Domain (gets Frank transit Frank.Api            (ASP.NET Core routing)
```

- Frank depends on nothing but two lightweight NuGet packages.
- Domain depends on Frank only (pure domain, unchanged).
- Application depends on Domain (gets Frank transitively).
- Infrastructure depends on Application + Domain + Frank.Infrastructure.
- Api depends on Application + Infrastructure + Frank.Api.
- All acyclic. Compilerively).
- Infrastructure depends on Application + Domain + Frank.Infrastructure.
- Api depends on Application + Infrastructure + Frank.Api.
- All acyclic. Compiler-enforced.

### Part H: Migrate product Architecture.Tests

Replace 15 individual guardrail test classes (~400 lines) with one class (~10 lines)-enforced.

### Part H: Migrate product Architecture.Tests

Replace 15 individual guardrail test classes (~400 lines) with one class (~10 lines) inheriting from `CleanArchitectureGuardrails`. Existing guardrail logic moves into Frank.Testing; product tests are reduced inheriting from `CleanArchitectureGuardrails`. Existing guardrail logic moves into Frank.Testing; product tests are reduced to assembly marker configuration.

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
| New (Frank isolation rules) | 3 | ~20 lines each |
| **Total in base class** | **18** | Inherited by all products |

### Part I: UpdateKernel isolation rules) | 3 | ~20 lines each |
| **Total in base class** | **18** | Inherited by all products |

### Part I: Update DI wiring

Frank exposes a generic `AddFrank(FrankOptions)` extension that product projects call from their composition root. Product-specific DI registrations stay in Infrastructure's `DependencyInjection.cs`.

Product `Program DI wiring

Frank exposes a generic `AddFrank(FrankOptions)` extension that product projects call from their composition root. Product-specific DI registrations stay.cs`:

```csharp
services.AddFrank(new FrankOptions
{
    ApplicationAssemblies = new[]
    {
        typeof(CampFitFurDogs.Application.AssemblyMarker).Assembly
    },
    DomainAssembly = typeof(CampFitFurDogs.Domain.Ass in Infrastructure's `DependencyInjection.cs`.

Product `Program.cs`:

```csharp
services.AddFrank(new FrankOptions
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

### Files moving to Frank.

## File Inventory

### Files moving to Frank

| Source | File | Destination |
|--------|------|-------------|
| Application | `Abstractions/ICommand.cs` | `Frank/Application/` |
| Application | `Abstractions/ICommandHandler.cs` | `Frank/Application/` |
| Application | `Abstractions/ICommandDispatcher.cs` | `Frank/Application/` |
| Application | `Abstractions/IQuery.cs` | `Frank/Application/` |
| Application | `Abstractions/IQueryHandler.cs` | `Frank/Application/` |
| Application | `Abstractions/IQueryDispatcher.cs` | `Frank/Application/` |
| Application | `Abstractions/IDomainEventDispatcher.cs` | `Frank/Application/` |
| Application |

| Source | File | Destination |
|--------|------|-------------|
| Application | `Abstractions/ICommand.cs` | `Frank/Application/` |
| Application | `Abstractions/ICommandHandler.cs` | `Frank/Application/` |
| Application | `Abstractions/ICommandDispatcher.cs` | `Frank/Application/` |
| Application | `Abstractions/IQuery.cs` | `Frank/Application/` |
| Application | `Abstractions/IQueryHandler.cs` | `Frank/Application/` |
| Application | `Abstractions/IQueryDispatcher.cs` | `Frank/Application/` |
| Application | `Abstractions/IDomainEventDispatcher.cs` | `Frank/Application/` |
| Application | `Abstractions/IDomainEventHandler.cs` | `Frank/Application/` |
| Application | `Abstractions/IUnitOfWork.cs` | `Frank/Application/` |
| Application | `Abstractions/ICurrentUserService.cs` | `Frank/Application/` |
| Application | `CommandDispatcher.cs` | `Frank/Application/` |
| Application | `QueryDispatcher.cs` | `Frank/Application/` |
| `Abstractions/IDomainEventHandler.cs` | `Frank/Application/` |
| Application | `Abstractions/IUnitOfWork.cs` | `Frank/Application/` |
| Application | `Abstractions/ICurrentUserService.cs` | `Frank/Application/` |
| Application | `CommandDispatcher.cs` | `Frank/Application/` |
| Application | `QueryDispatcher.cs` | `Frank/Application/` |
| Application | `DomainEvents/DomainEventDispatcher.cs` | `Frank/Application/` |
| Application | `DependencyInjection/DependencyInjection.cs` (foundation half) | `Frank/Application/` |
| Application | `DependencyInjection/DomainEventRegistration.cs` | Application | `DomainEvents/DomainEventDispatcher.cs` | `Frank/Application/` |
| Application | `DependencyInjection/DependencyInjection.cs` (foundation half) | `Frank/Application/` |
| Application | `DependencyInjection/DomainEventRegistration.cs` | `Frank/Application/` |

### Files moving to Frank.Infrastructure

| Source | File | Destination |
|--------|------|-------------|
| Infrastructure | `Data/Configurations/AggregateConfiguration.cs` | `Frank.Infrastructure/` |
| Infrastructure | `Data/EfUnitOfWork.cs` | `Frank.Infrastructure `Frank/Application/` |

### Files moving to Frank.Infrastructure

| Source | File | Destination |
|--------|------|-------------|
| Infrastructure | `Data/Configurations/AggregateConfiguration.cs` | `Frank.Infrastructure/` |
| Infrastructure | `Data/EfUnitOfWork.cs` | `Frank.Infrastructure/` |

### Files moving to Frank.Api

| Source | File | Destination |
|--------|------|-------------|
| Api | `IEndpoint.cs` | `Frank.Api/` |
| Api | `Endpoints.cs` | `Frank.Api/` |

### Files staying in product projects (no change)

| Project | Count | Contents |
|---------|-------|----------|
| Domain | 13 | Aggregates, V/` |

### Files moving to Frank.Api

| Source | File | Destination |
|--------|------|-------------|
| Api | `IEndpoint.cs` | `Frank.Api/` |
| Api | `Endpoints.cs` | `Frank.Api/` |

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

- [ ] ADR-0022: Foundation Extraction — Frank Expansion
- [ ] `Frank.csproj`erables

- [ ] ADR-0022: Foundation Extraction — Frank Expansion
- [ ] `Frank.csproj` updated with `M.E.DI.Abstractions` and `Scrutor` deps
- [ ] `Frank/Application/` subfolder with all 15 moved files
- [ ] Namespaces updated from `CampFitFurDogs.Application.*` to `Frank.Application.*`
- [ ] `C updated with `M.E.DI.Abstractions` and `Scrutor` deps
- [ ] `Frank/Application/` subfolder with all 15 moved files
- [ ] Namespaces updated fromampFitFurDogs.Frank.Infrastructure` project created
- [ ] `AggregateConfiguration.cs` and `EfUnitOfWork.cs` moved and re-namespaced
- [ ] `Frank.Api` project created
- [ ] `IEndpoint.cs` and `Endpoints.cs` moved and re- `CampFitFurDogs.Application.*` to `Frank.Application.*`
- [ ] `Frank.Infrastructure` project created
- [ ] `AggregateConfiguration.cs` and `EfUnitOfWork.cs` moved and re-namespaced
- [ ] `Frank.Api` project created
- [ ] `IEndpoint.cs` and `Endpoints.cs` moved and re-namespaced
- [ ] `Frank.Testing` project created
- [ ] `CleanArchitectureGuardrails` abstract base class with 18 inherited tests
- [ ] `ArchitectureGuard` runtime validator
- [ ] `Sharednamespaced
- [ ] `Frank.Testing` project created
- [ ] `CleanArchitectureGuardrails` abstract base class with 18 inherited tests
- [ ] `ArchitectureGuard` runtime validator
- [ ] `FrankOptions` configuration class
- [ ] `ArchitectureViolationException`
- [ ] All four new/expanded projects added to `CampFitFurDogs.KernelOptions` configuration class
- [ ] `ArchitectureViolationException`
- [ ] All four new/expanded projects added to `CampFitFurDogs.sln`
- [ ] Product `.csproj` references updated
- [ ] `AddFrank()` extension method created with runtime validation
- [ ] Product `DependencyInjection.cs` calls `AddFrank()sln`
- [ ] Product `.csproj` references updated
- [ ] `AddFrank()` extension method created with runtime validation
- [ ] Product `DependencyInjection.cs` calls `AddFrank()` and keeps only product registrations
- [ ] Product `Architecture.Tests` migrated to single inheriting class
- [ ] `` and keeps only product registrations
- [ ] Product `Architecture.Tests` migrated to single inheriting class
- [ ] `using` statements updated across all product files
- [ ] `folder-structure.md` updated with new project layout
- [ ] `copilot-instructions.md` updated with foundation vs. product boundary rules
- [ ] CHANGELOG updated
- [ ] CI passes

## Acceptance Criteria

- [ ] Frank contains zero product-specific types (no Camp Fit Fur Dogs handlers, commands, queries, DTOs, or domain entities)
- [ ] Frank.Infrastructure contains zero product-specific types (no AppDbContext, nousing` statements updated across all product files
- [ ] `folder-structure.md` updated with new project layout
- [ ] `copilot-instructions.md` updated with foundation vs. product boundary rules
- [ ] CHANGELOG updated
- [ ] CI passes

## Acceptance Criteria

- [ ] Frank contains zero product-specific types (no Camp Fit Fur Dogs handlers, commands, queries, DTOs, or domain entities)
- [ ] Frank.Infrastructure contains zero product-specific types (no AppDbContext, no entity configurations, no repositories)
- [ ] Frank.Api contains zero product-specific types (no product endpoints)
- [ ] Application, Infrastructure, and Api contain zero foundation types (no CQRS interfaces, no dispatchers, no DI scanning entity configurations, no repositories)
- [ ] Frank.Api contains zero product-specific types (no product endpoints)
- [ ] Application, Infrastructure, and Api contain zero foundation types (no CQRS interfaces, no dispatchers, no DI scanning conventions, no AggregateConfiguration, no EfUnitOfWork, no IEndpoint/Endpoints)
- [ ] Domain project is unchanged
- [ ] Reference conventions, no AggregateConfiguration, no EfUnitOfWork, no IEndpoint/Endpoints)
- [ ] Domain project is unchanged
- [ ] Reference graph is acyclic — verified by Architecture.Tests
- [ ] All existing guardrail rules pass via inherited `CleanArchitectureGuardrails`
- [ ] All existing graph is acyclic — verified by Architecture.Tests
- [ ] All existing guardrail rules pass via inherited `CleanArchitectureGuardrails`
- [ ] All existing unit and integration tests pass without modification (beyond `using` statement updates)
- [ ] `AddFrank()` accepts `FrankOptions` and registers all dispatchers, handlers, validators unit and integration tests pass without modification (beyond `using` statement updates)
- [ ] `AddFrank()` accepts `FrankOptions` and registers all dispatchers, handlers, validators, and readers
- [ ] `AddFrank()` throws `ArchitectureViolationException` on boundary violations when `ValidateBoundariesAtStartup` is `true`
- [ ] `AddFrank()` skips validation when `ValidateBoundariesAtStartup` is `false`
- [ ] Product `Architecture.Tests` is a single class (~10 lines) providing, and readers
- [ ] `AddFrank()` throws `ArchitectureViolationException` on boundary violations when `ValidateBoundariesAtStartup` is `true`
- [ ] `AddFrank()` skips validation when `ValidateBoundariesAtStartup` is `false`
- [ ] Product `Architecture.Tests` is a single class (~10 lines) providing 4 assembly markers
- [ ] A new product gets 18 architecture tests by inheriting `CleanArchitectureGuardrails`
- [ ] A hypothetical new product could reference Frank + Frank.Infrastructure + SharedK 4 assembly markers
- [ ] A new product gets 18 architecture tests by inheriting `CleanArchitectureGuardrails`
- [ ] A hypothetical new product could reference Frank + Frank.Infrastructure + Frank.Api and get the full CQRS + DI + endpoint + EF pipeline with zero Camp Fit Fur Dogs code
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

~8 hours (4 new/expanded projects + file moves + namespace updates + DI refactor + Frank.Testing + runtime guard + guardrail migration + docs)8 hours (4 new/expanded projects + file moves + namespace updates + DI refactor + Frank.Testing + runtime guard + guardrail migration + docs)

