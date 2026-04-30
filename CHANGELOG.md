# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased Sprint 7]

### Added
- Integration testing infrastructure using Neon branch databases
- GitHub Actions workflow for automated integration tests on PRs
- Local integration test runner script
- EF migration application step in CI
- Developer guide for running integration tests locally
- The Camp Fit Fur Dogs API is now hosted and available over HTTPS, enabling external access to dog profiles, registration flows, and future customer‑facing features.
- Automatic deployment is now active for all updates merged into the main branch, ensuring the latest functionality is always available.
- A public health check endpoint (`/health`) is now exposed for system monitoring and uptime verification.

## [Sprint 6] — 2026-04-27

### Completed Stories

| Story  | Title                              | Issue |
|--------|------------------------------------|-------|
| US-030 | Edit Dog Profile                   | #172  |
| US-031 | List Dogs by Current User          | #177  |
| US-032 | Remove Dog                         | #173  |
| US-035 | Invalid Info Validation             | #176  |
| US-037 | Missing Dog / NotFound              | #175  |
| US-038 | Confirm Destructive Action          | #174  |
| US-108 | Foundation Extraction               | #171  |

### Added
### Added
- Edit Dog Profile page — owners can update their dog's name, breed, date of birth, and sex (US-030)
- My Dogs page# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added

## [Sprint 6] — 2026-04-27

### Completed Stories

| Story  | Title                              | Issue |
|--------|------------------------------------|-------|
| US-030 | Edit Dog Profile                   | #172  |
| US-031 | List Dogs by Current User          | #177  |
| US-032 | Remove Dog                         | #173  |
| US-035 | Invalid Info Validation            | #176  |
| US-037 | Missing Dog / NotFound             | #175  |
| US-038 | Confirm Destructive Action         | #174  |
| US-108 | Foundation Extraction              | #171  |

### Added
- Edit Dog Profile page — owners can update their dog's name, breed, date of birth, and sex (US-030)
- Edit button on View Dog Profile navigates to the edit form (US-030)
- My Dogs page — owners see all their registered dogs at `/dogs` with quick links to view, edit, or register a new dog (US-031)
- Remove Dog — owners can remove a dog from their profile with a confirmation step to prevent accidents (US-032)
- Form validation — clear, accessible inline error messages guide owners when required fields are missing or invalid (US-035)
- Confirmation dialog — destructive actions like removing a dog prompt for confirmation before proceeding; Escape key and focus behavior default to the safe choice (US-038)
- Friendly not-found page — owners who navigate to a dog that doesn't exist see a helpful message with links back to their dogs (US-037)
- Reusable backend building blocks extracted into a shared library for faster future development (US-108)

## [Sprint 5] — 2026-04-18

### Completed Stories

| Story  | Title                                      | Issue |
|--------|--------------------------------------------|-------|
| US-029 | View Dog Profile (Frontend)                | #153  |
| US-049 | Secure Password Hashing                    | #154  |
| US-050 | Unit of Work                               | #155  |
| US-052 | Developer Guide: Feature Slice Walkthrough | #150  |
| US-104 | Architecture Test Consolidation            | #159  |
| US-105 | Doc Debloat                                | #160  |
| US-106 | Add-Only Slice Architecture                | #163  |
| US-107 | EF Entity Auto-Discovery                   | #165  |

### Added
- View Dog Profile frontend page — owners can view a dog's full profile at `/dogs/[id]` (US-029)
- Feature slice walkthrough — step-by-step TDD guide for adding new command and query slices (US-052)
- Endpoint auto-discovery — new endpoints are registered automatically by convention (US-106; ADR-0020)
- Query-side reader isolation — query handlers use dedicated read models instead of full domain repositories (US-106; ADR-0021)
- EF entity auto-discovery — new entities and their configurations are picked up automatically without manual wiring (US-107)

### Changed
- Password hashing upgraded to industry-standard BCrypt (US-049)
- Architecture tests consolidated and centrally managed for consistency (US-104)
- NuGet dependencies centrally managed with version pinning (US-104)
- Documentation streamlined — stale and duplicated content removed (US-105)

## [Sprint 4] — 2026-04-18

### Completed Stories

