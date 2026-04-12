# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

### Added

- `.github/copilot-instructions.md` — living conventions document (standing rules, PR conventions, architecture patterns, tooling, lessons learned)
- Sprint review closing checklist in `docs/sprint-reviews/_template.md`
- Conventions maintenance sections in all three contributor guides
- ADR-0011: CQRS Command/Query Pipelines (US-051)
- ADR-0011: CQRS Command/Query Pipelines (US-051)
- ADR-0012: Frontend Technology — React with Next.js (US-055)
- `.gitattributes` — enforces LF line endings repo-wide; eliminates CRLF phantom diffs in Dev Container
- First-Time Setup section in `docs/guides/developer-guide.md` (Docker Desktop auto-start, Git identity, hooks)

### Changed

- `.devcontainer/devcontainer.json` — adds `TESTCONTAINERS_RYUK_DISABLED` and `TESTCONTAINERS_HOST_OVERRIDE` for docker-outside-of-docker Testcontainers compatibility
- Root `.gitignore` — moves `node_modules/` and `.next/` to `src/frontend/.gitignore`; adds scratch file exclusions

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
- `src/CampFitFurDogs.SharedKernel/` — Entity, AggregateRoot, ValueObject,
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


- Next.js project scaffold with API proxy and health-check landing page (US-056)
