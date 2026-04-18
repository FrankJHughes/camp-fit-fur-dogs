# US-104 — Test Suite Restructure & Central Package Management

## Intent

As a **contributor**, I want the test suite organized by concern (architecture guardrails vs integration tests) and all NuGet versions centrally managed, so that I can run the right tests in isolation, avoid version drift, and onboard without chasing phantom build warnings.

## Value

- **Faster CI feedback** — pure-reflection guardrails run without DI/database overhead.
- **Zero version drift** — `Directory.Packages.props` is the single source of truth for every NuGet dependency.
- **Eliminates MSB3277** — transitive pinning resolves the EF Core Relational version conflict that produced build warnings.
- **Clearer contributor guidance** — docs reflect the actual test project layout.

## Acceptance Criteria

- [ ] `CampFitFurDogs.Architecture.Tests` project exists with 15 pure-reflection guardrails and `ReferenceScanner.cs`
- [ ] DI-dependent guardrails (`DomainEventGuardrailTests`, `RouteMappingGuardrailTests`, + 11 others) remain in `Api.Tests` with `GuardrailTestBase`
- [ ] `DiRegistrationScanner.cs` remains in `Api.Tests/Guardrails/Architecture/`
- [ ] `Directory.Packages.props` manages all 17 NuGet packages centrally
- [ ] `CentralPackageTransitivePinningEnabled` is `true` — no MSB3277 warnings
- [ ] No `Version=` attributes remain on any `PackageReference` in any csproj
- [ ] All version drifts normalized (FluentAssertions, Testcontainers.PostgreSql, xunit.runner.visualstudio)
- [ ] `docs/guides/developer/test-architecture.md` updated to include Architecture.Tests project and pure-reflection vs DI-dependent guardrail taxonomy
- [ ] `docs/guides/developer/folder-structure.md` Section 1 updated to include Architecture.Tests; contributor guideline updated
- [ ] `CHANGELOG.md` [Unreleased] section updated with test restructuring and CPM entries
- [ ] `.github/copilot-instructions.md` updated if any standing rules reference the old test layout
- [ ] All existing tests pass (131+)

## Emotional Guarantees

- A contributor never wonders which test project a guardrail belongs in.
- A contributor never debugs a version mismatch that the tooling should have prevented.
- Build output is clean — no warnings that erode trust in CI.