| Story  | Title                                | Issue |
|--------|--------------------------------------|-------|
| US-051 | CQRS Command/Query Pipelines        | #115  |
| US-055 | Frontend Technology Decision         | —     |
| US-056 | Next.js Project Scaffold            | —     |
| US-079 | Convention-Based Auto-Registration   | #120  |
| US-084 | Register Dog Page                    | #118  |
| US-102 | API Client Layer                     | #123  |
| US-103 | Frontend Testing Setup               | #124  |

### Added
- Register Dog page — owners can register a new dog through the web interface (US-084)
- Frontend application scaffold with API proxy and health-check landing page (US-056)
- Frontend API client with typed error handling (US-102)
- Frontend test infrastructure (US-103)
- CQRS command and query pipeline architecture (US-051; ADR-0011)
- Frontend technology decision — React with Next.js (US-055; ADR-0012)
- Living conventions document for contributor alignment
- Convention-based service registration — new services are wired automatically by naming convention (US-079)

### Changed
- Dog registration resolves owner identity server-side instead of requiring it in the request (US-084)
- Dev container updated for test container compatibility

## [Sprint 3] — 2026-04-11

### Completed Stories

| Story  | Title                        | Issue |
|--------|------------------------------|-------|
| US-009 | Developer Contributor Guide  | #98   |
| US-027 | Create Customer Account      | #95   |
| US-028 | Register Dog                 | #96   |
| US-029 | View Dog Profile             | #97   |
| US-045 | Product Owner Workflow Guide | #99   |
| US-046 | Scrum Master Workflow Guide  | #100  |

### Added
- Create Customer Account — new owners can create an account (US-027)
- Register Dog — owners can register a dog under their account (US-028)
- View Dog Profile — owners can view a dog's details with ownership verification (US-029)
- Developer, Product Owner, and Scrum Master contributor guides (US-009, US-045, US-046)
- Pre-push hook preventing direct pushes to main branch
- Backlog stories US-047 through US-050

### Changed
- README updated with milestone progress and navigation improvements
- Portfolio documents updated with milestone-driven structure

## [Sprint 2] — 2026-04-08

### Completed Stories

| Story | Title                             | Issue |
|-------|-----------------------------------|-------|
| US-002 | Containerized Dev Environment    | #58  |
| US-003 | Consistent Editor Experience     | #59  |
| US-004 | Standardized Developer Commands  | #57  |
| US-005 | One-Command Local Bootstrap      | #55  |
| US-008 | Doc Audit & Defragmentation      | —    |
| US-012 | Story Naming Convention          | —    |
| — owners see all their registered dogs at `/dogs` with quick links to view, edit, or register a new dog (US-031)
- Remove Dog — owners can remove a dog from their profile with a confirmation step to prevent accidents (US-032)
- Form validation — clear, accessible inline error messages guide owners when required fields are missing or invalid (US-035)
- Confirmation dialog — destructive actions like removing a dog prompt for confirmation before proceeding; Escape key and focus behavior default to the safe choice (US-038)
- Friendly not-found page — owners who navigate to a dog that doesn't exist see a helpful message with links back to their dogs (US-037)
- Foundation extraction — reusable backend building blocks extracted into a shared library for faster future development (US-108)

## [Sprint 5] — 2026-04-18

### Completed Stories

| Story  | Title                                      | Issue |
|--------|--------------------------------------------|-------|
| US-029 | View Dog Profile (Frontend)                | #153  |
| US-049 | Secure Password Hashing                    | #154  |
| US-050 | Unit of Work                               | #155  |
| US-052 | Developer Guide: Feature Slice Walkthrough | #150  |
| US-104 | Architecture Test Consolidation            | #159  |
| US-105 | Doc Debloat                                | #160  |
| US-106 | Add-Only Slice Architecture                | #163  |
| US-107 | EF Entity Auto-Discovery                   | #165  |

### Added
- `feature-slice-walkthrough.md` — step-by-step TDD walkthrough for adding command and query slices (US-052)
- `AppDbContextAutoDiscoveryGuardrailTests` — guardrail ensuring no `DbSet<T>` properties on `AppDbContext` (US-107)
- `GetDogProfileReaderTests` — 3 integration tests: profile found, not found, wrong owner (US-107)
- `IEndpoint` interface with static abstract `Map` method — assembly-scanned endpoint auto-discovery (US-106; ADR-0020)
- `IGetDogProfileReader` / `GetDogProfileReader` — query-side reader isolation, query handlers no longer depend on repositories (US-106; ADR-0021)
- `FakeGetDogProfileReader` test double for query handler unit tests (US-106)
- Architecture guardrail: query handlers must not depend on repository interfaces (US-106)
- Architecture guardrail: every `*Endpoint` class must implement `IEndpoint` (US-106)
- Architecture guardrail: at least one `IEndpoint` implementation exists (US-106)
- Scrutor scan for `Reader` suffix in Infrastructure DI (US-106)
- ADR-0020: Endpoint Auto-Discovery via IEndpoint (US-106)
- ADR-0021: Query-Side Reader Isolation (US-106)

