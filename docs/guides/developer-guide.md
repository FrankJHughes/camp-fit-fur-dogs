# Developer Contributor Guide

Welcome to Camp Fit Fur Dogs. This guide covers everything you need to clone the repo, run the app locally, and ship code through our pull request workflow.

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

# Restore dependencies
dotnet restore

# Start local infrastructure (PostgreSQL)
docker compose up -d

# Run the API
dotnet run --project src/CampFitFurDogs.Api
```

> **Note:** A unified developer experience script is planned but not yet
> implemented. See US-025 (DX Architecture Decision) for status. Until
> then, use the standard `dotnet` and `docker compose` commands shown in
> this guide.

## Project Structure

```
camp-fit-fur-dogs/
├── src/
│   ├── CampFitFurDogs.Api/             # ASP.NET Core host, controllers, middleware
│   ├── CampFitFurDogs.Application/     # Use cases, command/query handlers
│   ├── CampFitFurDogs.Domain/          # Aggregates, entities, value objects, domain events
│   └── CampFitFurDogs.Infrastructure/  # EF Core, repos, external service adapters
├── tests/
│   ├── CampFitFurDogs.Domain.Tests/
│   └── CampFitFurDogs.Api.Tests/
├── product/
│   ├── stories/                        # Backlog (the source of truth)
│   ├── definition-of-ready/
│   └── emotional-guarantees/
├── docs/
│   ├── adr/                            # Architecture Decision Records
│   ├── sprint-reviews/                 # Sprint review documents
│   ├── governance/                     # Process governance
│   └── guides/                         # ← You are here
├── CONTRIBUTING.md                     # Role-routing hub
├── CODEOWNERS                          # PR review assignments
└── CHANGELOG.md                        # Release history
```

## Architecture

The codebase follows Domain-Driven Design with four layers. Dependencies point inward — Domain has zero external references.

```
Api → Application → Domain
 └→ Infrastructure → Domain
```

- **Domain** — aggregates, value objects, domain events, repository interfaces. No framework dependencies.
- **Application** — command and query handlers that orchestrate domain logic. References Domain only.
- **Infrastructure** — EF Core DbContext, repository implementations, external adapters. References Domain for interface contracts.
- **Api** — ASP.NET Core host, controllers, middleware, DI composition root. References all layers.

### Key conventions

- One aggregate per file, named after the aggregate root.
- Value objects are `record` types in the aggregate's namespace.
- Repository interfaces live in Domain; implementations in Infrastructure.
- No reflection, no magic strings — explicit, compile-time-safe bindings.

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

Examples: `feature/us-027-create-customer-account`, `docs/sprint-3-stories`, `infra/ci-pipeline`.

### Commit messages

Use conventional-style commits:

```
<type>: <short summary>

<optional body explaining why, not what>
```

Types match branch types: `feature`, `fix`, `docs`, `infra`, `refactor`.

### Development loop

1. Pull latest `main`.
2. Create a feature branch from `main`.
3. Make small, focused commits.
4. Push and open a PR against `main`.
5. Address CODEOWNERS review feedback.
6. Squash-merge after approval.

### Source control safety

Direct pushes to `main` are blocked at two levels:

| Layer | What it stops | Setup |
|-------|---------------|-------|
| **GitHub branch rule** | Any push to `main` without a PR — including admins | Settings → Branches → `main` → "Require a pull request before merging" + "Do not allow bypassing" |
| **Local pre-push hook** | Push attempt before it leaves your machine (instant feedback) | One-time setup below |

#### Installing the pre-push hook

The repo ships a ready-made hook in `hooks/`. Install it once after cloning:

```powershell
Copy-Item hooks/pre-push .git/hooks/pre-push
```

After this, `git push origin main` will be rejected locally with a clear message before the push ever hits the network.

> **Note:** `.git/hooks/` is not tracked by Git, so every contributor must run the install step once. The source of truth is `hooks/pre-push` in the repo root.

## Building and Running

```powershell
# Full build
dotnet build

# Run the API (with hot reload)
dotnet watch --project src/CampFitFurDogs.Api

# Run all tests
dotnet test

# Start local infrastructure
docker compose up -d

# Tear down local containers
docker compose down

# Reset local database (destroy volume and recreate)
docker compose down -v && docker compose up -d
```

## Test-Driven Development (TDD)

All feature work follows Red-Green-Refactor. Every vertical slice begins with a failing test.

### The cycle

1. **Red** — Write a test that describes the behavior you want. Run it. Watch it fail. The failure message confirms you are testing the right thing.
2. **Green** — Write the minimum production code to make the test pass. No more.
3. **Refactor** — Clean up duplication, naming, and structure while all tests stay green.

### Slice order

Build each feature from the inside out, one layer at a time:

| Order | Layer | What to test | Example |
|-------|-------|--------------|---------|
| 1 | Domain | Value object invariants, aggregate factory rules | `DogName rejects empty string` |
| 2 | Domain | Aggregate behavior and state transitions | `Dog.Create sets all properties` |
| 3 | Application | Command handler orchestration (mock the repo) | `RegisterDogHandler persists dog` |
| 4 | Infrastructure | Repository round-trip against a real test DB | `DogRepository can save and retrieve` |
| 5 | API | Full HTTP request/response via WebApplicationFactory | `POST /customers/{id}/dogs returns 201` |

### Conventions

- Name tests to describe the scenario: `RegisterDog_WithMissingName_ReturnsBadRequest`.
- Domain tests are pure — no mocks, no infrastructure, no DI container.
- API tests use `WebApplicationFactory` with a real PostgreSQL test container.
- One test class per aggregate or endpoint. Group related scenarios with nested classes.

### When to skip TDD

TDD is required for all domain logic, command handlers, and API endpoints. Configuration-only changes (DI registration, EF mappings, middleware wiring) do not need dedicated tests but must be exercised by the integration tests above.

## Testing

- **Domain tests** — pure unit tests, no mocks, no infrastructure. Test aggregate behavior and value object invariants.
- **API tests** — integration tests using `WebApplicationFactory`. Test HTTP endpoints against a real test database.

Name test methods to describe the scenario: `CreateAccount_WithDuplicateEmail_ReturnsConflict`.

Run tests before pushing:

```powershell
dotnet test --verbosity normal
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
- [ ] All tests pass locally (`dotnet test`).
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

Be kind. Be constructive. Assume good intent. Every contributor — regardless of experience level — deserves respect and clear feedback.

## Getting Help

- Open a GitHub Discussion for questions.
- Tag `@FrankJHughes` for architecture or process questions.
- Check the [Product Owner Guide](product-owner-guide.md) for story and backlog questions.
- Check the [Scrum Master Guide](scrum-master-guide.md) for sprint and board questions.
