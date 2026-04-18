# ADR‑0015 — Convention‑Based Auto‑Registration

## Status  
Accepted

## Context  
As the codebase grows and new vertical slices are added, manual dependency‑injection (DI) registration becomes a scalability bottleneck. Each new handler, validator, or repository requires editing shared configuration files, increasing merge conflicts, slowing contributor onboarding, and violating the “slice‑local change” principle.

The RegisterDog proving slice (US‑084) demonstrated the desired slice structure and provided a regression baseline. US‑079 introduces a mechanism to eliminate manual DI wiring entirely by adopting convention‑based auto‑registration.

## Decision  
We will use **convention‑based assembly scanning** to automatically register:

- Command handlers  
- Query handlers  
- Validators  
- Repositories  

The conventions are:

- Handlers end with `Handler` and implement `ICommandHandler<TCommand,TResponse>` or `IQueryHandler<TQuery,TResponse>`
- Validators end with `Validator` and implement `IValidator<T>`
- Repositories end with `Repository` and implement an interface named `I<Something>Repository`
- All slice‑specific types live under their respective slice folders in Application or Infrastructure

Scrutor is used to perform assembly scanning and registration:

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<DependencyInjection>()
    .AddClasses(c => c.Where(t => t.Name.EndsWith("Handler")))
    .AsImplementedInterfaces()
    .WithScopedLifetime());
```

Similar scanning rules apply for validators and repositories.

`Program.cs` contains only:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
```

No slice‑specific manual registrations are permitted.

## Rationale  
- **Zero manual DI wiring**: Contributors add slice files only; no shared files change.  
- **Reduced merge conflicts**: Eliminates contention in DI modules.  
- **Predictable structure**: Naming conventions and folder structure become enforceable contracts.  
- **Improved onboarding**: New contributors follow conventions instead of learning DI internals.  
- **Guardrail‑friendly**: Purity tests can enforce conventions and detect regressions.  
- **Future‑proof**: Adding new slices or features does not require modifying global configuration.

## Alternatives Considered  

### 1. Manual DI Registration  
Rejected because it scales poorly, increases merge conflicts, and violates slice isolation.

### 2. Source Generators  
Rejected for now due to complexity, maintenance overhead, and lack of immediate need. Could be revisited later.

### 3. Reflection‑Free DI (e.g., compile‑time DI frameworks)  
Rejected due to added tooling complexity and reduced flexibility during rapid iteration.

## Consequences  

### Positive  
- Contributors never touch DI modules for slice‑specific types  
- New slices are plug‑and‑play  
- Architecture becomes self‑defending via guardrails  
- Reduced cognitive load and onboarding time  
- Consistent, predictable slice structure

### Negative  
- Requires adherence to naming conventions  
- Misnamed or misplaced types silently fail to register (mitigated by guardrails)  
- Assembly scanning adds minimal startup overhead (acceptable for this project)

## Notes  
The detailed conventions, folder structure, and contributor expectations are documented in:

**`docs/guides/di-conventions.md`**

This ADR records the *decision*.  
The guide documents the *rules*.