- View Dog Profile frontend slice — `/dogs/[id]` page with 13 tests (US-029; see `frontend/CHANGELOG.md`)
- `BCrypt.Net-Next` NuGet package dependency in Domain layer (#154)
- `CampFitFurDogs.Architecture.Tests` project — 15 pure-reflection guardrails and `ReferenceScanner.cs` relocated from Api.Tests (US-104)
- `Directory.Packages.props` — Central Package Management for all 17 NuGet dependencies with transitive pinning (US-104)

### Changed
- `AppDbContext` uses `ApplyConfigurationsFromAssembly` — eliminates per-entity `ApplyConfiguration` calls (US-107)
- `DogRepository` + `CustomerRepository` use `Set<T>()` — eliminates dependency on `DbSet<T>` properties (US-107)
- `GetDogProfileReader` uses `Set<T>()` instead of `DbSet<T>` property (US-107)
- ADR-0015 amended with EF configuration auto-discovery scope (US-107)
- `copilot-instructions.md` — added EF Core Conventions section (US-107)
- `di-conventions.md` — added Section 6: EF Entity Configuration Conventions (US-107)
- `folder-structure.md` — added `Configuration.cs` to slice anatomy, Infrastructure template, and contributor steps (US-107)
- `GetDogProfileHandler` depends on `IGetDogProfileReader` instead of `IDogRepository` (US-106)
- Endpoint classes implement `IEndpoint`; group files (`CustomerEndpoints.cs`, `DogEndpoints.cs`) eliminated (US-106)
- `Endpoints.MapEndpoints()` uses assembly scanning instead of manual wiring (US-106)

- `PasswordHash` value object uses BCrypt (`BCrypt.Net-Next`) instead of base64 encoding; added `Create()` and `Verify()` methods (#154)
- `CreateCustomerHandler` delegates hashing to `PasswordHash.Create()` — removed inline `HashPassword()` helper (#154)
- DI-dependent guardrails remain in `Api.Tests/Guardrails/`; pure-reflection guardrails moved to `Architecture.Tests` (US-104)
- 3 `Infrastructure*RegistrationGuardrailTests` consolidated into 1 `[Theory]` parameterized by suffix (US-104)
- `DispatchGuardrailTests` + `DomainEventGuardrailTests` merged into `DispatcherRegistrationGuardrailTests` (US-104)
- Duplicate `Post_Dogs_ShouldNotReturn404` removed from `RouteMappingGuardrailTests` (US-104)
- `DiRegistrationScanner.cs` flattened from `Guardrails/Architecture/` to `Guardrails/` (US-104)
- `PostgresFixture` updated to use `PostgreSqlBuilder(image)` constructor — resolves CS0618 (US-104)
- 3 version drifts normalized: FluentAssertions 8.3.0→8.9.0, Testcontainers.PostgreSql 4.6.0→4.11.0, xunit.runner.visualstudio 3.1.4→3.1.5 (US-104)
- All `Version=` attributes stripped from 11 csproj files — versions now managed centrally (US-104)

### Removed
- `DbSet<Customer>` and `DbSet<Dog>` properties from `AppDbContext` — entity access now uses `Set<T>()` (US-107)

- Stale `.gitkeep` files from test projects containing real content (US-104)
- Orphaned `Guardrails/Architecture/` subfolder — sole file flattened up (US-104)
- 5 redundant guardrail test files replaced by 2 consolidated files (US-104)

## [Sprint 4] — 2026-04-18

### Completed Stories

| Story  | Title                        | Issue |
|--------|------------------------------|-------|
| US-051 | CQRS Command/Query Pipelines | #115  |
| US-055 | Frontend Technology Decision | —     |
| US-056 | Next.js Project Scaffold     | —     |
| US-079 | Convention-Based Auto-Registration | #120  |
| US-083 | API Client Layer                   | #117  |
| US-084 | Register Dog Page            | #118  |
| US-102 | API Client Layer                   | #123  |
| US-103 | Frontend Testing Setup             | #124  |

### Added
- `.github/copilot-instructions.md` — living conventions document (standing rules, PR conventions, architecture patterns, tooling, lessons learned)
- Sprint review closing checklist in `docs/sprint-reviews/_template.md`
- Conventions maintenance sections in all three contributor guides
- ADR-0011: CQRS Command/Query Pipelines (US-051)
- ADR-0012: Frontend Technology — React with Next.js (US-055)
- `.gitattributes` — enforces LF line endings repo-wide; eliminates CRLF phantom diffs in Dev Container
- First-Time Setup section in `docs/guides/developer-guide.md` (Docker Desktop auto-start, Git identity, hooks)
- Frontend API client with typed error handling and unit test suite (see `frontend/CHANGELOG.md`)
- `ICurrentUserService` abstraction for server-side identity resolution (#118)
- `RegisterDogRequest` API DTO — request body no longer includes `OwnerId` (#118)
- `DummyCurrentUserService` pre-auth placeholder in Infrastructure (#118)
- `ApiTestHelpers` shared test utilities for owner/dog creation (#118)
- Next.js project scaffold with API proxy and health-check landing page (US-056)

### Changed

- Frontend relocated from `src/frontend/` to `frontend/src/` for role-based monorepo layout
- `POST /api/dogs` endpoint resolves owner identity from `ICurrentUserService` instead of request body (#118)
- `.devcontainer/devcontainer.json` — adds `TESTCONTAINERS_RYUK_DISABLED` and `TESTCONTAINERS_HOST_OVERRIDE` for docker-outside-of-docker Testcontainers compatibility
- Root `.gitignore` — moves `node_modules/` and `.next/` to `frontend/src/.gitignore`; adds scratch file exclusions

## [Sprint 3] — 2026-04-11

### Completed Stories

| Story  | Title                        | Issue |
|--------|------------------------------|-------|
| US-009 | Developer Contributor Guide  | #98   |
| US-027 | Create Customer Account      | #95   |
| US-028 | Register Dog                 | #96   |
| US-029 | View Dog Profile             | #97   |
| US-045 | Product Owner Workflow Guide | #99   |
| US-046 | Scrum Master Workflow Guide  | #100  |

### Added
- `POST /api/customers` — create customer account (US-027)
- `POST /api/dogs` — register a dog under a customer (US-028)
- `GET /api/dogs/{id}` — view dog profile with ownership guard (US-029)
- CQRS command pipeline: `ICommand`, `ICommandHandler`, `ICommandDispatcher`
- CQRS query pipeline: `IQuery<TResponse>`, `IQueryHandler`, `IQueryDispatcher`
- `Endpoints.cs` single entry point with `MapGroup` consolidation
- `docs/guides/developer-guide.md` (US-009)
- `docs/guides/product-owner-guide.md` (US-045)
- `docs/guides/scrum-master-guide.md` (US-046)
- `CONTRIBUTING.md` rewritten as role-routing hub
- Backlog stories US-047 through US-050
- `hooks/pre-push` — local guardrail blocking direct pushes to `main`
- Developer guide: "Source control safety" section with two-layer protection model

### Changed

- README: milestone progress table, clickable file paths, dx.ps1 → CLI commands
- `portfolio/USE-CASES.md` and `portfolio/SURFACING-STRATEGY.md`: milestone-driven rewrite, clickable paths
- `docs/README.md`: added Roadmap section
- Developer guide: added TDD section, fixed dx.ps1 references

## [Sprint 2] — 2026-04-08

### Completed Stories

| Story | Title | Issue |
|-------|-------|-------|
| US-002 | Containerized Dev Environment | #58 |
| US-003 | Consistent Editor Experience | #59 |
| US-004 | Standardized Developer Commands | #57 |
| US-005 | One-Command Local Bootstrap | #55 |
| US-008 | Doc Audit & Defragmentation | — |
| US-012 | Story Naming Convention | — |
| US-020 | Merge Protection Governance | #47 |
| US-022 | Planning Runbook (original scope) | #49 |
| US-024 | Planning Conventions README | — |
| US-025 | DX Architecture Decision | #54 |
| US-026 | Declarative Infra Dependencies | #56 |

### Added
- ADR-0008: Consistent Editor Experience
- ADR-0009: Story Naming Convention
- ADR-0010: Retire Planning YAML Infrastructure
- `.devcontainer/` configuration (Dockerfile, devcontainer.json, docker-compose.yml)
- `.editorconfig` and `.vscode/` settings (extensions, launch, settings, tasks)
- `Makefile` with standardized developer commands
- `compose.yml` for infrastructure (PostgreSQL, Seq, MailHog)
- `bootstrap.ps1` and `bootstrap.sh` for one-command local setup
- `docs/README.md` navigation hub
- `docs/sprint-reviews/` with template and Sprint 2 review
- `CHANGELOG.md`
- `CODEOWNERS`
- `scripts/configure-branch-protection.sh`

### Changed

- All 44 stories renamed to `US-{NNN}-{kebab-name}.md` convention (ADR-0009)
- CONTRIBUTING.md rewritten with 2-step story workflow
- `docs/governance/governance.md` fixed 6 stale cross-references
- US-006 directory convention fixed (`features/` to `customer/`)
- US-009 scope path fixed (`docs/contributing/` to `docs/guides/`)
- US-022 expanded to absorb US-010 and US-011 (19 to 42 AC)
- README.md updated with Current Status section

### Removed

- `planning/` directory (36 YAMLs, 3 manifests, 1 epic, 1 README)
- `.github/scripts/` (7 stale bootstrap scripts)
- `docs/process/`, `docs/ci/`, `docs/decisions/` (stale YAML-era docs)
- `docs/changelog.md`, `docs/CONTRIBUTING.md`, `docs/governance.md`,
  `docs/runbook.md` (duplicates consolidated into canonical locations)
- `docs/governance/story-yaml-governance.md` (YAML-era governance)

### Retired / Absorbed

- US-021: Post-Merge Sprint Bootstrap — retired (obsoleted by ADR-0010)
- US-023: Sprint Manifest Template — retired (obsoleted by ADR-0010)
- US-010: PO Contributor Guide — absorbed into US-022
- US-011: SM Contributor Guide — absorbed into US-022

## [Sprint 1] — 2026-04-03

### Completed Stories

| Story | Title | Issue |
|-------|-------|-------|
| US-016 | ADR Foundation | #17 |
| US-017 | Shared Kernel DDD Building Blocks | #18 |
| US-018 | Domain Model Core Aggregates | #19 |
| US-019 | API DDD Layer Wiring | #20 |

### Added
- ADR-0003: Developer Experience Toolchain
- ADR-0004: Docker Compose for Infrastructure Definitions
- ADR-0005: Makefile for Standardized Developer Commands
- ADR-0006: Dev Container for Containerized Development Environment
- ADR-0007: One-Command Local Bootstrap
- `src/SharedKernel/` — Entity, AggregateRoot, ValueObject,
  IDomainEvent, IRepository base types
- `src/CampFitFurDogs.Domain/Dogs/` — Dog aggregate, DogId value object
- `src/CampFitFurDogs.Domain/Guardians/` — Guardian aggregate, GuardianId
- `src/CampFitFurDogs.Application/DependencyInjection.cs`
- `src/CampFitFurDogs.Infrastructure/DependencyInjection.cs`
- Domain and SharedKernel unit tests

## [Sprint 0] — 2026-03-27

### Completed Stories

| Story | Title | Issue |
|-------|-------|-------|
| US-013 | Add CONTRIBUTING, PR Template, .gitignore | #6 |
| US-014 | .NET Solution Skeleton | #7 |
| US-015 | CI Baseline Build and Test | #8 |

### Added
- Repository initialization
- ADR-0001: Use Lightweight ADRs
- ADR-0002: DDD Layered Architecture
- CONTRIBUTING.md and `.github/PULL_REQUEST_TEMPLATE.md`
- `.gitignore` for .NET projects
- `CampFitFurDogs.slnx` solution file
- `src/CampFitFurDogs.Api/` project with minimal API
- Test project scaffolds (Api, Application, Domain, Infrastructure, SharedKernel)
- `.github/workflows/ci.yaml` — build and test pipeline
- `global.json` pinning .NET SDK version
- Product vision, capability themes, emotional guarantees, definition of ready
- 44 product stories across infra, docs, and customer domains


