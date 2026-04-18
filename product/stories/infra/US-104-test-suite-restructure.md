# US-104 — Test Suite Restructure & Central Package Management

## Intent

As a **contributor**, I want the test suite organized by concern (architecture guardrails vs integration tests), all NuGet versions centrally managed, redundant guardrails consolidated, and test helpers cleaned up, so that I can run the right tests in isolation, avoid version drift, and onboard without chasing phantom build warnings.

## Value

- **Faster CI feedback** — pure-reflection guardrails run without DI/database overhead.
- **Zero version drift** — `Directory.Packages.props` is the single source of truth for every NuGet dependency.
- **Eliminates MSB3277** — transitive pinning resolves the EF Core Relational version conflict that produced build warnings.
- **Eliminates CS0618** — `PostgresFixture` uses the current Testcontainers constructor.
- **Fewer files, same coverage** — redundant guardrail tests consolidated via `[Theory]` parameterization.
- **Flat, navigable structure** — no orphaned subfolders or stale scaffolding files.
- **Clearer contributor guidance** — docs reflect the actual test project layout.

## Scope — 4 Commits on `refactor/test-suite-restructure`

### Commit 1 — Architecture.Tests Project (Finding F-1)
- New `CampFitFurDogs.Architecture.Tests` project (net10.0, xunit 2.9.3)
- Relocated 15 pure-reflection guardrail tests from `Api.Tests/Guardrails/`
- Moved `ReferenceScanner.cs` utility to Architecture.Tests
- Updated namespaces: `Api.Tests.Guardrails` → `Architecture.Tests`

### Commit 2 — Central Package Management (Finding F-2)
- Added `Directory.Packages.props` with 17 centrally-managed packages
- Enabled `ManagePackageVersionsCentrally` and `CentralPackageTransitivePinningEnabled`
- Stripped `Version=` attributes from all 11 csproj files
- Normalized 3 version drifts: FluentAssertions 8.3.0→8.9.0, Testcontainers.PostgreSql 4.6.0→4.11.0, xunit.runner.visualstudio 3.1.4→3.1.5

### Commit 3 — Guardrail Consolidation
- Merged 3 `Infrastructure*RegistrationGuardrailTests` into 1 `[Theory]` parameterized by suffix
- Merged `DispatchGuardrailTests` + `DomainEventGuardrailTests` into `DispatcherRegistrationGuardrailTests`
- Removed duplicate `Post_Dogs_ShouldNotReturn404` from `RouteMappingGuardrailTests`
- Guardrails/: 14 → 11 .cs files, zero test coverage loss

### Commit 4 — Test Helper Cleanup & Hygiene
- Flattened `Guardrails/Architecture/` subfolder — moved `DiRegistrationScanner.cs` to `Guardrails/`
- Fixed CS0618: `PostgresFixture` now uses `PostgreSqlBuilder(image)` constructor
- Removed stale `.gitkeep` files from test projects with real content
- Stripped stale `using` directives from consumers

## Acceptance Criteria

- [ ] `CampFitFurDogs.Architecture.Tests` project exists with 15 pure-reflection guardrails and `ReferenceScanner.cs`
- [ ] DI-dependent guardrails remain in `Api.Tests/Guardrails/` with `GuardrailTestBase`
- [ ] `DiRegistrationScanner.cs` lives in `Guardrails/` (flat — no `Architecture/` subfolder)
- [ ] `Directory.Packages.props` manages all 17 NuGet packages centrally
- [ ] `CentralPackageTransitivePinningEnabled` is `true` — no MSB3277 warnings
- [ ] No `Version=` attributes remain on any `PackageReference` in any csproj
- [ ] All version drifts normalized (FluentAssertions, Testcontainers.PostgreSql, xunit.runner.visualstudio)
- [ ] 3 Infrastructure*Registration tests consolidated into 1 `[Theory]`
- [ ] Dispatch + DomainEvent tests consolidated into `DispatcherRegistrationGuardrailTests`
- [ ] Duplicate route mapping test removed
- [ ] CS0618 warning resolved in `PostgresFixture.cs`
- [ ] No stale `.gitkeep` files in test projects with real content
- [ ] `docs/guides/developer/test-architecture.md` updated with Architecture.Tests and guardrail taxonomy
- [ ] `docs/guides/developer/folder-structure.md` Section 1 updated to include Architecture.Tests
- [ ] `CHANGELOG.md` [Unreleased] section updated
- [ ] `.github/copilot-instructions.md` updated with CPM lesson learned
- [ ] All existing tests pass

## Emotional Guarantees

- A contributor never wonders which test project a guardrail belongs in.
- A contributor never debugs a version mismatch that the tooling should have prevented.
- Build output is clean — no warnings that erode trust in CI.