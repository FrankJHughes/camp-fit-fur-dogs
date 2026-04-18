# ADR-0019: Test Suite Organization — Pure-Reflection vs DI-Dependent Guardrails

| Field     | Value            |
|-----------|------------------|
| Status    | Accepted         |
| Date      | 2026-04-18       |
| Deciders  | Frank Hughes     |

## Context

All architectural guardrail tests lived in `CampFitFurDogs.Api.Tests/Guardrails/`. This mixed two fundamentally different categories:

- **Pure-reflection guardrails** — tests that inspect assemblies via `System.Reflection` only (layer purity, naming conventions, reference scanning). These need no running host, DI container, or database.
- **DI-dependent guardrails** — tests that resolve services from the real DI container via `GuardrailTestBase` (registration completeness, dispatcher wiring, route mapping). These require `WebApplicationFactory` and Testcontainers.

Mixing them in one project meant:

- Pure-reflection tests paid the startup cost of booting the test server and PostgreSQL container.
- Contributors had no clear signal for where to place a new guardrail.
- CI feedback was slower than necessary for structural checks that should run in milliseconds.

## Decision

Split guardrail tests into two projects based on their runtime needs:

| Project | Contents | Dependencies |
|---------|----------|-------------|
| `CampFitFurDogs.Architecture.Tests` | Pure-reflection guardrails + `ReferenceScanner.cs` | xunit, FluentAssertions — no ASP.NET, no Testcontainers |
| `CampFitFurDogs.Api.Tests/Guardrails/` | DI-dependent guardrails + `GuardrailTestBase` + `DiRegistrationScanner.cs` | WebApplicationFactory, Testcontainers |

**Routing rule:** If a guardrail test can run with just assembly reflection (no `Get<T>()`, no `Factory.CreateClient()`), it belongs in `Architecture.Tests`. Otherwise it belongs in `Api.Tests/Guardrails/`.

## Consequences

### Positive

- Pure-reflection guardrails run in milliseconds with no infrastructure overhead.
- Clear, enforceable routing rule for contributor guardrail placement.
- Architecture.Tests can run in CI even when database infrastructure is unavailable.
- Separation makes it easy to add CI stages that run fast guardrails before slow integration tests.

### Negative

- One additional test project to maintain in the solution.
- Guardrails that evolve from pure-reflection to DI-dependent (or vice versa) must be relocated across projects.

### Neutral

- Total test count is unchanged — this is a reorganization, not a coverage change.
- Both projects use the same test framework (xunit) and assertion library (FluentAssertions).