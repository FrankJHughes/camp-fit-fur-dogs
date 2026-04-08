# ADR-0007: One-Command Local Bootstrap

**Status:** Accepted
**Date:** 2026-04-07

## Context

Setting up a local development environment requires multiple manual
steps: installing prerequisites, starting infrastructure, restoring
packages, building, and running tests. Each step can fail silently,
leaving developers unsure whether their environment is healthy. The
Diamond Model (ADR-003) defines **L5 – One-Command Onboarding** as
the capstone layer that orchestrates all lower layers into a single
entry point.

## Decision

Provide **two platform-native bootstrap scripts** at the repository
root:

| Script | Platform | Build pipeline |
|--------|----------|---------------|
| `bootstrap.sh` | Linux / macOS / WSL / dev container | `make all` (L4) |
| `bootstrap.ps1` | Windows (PowerShell) | Raw `dotnet` CLI commands |

### Four-Phase Pipeline

Both scripts follow an identical four-phase structure:

1. **Validate** — Check for required tools (`docker`, `dotnet`, `make`
   on bash) and confirm the Docker daemon is running. Exit immediately
   with actionable install instructions if anything is missing.
2. **Infrastructure** — `docker compose up -d --wait` starts services
   from the shared `compose.yml` (L1) and blocks until all health
   checks pass.
3. **Build & Test** — Run the full restore/build/test pipeline.
   `bootstrap.sh` calls `make all` (L4); `bootstrap.ps1` calls the
   equivalent `dotnet` commands directly.
4. **Report** — Print a structured readiness summary: service
   endpoints with ports, pipeline pass/fail status, and elapsed time.

### Design Choices

- **Two scripts, not one:** Windows has no native `make`, so
  `bootstrap.ps1` calls `dotnet` commands directly rather than
  requiring developers to install `make` before they can bootstrap.
  `bootstrap.sh` uses `make` because it is ubiquitous on Linux/macOS.
- **`docker compose up -d --wait`:** The `--wait` flag blocks until
  health checks pass, eliminating race conditions between
  infrastructure startup and the build/test phase.
- **Fail-fast:** Both scripts exit on the first error with a clear
  message. No partial state is left behind.
- **Idempotent:** Every underlying command is idempotent — `compose up`
  skips running containers, `dotnet restore` skips cached packages,
  builds are incremental.

## Alternatives Considered

| Option | Pros | Cons |
|--------|------|------|
| **Platform-native scripts** | Zero extra dependencies; idiomatic per OS | Two files to maintain |
| **Single cross-platform script (Python)** | One file | Extra dependency; not idiomatic |
| **Makefile `bootstrap` target only** | Reuses L4 | Doesn't work on Windows without `make` |
| **README with manual steps** | No tooling | Error-prone; the problem we are solving |

## Consequences

- New developers run one command after cloning:
  `./bootstrap.sh` (bash) or `.\bootstrap.ps1` (PowerShell).
- Prerequisites are validated before any work begins — no silent
  failures mid-pipeline.
- The readiness report provides immediate, visible confirmation that
  the environment is healthy.
- Future prerequisites (e.g., Node.js for a frontend) are added to
  Phase 1 of both scripts.