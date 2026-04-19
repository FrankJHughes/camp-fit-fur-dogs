# US-107 — EF Entity Auto-Discovery

## Intent

As a **contributor**, I want new aggregate roots to be discovered by EF Core automatically through their `IEntityTypeConfiguration<T>` — without editing `AppDbContext` — so that scaffolding a new aggregate never requires modifying an existing file.

## Value

- **Last modification point eliminated** — after US-079 (DI auto-registration) and US-106 (add-only slices), this removes the final shared-file edit required when introducing a new aggregate root.
- **True open-closed at the aggregate level** — scaffolding a new aggregate is purely additive: drop files, run migration, done.
- **Stable `AppDbContext`** — the class becomes a sealed, zero-touch infrastructure fixture. No merge conflicts, no forgotten registrations.
- **Consistent convention-over-configuration** — the same assembly-scanning philosophy used for handlers, validators, readers, and endpoints now applies to EF entity registration.

## Problem

Today, adding a new aggregate root requires **two edits** to `AppDbContext.cs`:

| Line added per aggregate | Purpose |
|--------------------------|---------|
| `public DbSet<T> Xs => Set<T>();` | Exposes the entity set |
| `model.ApplyConfiguration(new XConfiguration());` | Registers the entity type configuration |

These are the only remaining modification points after US-079 and US-106 eliminate DI registration and slice-level edits.

## Solution

### Step 1 — Assembly-scanned configuration

Replace the individual `ApplyConfiguration` calls with a single assembly scan:

```csharp
protected override void OnModelCreating(ModelBuilder model)
{
    model.ApplyConfigurationsFromAssembly(
        typeof(AppDbContext).Assembly);
}
```

Every `IEntityTypeConfiguration<T>` in the Infrastructure assembly is discovered automatically — which is already added as a new file per aggregate (`{Name}Configuration.cs`).

### Step 2 — Remove `DbSet<T>` properties

Delete the explicit `DbSet<Customer>` and `DbSet<Dog>` properties. Repositories use `_context.Set<T>()` directly instead of `_context.Customers` / `_context.Dogs`. EF Core resolves the entity through the model regardless of whether a `DbSet` property exists.

### Step 3 — Retrofit existing repositories

Update `DogRepository` and `CustomerRepository` to use `_context.Set<T>()` for all data access. Verify all existing tests pass.

### Step 4 — Guardrail test

Add an architecture guardrail test that asserts `AppDbContext` declares zero `DbSet<T>` properties — any new one is a build-time signal that the convention was bypassed.

## Acceptance Criteria

- [ ] `OnModelCreating` calls `ApplyConfigurationsFromAssembly` — no individual `ApplyConfiguration` calls remain
- [ ] `AppDbContext` has zero `DbSet<T>` properties
- [ ] `DogRepository` and `CustomerRepository` use `Set<T>()` for data access
- [ ] Architecture guardrail test fails if a `DbSet<T>` property is added to `AppDbContext`
- [ ] `.github/copilot-instructions.md` updated with `Set<T>()` and `ApplyConfigurationsFromAssembly` conventions
- [ ] `docs/guides/developer/folder-structure.md` updated (if aggregate scaffold section exists)
- [ ] `CHANGELOG.md` [Unreleased] section updated
- [ ] All existing tests pass
- [ ] CI passes

## Emotional Guarantees

- A contributor scaffolding a new aggregate never opens an existing file.
- A contributor never worries about forgetting to add a `DbSet` or `ApplyConfiguration` call — the configuration file *is* the registration.
- The scaffold checklist is purely "create files" — no "edit files" step exists.
