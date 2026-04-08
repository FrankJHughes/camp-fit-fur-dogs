# ADR-0005: Makefile for Standardized Developer Commands

**Status:** Accepted
**Date:** 2026-04-07

## Context

Developers and CI currently invoke raw `dotnet` CLI commands with
framework-specific flags (`--no-restore`, `--configuration Release`).
This creates duplication between local workflows and CI, increases
onboarding friction, and makes flag changes error-prone.

The Diamond Model (ADR-003) defines **L4 – Developer Workflow** as the
layer that standardises commands across environments.

## Decision

Use **GNU Make** (`Makefile` at repository root) as the project task
runner.

### Targets

| Target | Description |
|--------|-------------|
| `help` | Lists all targets with descriptions (default) |
| `restore` | `dotnet restore` |
| `build` | `dotnet build` (depends on `restore`) |
| `test` | `dotnet test` (depends on `build`) |
| `clean` | `dotnet clean` + remove `bin`/`obj` directories |
| `infra-up` | `docker compose up -d` |
| `infra-down` | `docker compose down` |
| `all` | Full pipeline: `restore` → `build` → `test` |

## Alternatives Considered

| Option | Pros | Cons |
|--------|------|------|
| **GNU Make** | Zero install on Linux/CI/dev containers; self-documenting; industry standard | Requires install on bare Windows (mitigated by WSL2 / dev containers) |
| **Just** | Modern syntax, cross-platform | Extra install step in CI; less ubiquitous |
| **Nuke / Cake** | .NET-native, type-safe | Heavy dependencies; tightly coupled to .NET ecosystem |
| **PowerShell scripts** | Native on Windows | Not standard on Linux CI; no built-in dependency graph |

## Consequences

- CI workflow (`ci.yaml`) replaces three raw `dotnet` steps with
  `make all`.
- New developers run `make help` to discover available commands.
- The dev container (Issue #58) will include `make` in its image,
  ensuring identical behaviour across all environments.
- Future targets (e.g., `migrate`, `lint`, `format`) follow the same
  pattern.