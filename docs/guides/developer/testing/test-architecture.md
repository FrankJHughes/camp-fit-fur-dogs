# Test Architecture & Guardrails (Aligned With Recent Changes)

This guide describes the **testing strategy**, **test layering**, and **architectural guardrail system** for Camp Fit Fur Dogs.  
It has been fully aligned with the **new DI patterns**, **new guardrail boundaries**, **new identity model**, and **recent refactors** (including the removal of unnecessary initialization in guardrails and the separation of DI‑dependent vs pure‑reflection guardrails).

The goal is to ensure the architecture remains **clean, predictable, and self‑defending** as the system grows.

---

# 1. Goals

- Enforce architectural rules automatically  
- Keep layers pure and well‑separated  
- Catch regressions early  
- Ensure vertical slices remain consistent  
- Make tests reflect architecture, not just behavior  
- Provide contributors with clear patterns for writing new tests  
- Ensure guardrails use the **correct test harness** (minimal DI vs full host)  

---

# 2. Test Project Structure

The solution uses **one test project per layer**, plus a dedicated project for **pure‑reflection architectural guardrails**:

```
tests/
    CampFitFurDogs.Api.Tests/
    CampFitFurDogs.Application.Tests/
    CampFitFurDogs.Architecture.Tests/
    CampFitFurDogs.Domain.Tests/
    CampFitFurDogs.Infrastructure.Tests/
```

Each project tests only its corresponding layer, except `Architecture.Tests`, which validates **cross‑cutting structural rules** via pure reflection.

---

## 2.1 Api.Tests (Updated)

Api.Tests contains:

- Endpoint tests (full request → response flow)  
- Request/response mapping tests  
- **DI‑dependent guardrails** using the **new ApiContext + ApiFactory harness**  
- Authentication callback tests (see Authentication Testing Guide)  
- Identity resolution integration tests (external ID → internal ID mapping)  

**Important recent change:**  
Guardrails in this project **must not** use Testcontainers unless the guardrail explicitly tests database connectivity.

---

## 2.2 Application.Tests

- Command handler tests  
- Query handler tests  
- Validator tests  
- Dispatcher pipeline tests  
- Domain event dispatch tests  
- Fake test doubles for slice dependencies  
  - Fake readers for queries  
  - Fake repositories for commands  

**Updated rule:**  
Query handlers must depend on **readers**, not repositories (ADR‑0021).

---

## 2.3 Architecture.Tests (Updated)

Architecture.Tests contains **pure‑reflection guardrails**.  
These tests:

- Use only `System.Reflection`  
- Use `ReferenceScanner` for dependency graph validation  
- Do **not** use DI, Testcontainers, or ASP.NET test host  

Examples:

- Domain must not reference Application or Infrastructure  
- Application must not reference API or Infrastructure  
- DTOs must not reference domain entities  
- Endpoints must not bypass the dispatcher pipeline  
- Query handlers must not depend on repositories  
- Frank must have no upstream dependencies  

**Important recent change:**  
Tests that previously used DI but only needed reflection have been moved here.

---

## 2.4 Domain.Tests

- Entity behavior tests  
- Value object tests  
- Domain event tests  
- Invariant enforcement tests  

Domain tests remain pure and do not use DI or infrastructure.

---

## 2.5 Infrastructure.Tests

- Repository tests (command‑side persistence)  
- Reader tests (query‑side data retrieval)  
- Persistence tests  
- External system integration tests  
- Infrastructure guardrails (e.g., no domain logic)  

**Updated:**  
Infrastructure tests now use the **new ApiContext database harness** when persistence is required.

---

# 3. Guardrail Tests (Updated)

Guardrail tests enforce architectural purity and prevent regressions.  
They fall into **two categories**, aligned with recent changes.

---

## 3.1 DI‑Dependent Guardrails (Api.Tests/Guardrails)

These tests require the **real DI container** and use the **new ApiContext + ApiFactory** harness.

They no longer use:

- Testcontainers (unless testing DB connectivity)  
- ApiWithPostgresTestBase  
- Fake sign‑in endpoints for identity guardrails  

**Identity guardrails now use DefaultHttpContext + HttpContextAccessor.**

### Files (Updated)

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
| `CurrentUserServiceGuardrailTests.cs` | **Updated: now uses DefaultHttpContext, no Testcontainers** |
| `DbContextGuardrailTests.cs` | Npgsql + single DbContext registration |
| `EndpointConventionGuardrailTests.cs` | Every `*Endpoint` implements `IEndpoint` |
| `RouteMappingGuardrailTests.cs` | Route smoke test |
| `TestcontainersGuardrailTests.cs` | Database connectivity smoke test |

**Important recent change:**  
`CurrentUserServiceGuardrailTests` no longer uses `/__test__/sign-in` or a running API host.  
It now uses:

- `DefaultHttpContext`  
- `HttpContextAccessor`  
- `AuthenticatedUserService`  

This aligns with the new identity model and guardrail boundaries.

---

## 3.2 Pure‑Reflection Guardrails (Architecture.Tests)

These tests use only reflection and do not require DI or a running host.

Examples:

- Layering rules  
- DTO purity  
- Handler naming conventions  
- Endpoint purity  
- Dispatcher usage  
- Reader vs repository purity  
- Frank dependency purity  

**Updated:**  
Any guardrail that does not require DI has been moved here.

---

## 3.3 When to Use Which (Updated)

| Need DI container or test server? | Project |
|-----------------------------------|---------|
| **No** — type inspection only | Architecture.Tests |
| **Yes** — DI resolution or endpoint calls | Api.Tests/Guardrails |

**Rule of thumb:**  
If the test can run with only reflection, it belongs in **Architecture.Tests**.  
If it calls `Get<T>()`, `GetAll<T>()`, or `Factory.CreateClient()`, it belongs in **Api.Tests/Guardrails**.

---

# 4. Test Patterns (Aligned)

## 4.1 Black‑Box Testing for Guardrails

Guardrail tests should:

- Assert public behavior, not implementation details  
- Avoid mocking internal types  
- Use **minimal DI** when possible  
- Use **DefaultHttpContext** for identity guardrails  

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

- Use the **new ApiFactory**  
- Test full request → response flow  
- Assert status codes + response shapes  
- Use Testcontainers only when persistence is required  

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
- Using Testcontainers in guardrails unless required  

---

# 6. Contributor Guidelines (Updated)

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
