# Developer Contributor Guide

## User Story

As a developer joining the project, I want a single, scannable guide
that walks me from clone to first commit — covering environment setup,
build/test/debug workflows, code standards, and PR conventions — so
that I can contribute confidently without tribal knowledge or guesswork.

## Context

The Diamond Model (ADR-003) delivers five layers of developer
infrastructure, but no documentation ties them together into a
coherent onboarding narrative. A developer landing on the README today
sees no Getting Started section, no prerequisites list, and no mention
of the bootstrap scripts, dev container, or available `make` commands.

This story covers the **developer-facing** contributor experience.
Companion stories cover the Product Owner and Scrum Master roles
separately.

## Scope

This guide lives primarily in two places:

- **README.md § Getting Started** — the first thing a developer sees.
  Brief, opinionated, two onboarding paths.
- **docs/guides/developer-guide.md** — the deep reference.
  Everything a developer needs after the first build passes.

CONTRIBUTING.md becomes a hub page that routes each role to its
dedicated guide.

## Acceptance Criteria

### README — Getting Started

- [x] Prerequisites listed with version expectations and install links
      (Docker Desktop, .NET SDK 10, VS Code recommended).
- [x] Two onboarding paths presented side-by-side:
      - **Local bootstrap:** `./bootstrap.sh` (Linux/macOS/WSL) or
        `.\bootstrap.ps1` (Windows) — what it does, what to expect
        (readiness report).
      - **Dev container:** prerequisites (Docker + VS Code), "Reopen
        in Container," what to expect (auto build/test on open).
- [x] Available commands section referencing `make help` (bash) and
      listing the core commands (restore, build, test, clean,
      infra-up, infra-down).
- [x] Link to the full developer guide for deeper reference.

### Developer Guide — Environment

- [x] `.env.example` → `.env` setup explained with every variable
      documented (purpose, default value, when to override).
- [x] Infrastructure services listed with ports, credentials, and
      management UIs (PostgreSQL :5432, Redis :6379, RabbitMQ :5672,
      RabbitMQ Management :15672).
- [x] Explains how to start, stop, and reset infrastructure
      (`make infra-up`, `make infra-down`, volume pruning).
- [x] Troubleshooting section: Docker not running, port conflicts,
      WSL integration, health-check failures.

### Developer Guide — Build, Test, Debug

- [x] Build pipeline documented: `make all` (bash) or
      `dotnet restore` → `dotnet build` → `dotnet test` (PowerShell).
- [x] Testing conventions: project structure, naming patterns,
      how to run a single test or test project.
- [x] F5 debugging documented: what `launch.json` does, how
      `serverReadyAction` opens the browser, environment variables.
- [x] `dotnet watch` documented for hot-reload development.
- [x] VS Code tasks documented: Ctrl+Shift+B for build, test task,
      watch task.

### Developer Guide — Code Standards

- [x] Solution architecture overview: DDD layers (Domain, Application,
      Infrastructure, Api, SharedKernel), project dependency rules.
- [x] `.editorconfig` explained: what it enforces, why Makefile uses
      tabs, why markdown preserves trailing whitespace.
- [x] Recommended extensions explained: what each one does, why it
      is recommended.

### Developer Guide — PR Workflow

- [x] Branch naming convention documented with examples
      (`chore/`, `feat/`, `fix/`, `docs/`).
- [x] Commit message format documented (conventional commits style).
- [x] Merge checklist explained — what each item means and how to
      verify it.
- [x] CI pipeline explained: what `Build & Test` checks, how to
      read failures.
- [x] Review expectations: self-review before requesting, response
      time norms.

### CONTRIBUTING.md — Hub

- [x] CONTRIBUTING.md restructured as a role-routing hub:
      - Developer → `docs/guides/developer-guide.md`
      - Product Owner → `docs/guides/product-owner-guide.md`
      - Scrum Master → `docs/guides/scrum-master-guide.md`
- [x] Each link includes a one-sentence description of what the
      guide covers.

## Dependencies

- Diamond Model layers L1–L5 (Issues #55, #56, #57, #58, #59) must
  be merged so documentation reflects actual tooling.

## Open Questions

- Should the README Getting Started section include a terminal
  recording (asciicast) or screenshot of the readiness report?
- Should the developer guide include an architecture diagram
  (Mermaid in markdown)?