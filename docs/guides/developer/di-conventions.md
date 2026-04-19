# Dependency Injection Conventions
**Convention‑Based Auto‑Registration for Handlers, Validators, Repositories, and Readers**

This guide documents the conventions used for automatic discovery and registration of slice‑specific types in the Application and Infrastructure layers. Contributors should follow these conventions when adding new slices or modifying existing ones.

These conventions eliminate the need for manual DI wiring in `Program.cs` or shared configuration files. When followed correctly, new slices become plug‑and‑play.

---

# 1. Overview

The project uses **assembly scanning** to automatically register:

- Command handlers
- Query handlers
- Validators
- Repositories
- Readers

This scanning is convention‑based: if a type follows the naming and folder rules described below, it will be discovered and registered automatically.

---

# 2. Folder Structure Conventions

Each vertical slice follows this structure:

```
<Feature>/
  Commands/
  Queries/
  Handlers/
  Validators/
  Models/
  Repositories/        (Infrastructure only — command slices)
  Readers/             (Infrastructure only — query slices)
```

Slices live in:

- `src/CampFitFurDogs.Application/<Feature>/`
- `src/CampFitFurDogs.Infrastructure/<Feature>/`

The folder names are important for discoverability and contributor clarity, but **DI scanning is based on naming conventions**, not folder paths.

---

# 3. Handler Conventions

## Naming
Handlers must end with:

```
Handler
```

Examples:

- `RegisterDogCommandHandler`
- `GetDogProfileQueryHandler`

## Interfaces
Handlers must implement one of:

```
ICommandHandler<TCommand, TResponse>
IQueryHandler<TQuery, TResponse>
```

## Registration
Handlers are registered automatically via Scrutor:

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<DependencyInjection>()
    .AddClasses(c => c.Where(t => t.Name.EndsWith("Handler")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

No manual registration is required.

---

# 4. Validator Conventions

## Naming
Validators must end with:

```
Validator
```

Examples:

- `RegisterDogCommandValidator`
- `DogProfileQueryValidator`

## Interfaces
Validators must implement:

```
IValidator<T>
```

## Registration
Validators are registered automatically:

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<DependencyInjection>()
    .AddClasses(c => c.Where(t => t.Name.EndsWith("Validator")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

---

# 5. Repository Conventions

## Naming
Repositories must end with:

```
Repository
```

Examples:

- `DogRepository`
- `OwnerRepository`

## Interfaces
Repositories must implement an interface named:

```
I<Something>Repository
```

Example:

- `IDogRepository` → `DogRepository`

## Location
Repositories live in Infrastructure:

```
src/CampFitFurDogs.Infrastructure/<Feature>/
```

## Registration
Repositories are registered automatically:

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Infrastructure.DependencyInjection>()
    .AddClasses(c => c.Where(t => t.Name.EndsWith("Repository")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

## Scope
Repositories are for **command handlers only**. They return domain aggregates for invariant enforcement. Query handlers must not depend on repository interfaces — use readers instead (see Section 6).

---

# 6. Reader Conventions

## Naming
Readers must end with:

```
Reader
```

Examples:

- `GetDogProfileReader`
- `ListDogsReader`

## Interfaces
Reader interfaces live in **Application.Abstractions**, named after the slice:

```
I<UseCase>Reader
```

Example:

- `IGetDogProfileReader` → `GetDogProfileReader`

## Location
Reader implementations live in Infrastructure:

```
src/CampFitFurDogs.Infrastructure/<Feature>/
```

## Registration
Readers are registered automatically via Scrutor's suffix scan:

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<Infrastructure.DependencyInjection>()
    .AddClasses(c => c.Where(t => t.Name.EndsWith("Reader")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

## Scope
Readers are for **query handlers only**. They return response DTOs directly — no domain aggregates cross the boundary. This enforces the CQRS read/write split (ADR-0021).

An architecture guardrail test enforces this: no `IQueryHandler` implementation may depend on any interface ending in `Repository`.

---

# 7. Zero Manual Registration

`Program.cs` must contain only:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
```

No slice‑specific types may be registered manually.

If a contributor finds themselves editing DI code to add a handler, validator, repository, or reader, the slice is violating conventions.

**Exception — cross-cutting infrastructure types.** Types that serve all slices but do not match Scrutor's suffix scan (e.g., `EfUnitOfWork` implementing `IUnitOfWork`) are registered explicitly in `Infrastructure/DependencyInjection.cs`. These are shared infrastructure, not slice-specific. See ADR-0017.

---

# 8. Troubleshooting

If a type is not being registered:

1. Ensure the class name ends with `Handler`, `Validator`, `Repository`, or `Reader`.
2. Ensure the class implements the correct interface.
3. Ensure the class is public.
4. Ensure the class is in the Application or Infrastructure assembly.
5. Ensure the slice folder structure is correct.

If all conventions are followed, the type will be discovered automatically.

---

# 9. Regression Guarantee

The RegisterDog slice (US‑084) serves as the proving slice for command-side conventions.
The GetDogProfile slice (US-106) serves as the proving slice for query-side conventions.
If auto‑registration breaks, these flows will fail, signaling a regression.

---

# 10. Contributor Expectations

When adding a new slice:

- Create the slice folder under Application and Infrastructure.
- Add commands, queries, handlers, validators, repositories (command slices), and readers (query slices) following naming conventions.
- Do **not** modify DI configuration.
- Do **not** add manual registrations.
- Do **not** modify shared files.

If the slice follows conventions, it will "just work."

---

# 11. Related Documents

- ADR‑0015 — Convention‑Based Auto‑Registration (decision record)
- ADR-0021 — Query-Side Reader Isolation (decision record)
- `developer-guide.md` — High‑level contributor workflow
