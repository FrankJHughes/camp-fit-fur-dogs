# Test Architecture & Guardrails

This guide describes the testing strategy for Camp Fit Fur Dogs, including how architectural guardrails are enforced, how test projects are structured, and how contributors should write and organize tests.  
The goal is to ensure the architecture remains clean, predictable, and self‑defending as the system grows.

---

# 1. Goals

- Enforce architectural rules automatically  
- Keep layers pure and well‑separated  
- Catch regressions early  
- Ensure vertical slices remain consistent  
- Make tests reflect architecture, not just behavior  
- Provide contributors with clear patterns for writing new tests  

---

# 2. Test Project Structure

The solution uses one test project per layer, plus a dedicated project for pure‑reflection architectural guardrails:

```
tests/
    CampFitFurDogs.Api.Tests/
    CampFitFurDogs.Application.Tests/
    CampFitFurDogs.Architecture.Tests/
    CampFitFurDogs.Domain.Tests/
    CampFitFurDogs.Infrastructure.Tests/
```

Each project tests only its corresponding layer, except `Architecture.Tests`, which validates cross‑cutting structural rules via pure reflection.

---

## 2.1 Api.Tests

- Endpoint tests (full request → response flow)  
- Request/response mapping tests  
- **DI‑dependent guardrails** — tests requiring the real DI container via `GuardrailTestBase`  
- Authentication callback tests (see Authentication Testing Guide)  

---

## 2.2 Application.Tests

- Handler tests (command and query)  
- Validator tests  
- Dispatcher pipeline tests  
- Domain event dispatch tests  
- Fake test doubles for slice dependencies  
  - Fake readers for query handlers  
  - Fake repositories for command handlers  

---

## 2.3 Architecture.Tests

- **Pure‑reflection guardrails** — assembly‑level rules  
- `ReferenceScanner.cs` for dependency graph validation  
- No dependency on ASP.NET test host or Testcontainers  

Examples:

- Domain must not reference Application or Infrastructure  
- Application must not reference API or Infrastructure  
- Handlers must follow naming conventions  
- DTOs must not reference domain entities  
- Endpoints must not bypass the dispatcher pipeline  
- Query handlers must not depend on repositories (ADR‑0021)  
- Frank must have no upstream dependencies  

---

## 2.4 Domain.Tests

- Entity behavior tests  
- Value object tests  
- Domain event tests  
- Invariant enforcement tests  

---

## 2.5 Infrastructure.Tests

- Repository tests (command‑side persistence)  
- Reader tests (query‑side data retrieval)  
- Persistence tests  
- External system integration tests  
- Infrastructure guardrails (e.g., no domain logic)  

---

# 3. Guardrail Tests

Guardrail tests enforce architectural purity and prevent regressions.  
They fall into two categories based on runtime needs.

---

## 3.1 DI‑Dependent Guardrails (Api.Tests/Guardrails)

These tests require the real DI container.  
They inherit from `GuardrailTestBase`, which boots the test server via `CampFitFurDogsApiFactory` and exposes `Get<T>()` / `GetAll<T>()`.

**Files (14):**

| File | Purpose |
|------|---------|
| `GuardrailTestBase.cs` | DI scope helpers |
| `DiRegistrationScanner.cs` | Assembly + DI resolution scanner |
| `InfrastructureRegistrationGuardrailTests.cs` | Repository/Service/Provider registration |
| `ReaderRegistrationGuardrailTests.cs` | Reader types registered via Scrutor |
| `DispatcherRegistrationGuardrailTests.cs` | Command + DomainEvent dispatchers registered |
| `NoManualHandlerRegistrationGuardrailTests.cs` | Handlers registered via Scrutor only |
| `NoManualInfrastructureRegistrationGuardrailTests.cs` | Infra types registered via Scrutor only |
| `NoDuplicateServiceRegistrationGuardrailTests.cs` | No duplicate DI registrations |
| `DomainEventHandlerRegistrationGuardrailTests.cs` | Domain event handlers registered |
| `CurrentUserServiceGuardrailTests.cs` | TestCurrentUserService wiring |
| `DbContextGuardrailTests.cs` | Npgsql + single DbContext registration |
| `EndpointConventionGuardrailTests.cs` | Every `*Endpoint` implements `IEndpoint` |
| `RouteMappingGuardrailTests.cs` | Route smoke test |
| `TestcontainersGuardrailTests.cs` | Database connectivity smoke test |

