# Developer Contributor Guide

Welcome to Camp Fit Fur Dogs. This guide covers everything you need to
clone the repo, run the app locally, and ship code through our pull
request workflow.

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | 9.0+ | Build and run the API |
| Docker Desktop | Latest | Container runtime for local services |
| PowerShell | 7+ | Developer experience scripts |
| Git | 2.x | Source control |
| GitHub CLI (`gh`) | 2.x | Issue and PR management |

## Getting Started

```powershell
# Clone the repo
git clone https://github.com/frankjhughes/camp-fit-fur-dogs.git
cd camp-fit-fur-dogs

# Bootstrap the local environment
./dx.ps1 bootstrap

# Start the app
./dx.ps1 up
```

The `dx.ps1` script is the single entry point for all developer
commands. Run `./dx.ps1 help` to see available commands.

## Project Structure

```
camp-fit-fur-dogs/
├── src/
│   ├── CampFitFurDogs.Api/           # ASP.NET Core host, controllers, middleware
│   ├── CampFitFurDogs.Application/   # Use cases, command/query handlers
│   ├── CampFitFurDogs.Domain/        # Aggregates, entities, value objects, domain events
│   └── CampFitFurDogs.Infrastructure/# EF Core, repos, external service adapters
├── tests/
│   ├── CampFitFurDogs.Domain.Tests/
│   └── CampFitFurDogs.Api.Tests/
├── product/
│   ├── stories/                      # Backlog (the source of truth)
│   ├── definition-of-ready/
│   └── emotional-guarantees/
├── docs/
│   ├── adr/                          # Architecture Decision Records
│   ├── sprint-reviews/               # Sprint review documents
│   ├── governance/                   # Process governance
│   └── guides/                       # ← You are here
├── CONTRIBUTING.md                   # Role-routing hub
├── CODEOWNERS                        # PR review assignments
├── CHANGELOG.md                      # Release history
└── dx.ps1                            # Developer experience script
```

## Architecture

The codebase follows Domain-Driven Design with four layers. Dependencies
point inward — Domain has zero external references.

```
Api → Application → Domain
 └→ Infrastructure → Domain
```

- **Domain** — aggregates, value objects, domain events, repository
  interfaces. No framework dependencies.
- **Application** — command and query handlers that orchestrate domain
  logic. References Domain only.
- **Infrastructure** — EF Core DbContext, repository implementations,
  external adapters. References Domain for interface contracts.
- **Api** — ASP.NET Core host, controllers, middleware, DI composition
  root. References all layers.

### Key conventions

- One aggregate per file, named after the aggregate root.
- Value objects are `record` types in the aggregate's namespace.
- Repository interfaces live in Domain; implementations in
  Infrastructure.
- No reflection, no magic strings — explicit, compile-time-safe
  bindings.

## Development Workflow

### Branch naming

```
<type>/<short-description>
```

| Type | When |
|------|------|
| `feature/` | New user-facing capability |
| `fix/` | Bug fix |
| `docs/` | Documentation only |
| `infra/` | CI/CD, tooling, config |
| `refactor/` | Internal restructuring |

Examples: `feature/us-027-create-customer-account`,
`docs/sprint-3-stories`, `infra/ci-pipeline`.

### Commit messages

Use conventional-style commits:

```
<type>: <short summary>

<optional body explaining why, not what>
```

Types match branch types: `feature`, `fix`, `docs`, `infra`,
`refactor`.

### Development loop

1. Pull latest `main`.
2. Create a feature branch from `main`.
3. Make small, focused commits.
4. Push and open a PR against `main`.
5. Address CODEOWNERS review feedback.
6. Squash-merge after approval.

## Building and Running

```powershell
# Full build
./dx.ps1 build

# Run the API (with hot reload)
./dx.ps1 up

# Run all tests
./dx.ps1 test

# Tear down local containers
./dx.ps1 down

# Reset local database
./dx.ps1 db-reset
```

## Testing

- **Domain tests** — pure unit tests, no mocks, no infrastructure.
  Test aggregate behavior and value object invariants.
- **API tests** — integration tests using `WebApplicationFactory`.
  Test HTTP endpoints against a real test database.

Name test methods to describe the scenario:
`CreateAccount_WithDuplicateEmail_ReturnsConflict`.

Run tests before pushing:

```powershell
./dx.ps1 test
```

## Pull Request Process

### Opening a PR

1. Push your branch to origin.
2. Open a PR against `main` using `gh pr create`.
3. Title format: `<type>: <summary>` (matches commit convention).
4. Body should reference the story: `Closes #<issue-number>`.
5. CODEOWNERS will be auto-assigned as reviewers.

### PR checklist

Before requesting review, confirm:

- [ ] Code compiles with zero warnings.
- [ ] All tests pass locally (`./dx.ps1 test`).
- [ ] New code has tests covering the happy path and key edge cases.
- [ ] No unrelated changes bundled into the PR.
- [ ] Commit history is clean (squash fixups before review).

### Merge rules

- PRs require at least one CODEOWNERS approval.
- All CI checks must pass.
- Squash-merge is the default strategy.
- Delete the branch after merge.

## Architecture Decision Records (ADRs)

Significant technical decisions are recorded as ADRs in `docs/adr/`.

### When to write an ADR

- Choosing a framework, library, or tool.
- Changing project structure or conventions.
- Making a trade-off that future contributors will question.

### ADR format

```markdown
# ADR-NNNN: Title

## Status
Accepted | Superseded | Deprecated

## Context
What situation prompted this decision?

## Decision
What did we decide?

## Consequences
What trade-offs result from this decision?
```

### Existing ADRs

Browse the full list at [`docs/adr/`](../../docs/adr/).

## Code of Conduct

Be kind. Be constructive. Assume good intent. Every contributor —
regardless of experience level — deserves respect and clear feedback.

## Getting Help

- Open a GitHub Discussion for questions.
- Tag `@FrankJHughes` for architecture or process questions.
- Check the [Product Owner Guide](product-owner-guide.md) for story
  and backlog questions.
- Check the [Scrum Master Guide](scrum-master-guide.md) for sprint
  and board questions.
