# US-106 — Add-Only Slice Architecture

## Intent

As a **developer**, I want introducing a new vertical slice to require only adding new files — never modifying existing ones — so that slices are fully isolated, merge conflicts are eliminated, and the Open-Closed Principle is enforced at the architecture level.

## Value

- **Zero modification points** — new slices never touch shared files.
- **No merge conflicts** — parallel slice work cannot collide.
- **Faster onboarding** — "drop files in the right folders" is the entire mental model.
- **Architectural proof** — if a slice requires editing a shared file, the architecture has a gap.

## Problem

Today, adding a new slice (e.g., GetCustomerProfile) requires modifying **4 existing files**:

| Modified File | Root Cause |
|---------------|-----------|
| `ICustomerRepository.cs` | Shared repository interface grows with every new read path |
| `CustomerRepository.cs` | Implementation follows the interface |
| `FakeCustomerRepository.cs` | Test fake follows the interface |
| `CustomerEndpoints.cs` | Central route file grows with every new endpoint |

Two root causes produce all four modification points.

## Solution

### Part A: Auto-Discovered Endpoints (ADR-0013)

Each endpoint self-registers its own route via a marker interface:

```csharp
// SharedKernel or Abstractions
public interface IEndpoint
{
    void Map(IEndpointRouteBuilder app);
}
```

A one-time `DiscoverEndpoints()` extension in Program.cs scans the assembly for `IEndpoint` implementations and calls `Map()` on each. Same Scrutor-style pattern already trusted for handlers.

**Result:** New endpoints are add-only. No more editing `CustomerEndpoints.cs` or `DogEndpoints.cs`.

### Part B: Query-Side Readers (ADR-0014)

Queries define their own data access contract inside the slice folder, not on the shared aggregate repository:

```
Application/Customers/GetCustomerProfile/
├── GetCustomerProfileQueryHandler.cs
├── ICustomerProfileReader.cs          ← slice-scoped read contract

Infrastructure/Customers/GetCustomerProfile/
├── CustomerProfileReader.cs           ← slice-scoped implementation
```

Scrutor auto-registers `CustomerProfileReader` → `ICustomerProfileReader` by naming convention. Repositories stay focused on writes (`AddAsync`, `UpdateAsync`, `GetByIdAsync` for command-side rehydration).

**Result:** New query slices never touch `ICustomerRepository`, `CustomerRepository`, or `FakeCustomerRepository`.

### Part C: Retrofit Existing Query Slices

Move the read path from `IDogRepository.GetByIdAsync` into a `IDogProfileReader` inside the ViewDogProfile slice. This validates the pattern and removes the precedent of queries using aggregate repositories.

## File Inventory — Before vs After

### Before (GetCustomerProfile)

| Type | Count |
|------|-------|
| New files | 6 |
| Modified files | 4 |
| **Total touch points** | **10** |

### After (GetCustomerProfile)

| Type | Count |
|------|-------|
| New files | 8 |
| Modified files | 0 |
| **Total touch points** | **8** |

## Deliverables

- [ ] ADR-0013: Auto-Discovered Endpoints
- [ ] ADR-0014: Query-Side Reader Pattern
- [ ] `IEndpoint` interface in SharedKernel or Abstractions
- [ ] `DiscoverEndpoints()` extension method
- [ ] Program.cs wired with `app.DiscoverEndpoints()`
- [ ] Existing endpoints retrofitted to implement `IEndpoint`
- [ ] `ICustomerProfileReader` / `CustomerProfileReader` example (if GetCustomerProfile slice exists)
- [ ] `IDogProfileReader` / `DogProfileReader` retrofit for ViewDogProfile
- [ ] `IDogRepository.GetByIdAsync` removed (read path moved to reader)
- [ ] `FakeDogRepository.GetByIdAsync` removed
- [ ] Guardrail test: every `IEndpoint` implementation is discovered
- [ ] Guardrail test: no `IQueryHandler` depends on an `IRepository` interface
- [ ] `copilot-instructions.md` updated with add-only slice rules
- [ ] `folder-structure.md` updated with reader placement convention
- [ ] `abstractions-contract.md` updated with reader pattern
- [ ] `di-conventions.md` updated with reader auto-registration

## Acceptance Criteria

- [ ] Adding a new query slice requires 0 modifications to existing files
- [ ] Adding a new command slice requires 0 modifications to existing files (once the aggregate's write surface is stable)
- [ ] Adding a new endpoint requires 0 modifications to existing files
- [ ] All existing endpoints implement `IEndpoint` and self-register
- [ ] ViewDogProfile uses `IDogProfileReader`, not `IDogRepository`
- [ ] Scrutor auto-registers all readers by naming convention (no manual DI)
- [ ] `DiscoverEndpoints()` discovers and maps all `IEndpoint` implementations
- [ ] Guardrail test fails if a query handler depends on a repository interface
- [ ] Guardrail test fails if an `IEndpoint` is not discovered
- [ ] ADR-0013 and ADR-0014 accepted and indexed
- [ ] All cross-references in developer docs resolve
- [ ] CI passes

## Emotional Guarantees

- A developer introducing a new slice never worries about breaking existing slices.
- A developer never has to coordinate with another developer over shared file edits.
- A developer can look at the folder and know exactly what files to create — nothing else.

## Dependencies

- US-050 (Unit of Work) — completed (Sprint 5)
- US-051 (CQRS Pipelines) — completed (Sprint 4)
- US-079 (Convention-Based Auto-Registration) — completed (Scrutor)

## Estimated Effort

~4 hours (2 ADRs + infrastructure code + retrofit + docs)