---

## 3.2 Pure‑Reflection Guardrails (Architecture.Tests)

These tests use only `System.Reflection` and `ReferenceScanner`.  
They do **not** require:

- A running host  
- A DI container  
- A database  

They are fast, isolated, and enforce structural purity.

Examples:

- Domain → no references to Application or Infrastructure  
- Application → no references to API or Infrastructure  
- DTOs → no domain entities  
- Endpoints → must use dispatcher pipeline  
- Query handlers → must depend on readers, not repositories  
- Frank → must have no upstream dependencies  

---

## 3.3 When to Use Which

| Need DI container or test server? | Project |
|-----------------------------------|---------|
| **No** — type inspection only | Architecture.Tests |
| **Yes** — DI resolution or endpoint calls | Api.Tests/Guardrails |

**Rule of thumb:**  
If the test can run with only reflection, it belongs in **Architecture.Tests**.  
If it calls `Get<T>()`, `GetAll<T>()`, or `Factory.CreateClient()`, it belongs in **Api.Tests/Guardrails**.

---

# 4. Test Patterns

## 4.1 Black‑Box Testing for Guardrails

Guardrail tests should:

- Assert public behavior, not implementation details  
- Avoid mocking internal types  
- Use reflection only when necessary  

---

## 4.2 Handler Tests

### Command Handler Tests

- Test business logic in isolation  
- Mock repositories or external services  
- Do **not** test validation (belongs to validator tests)  
- Inject `FakeUnitOfWork`  
- Assert:  
  - `Committed == true`  
  - `CommitCount` matches expected behavior  

### Query Handler Tests

- Inject fake readers (never repositories)  
- Assert mapping to result DTO  
- Do **not** inject `FakeUnitOfWork` — queries have no side effects  

---

## 4.3 Validator Tests

Validator tests should:

- Test validation rules explicitly  
- Cover valid + invalid cases  

---

## 4.4 Domain Tests

Domain tests should:

- Test invariants  
- Test domain event raising  
- Test value object equality + behavior  

---

## 4.5 API Tests

API tests should:

- Use the test server factory  
- Test full request → response flow  
- Assert status codes + response shapes  

---

# 5. Test Data & Fixtures

Tests may use:

- Builders  
- Factory methods  
- Test doubles  
  - Fake repositories (commands)  
  - Fake readers (queries)  
- In‑memory repositories (Infrastructure tests)  

Avoid:

- Sharing mutable state  
- Overusing mocks (especially in guardrails)  

---

# 6. Contributor Guidelines

## When adding new architecture

- Add or update guardrail tests  
- Pure‑reflection guardrails → `Architecture.Tests`  
- DI‑dependent guardrails → `Api.Tests/Guardrails`  
- Ensure purity rules remain enforced  
- Update documentation (e.g., `purity-rules.md`)  

## When adding new features

- Add handler tests  
  - Fake readers for queries  
  - Fake repositories for commands  
- Add validator tests  
- Add domain tests  
- Add endpoint tests  
- Ensure guardrails still pass  
- Command handler tests must verify:  
  - `IUnitOfWork.CommitAsync()` is called on success  
  - Not called on validation/guard failures  
- Query handler tests must **not** use `FakeUnitOfWork`  

## When modifying existing architecture

- Update guardrails accordingly  
- Ensure dependency graph remains clean  
- Update folder structure tests if needed  

---

# 7. Philosophy

Tests are not just for correctness — they are **architectural enforcement tools**.

They ensure:

- Architecture stays clean  
- Conventions remain consistent  
- Contributors cannot accidentally break layering  
- The system remains maintainable as it grows  

Guardrails turn architecture into something the system **defends**, not something developers must remember.

---

# Related Documentation

- **[Authentication Testing](../authentication-testing.md)**  
- **[Integration Testing](../integration-testing.md)**  
- **[Frontend Testing](../frontend-testing.md)**  
- **[Purity Rules](../purity-rules.md)**  
- **[Dispatcher Pipeline](../dispatcher-pipeline.md)**  
- **[Abstractions Contract](../abstractions-contract.md)**
