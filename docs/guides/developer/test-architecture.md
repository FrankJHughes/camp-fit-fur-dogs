# Test Architecture & Guardrails

This guide describes the testing strategy for Camp Fit Fur Dogs, including how architectural guardrails are enforced, how test projects are structured, and how contributors should write and organize tests. The goal is to ensure the architecture remains clean, predictable, and self‑defending as the system grows.

---

## 1. Goals

- Enforce architectural rules automatically.
- Keep layers pure and well-separated.
- Catch regressions early.
- Ensure vertical slices remain consistent.
- Make tests reflect architecture, not just behavior.
- Provide contributors with clear patterns for writing new tests.

---

## 2. Test Project Structure

The solution uses one test project per layer, plus a dedicated project for pure-reflection architectural guardrails:

```
tests/
    CampFitFurDogs.Api.Tests/
    CampFitFurDogs.Application.Tests/
    CampFitFurDogs.Architecture.Tests/
    CampFitFurDogs.Domain.Tests/
    CampFitFurDogs.Infrastructure.Tests/
```

Each project tests only its corresponding layer, except `Architecture.Tests` which validates cross-cutting structural rules via pure reflection.

### 2.1 Api.Tests

- Endpoint tests (full request → response flow)
- Request/response mapping tests
- **DI-dependent guardrails** — tests that need the real DI container via `GuardrailTestBase` (see §3.1)

### 2.2 Application.Tests

- Handler tests
- Validator tests
- Dispatcher pipeline tests
- Domain event dispatch tests

### 2.3 Architecture.Tests

- **Pure-reflection guardrails** — tests that inspect assemblies without a running host (see §3.2)
- `ReferenceScanner.cs` utility for assembly-reference validation
- No dependency on `Microsoft.AspNetCore.Mvc.Testing` or Testcontainers

### 2.4 Domain.Tests

- Entity behavior tests
- Value object tests
- Domain event tests
- Invariant enforcement tests

### 2.5 Infrastructure.Tests

- Repository tests
- Persistence tests
- External system integration tests
- Infrastructure guardrails (e.g., no domain logic)

---

## 3. Guardrail Tests

Guardrail tests enforce architectural purity and prevent regressions. They fall into two categories based on their runtime needs.

### 3.1 DI-Dependent Guardrails (Api.Tests/Guardrails/)

These tests need the real DI container to resolve services. They inherit from `GuardrailTestBase`, which boots the test server via `CampFitFurDogsApiFactory` and exposes `Get<T>()` / `GetAll<T>()` helpers.

**Files (12):**

| File | Purpose |
|------|---------|
| `GuardrailTestBase.cs` | Abstract base — DI scope helpers |
| `DiRegistrationScanner.cs` | Static scanner — assembly + DI resolution |
| `InfrastructureRegistrationGuardrailTests.cs` | `[Theory]` — Repository/Service/Provider registered |
| `DispatcherRegistrationGuardrailTests.cs` | Command + DomainEvent dispatchers registered |
| `NoManualHandlerRegistrationGuardrailTests.cs` | Handlers registered via Scrutor only |
| `NoManualInfrastructureRegistrationGuardrailTests.cs` | Infra types registered via Scrutor only |
| `NoDuplicateServiceRegistrationGuardrailTests.cs` | No duplicate DI registrations |
| `DomainEventHandlerRegistrationGuardrailTests.cs` | Domain event handlers registered |
| `CurrentUserServiceGuardrailTests.cs` | TestCurrentUserService wiring |
| `DbContextGuardrailTests.cs` | Npgsql + single DbContext registration |
| `RouteMappingGuardrailTests.cs` | Route smoke test |
| `TestcontainersGuardrailTests.cs` | Database connectivity smoke test |

### 3.2 Pure-Reflection Guardrails (Architecture.Tests/)

These tests use only `System.Reflection` and `ReferenceScanner` to inspect assemblies. They do **not** need a running host, DI container, or database — making them fast and isolated.

Examples:

- Domain must not reference Application or Infrastructure.
- Application must not reference API or Infrastructure.
- Handlers must follow naming conventions.
- DTOs must not reference domain entities.
- Endpoints must not bypass the dispatcher pipeline.
- SharedKernel must have no upstream dependencies.

### 3.3 When to Use Which

| Need the DI container or test server? | Project |
|---------------------------------------|---------|
| **No** — inspecting types, references, naming | Architecture.Tests |
| **Yes** — resolving services, hitting endpoints | Api.Tests/Guardrails/ |

**Rule of thumb:** if the test can run with just assembly reflection, it belongs in Architecture.Tests. If it calls `Get<T>()`, `GetAll<T>()`, or `Factory.CreateClient()`, it belongs in Api.Tests/Guardrails/.

---

## 4. Test Patterns

### 4.1 Black-Box Testing for Guardrails

Guardrail tests should:

- Assert on public behavior, not implementation details.
- Avoid mocking internal types.
- Use reflection only when necessary to inspect assemblies.

### 4.2 Handler Tests

Handler tests should:

- Test business logic in isolation.
- Mock repositories or external services.
- Avoid testing validation (that belongs to validator tests).
- Inject `FakeUnitOfWork` alongside fake repositories. Assert `Committed` is `true` and verify `CommitCount` for commit behavior.

### 4.3 Validator Tests

Validator tests should:

- Test validation rules explicitly.
- Cover both valid and invalid cases.

### 4.4 Domain Tests

Domain tests should:

- Test invariants.
- Test domain event raising.
- Test value object equality and behavior.

### 4.5 API Tests

API tests should:

- Use the test server factory.
- Test full request → response flow.
- Assert status codes and response shapes.

---

## 5. Test Data & Fixtures

Tests may use:

- Builders
- Factory methods
- Test doubles
- In-memory repositories (for Infrastructure tests)

Avoid:

- Sharing mutable state across tests
- Overusing mocks (especially in guardrails)

---

## 6. Contributor Guidelines

When adding new architecture:

- Add or update guardrail tests.
- Pure-reflection guardrails → `Architecture.Tests`.
- DI-dependent guardrails → `Api.Tests/Guardrails/`.
- Ensure purity rules are enforced.
- Update documentation (e.g., `purity-rules.md`).

When adding new features:

- Add handler tests.
- Add validator tests.
- Add domain tests.
- Add endpoint tests.
- Ensure existing guardrails still pass.
- Handler tests must verify `IUnitOfWork.CommitAsync` is called on the happy path and not called on validation/guard failures.

When modifying existing architecture:

- Update guardrails accordingly.
- Ensure the dependency graph remains clean.
- Update folder structure tests if needed.

---

## 7. Philosophy

Tests are not just for correctness — they are **architectural enforcement tools**. They ensure:

- The architecture stays clean.
- Conventions remain consistent.
- Contributors cannot accidentally break layering.
- The system remains maintainable as it grows.

Guardrails turn architecture into something the system *defends*, not something developers must remember.