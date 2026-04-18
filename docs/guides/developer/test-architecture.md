# Test Architecture & Guardrails

This guide describes the testing strategy for Camp Fit Fur Dogs, including how architectural guardrails are enforced, how test projects are structured, and how contributors should write and organize tests.

The goal is to ensure the architecture remains clean, predictable, and self‑defending as the system grows.

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

The solution uses one test project per layer:

```
tests/
  CampFitFurDogs.Api.Tests/
  CampFitFurDogs.Application.Tests/
  CampFitFurDogs.Domain.Tests/
  CampFitFurDogs.Infrastructure.Tests/
```

Each project tests only its corresponding layer.

### 2.1 Api.Tests
- Endpoint tests
- Request/response mapping tests
- API guardrail tests (e.g., no direct handler/repository usage)

### 2.2 Application.Tests
- Handler tests
- Validator tests
- Dispatcher pipeline tests
- Domain event dispatch tests
- DI guardrail tests
- Auto-registration tests

### 2.3 Domain.Tests
- Entity behavior tests
- Value object tests
- Domain event tests
- Invariant enforcement tests

### 2.4 Infrastructure.Tests
- Repository tests
- Persistence tests
- External system integration tests
- Infrastructure guardrails (e.g., no domain logic)

---

## 3. Guardrail Tests

Guardrail tests enforce architectural purity and prevent regressions.

### 3.1 DI Guardrails

Examples:

- Handlers must be auto-registered.
- Validators must be auto-registered.
- Repositories must be auto-registered.
- No manual DI registration of slice-specific types.

These tests ensure DI conventions remain intact.

### 3.2 Layer Purity Guardrails

Examples:

- Domain must not reference Application or Infrastructure.
- Application must not reference API or Infrastructure implementation details.
- Infrastructure must not contain domain logic.
- API must not reference Application internals.

These tests enforce the dependency graph.

### 3.3 API Guardrails

Examples:

- Endpoints must use dispatchers, not handlers.
- Endpoints must not call repositories.
- Endpoints must not contain domain logic.

### 3.4 Dispatcher Pipeline Guardrails

Examples:

- Commands must be validated.
- Queries must be validated.
- Domain events must be dispatched after command execution.
- Handlers must be resolved via DI.

### 3.5 DTO Purity Guardrails

Examples:

- DTOs must not reference domain entities.
- DTOs must not reference Application internals.
- DTOs must be simple data carriers.

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

Tests are not just for correctness — they are **architectural enforcement tools**.

They ensure:

- The architecture stays clean.
- Conventions remain consistent.
- Contributors cannot accidentally break layering.
- The system remains maintainable as it grows.

Guardrails turn architecture into something the system *defends*, not something developers must remember.

