# ADR-0006: Dev Container for Containerized Development Environment

**Status:** Accepted
**Date:** 2026-04-07

## Context

Setting up a local development environment requires installing Docker
Desktop, WSL2 with Ubuntu, GNU Make, and the .NET SDK — a multi-step
process that varies by OS and is error-prone for new contributors. The
Diamond Model (ADR-003) defines **L2 – Containerized Environment** as
the layer that packages all tooling into a reproducible container.

## Decision

Use **VS Code Dev Containers** with a Docker Compose-based
configuration:

| Component | Purpose |
|-----------|---------|
| `.devcontainer/Dockerfile` | .NET SDK 10.0 image with `make` installed |
| `.devcontainer/docker-compose.yml` | Extends root `compose.yml` with a dev container service |
| `.devcontainer/devcontainer.json` | Orchestrates container, extensions, lifecycle hooks, port forwarding |

### Lifecycle

1. VS Code reads `devcontainer.json` and merges the two Compose files.
2. Infrastructure services (PostgreSQL, Redis, RabbitMQ) start first
   with health-check gates via `depends_on`.
3. The dev container starts after all services report healthy.
4. `postCreateCommand` runs `make all` (restore, build, test).
5. The developer lands at a terminal with a passing build.

### Key Design Choices

- **Compose-based over single container:** Reuses the root `compose.yml`
  (L1) so infrastructure definitions are never duplicated. The dev
  container Compose file only adds the development service itself.
- **`depends_on` with health checks:** The dev container waits for all
  infrastructure to report healthy before starting, eliminating race
  conditions during `postCreateCommand`.
- **`postCreateCommand: make all`:** Wires directly into the L4 task
  runner (ADR-0005), ensuring the same restore/build/test pipeline runs
  in the container as in CI and locally.
- **Forwarded ports with labels:** PostgreSQL (5432), Redis (6379),
  RabbitMQ (5672), and RabbitMQ Management (15672) are forwarded with
  descriptive labels for discoverability.

## Alternatives Considered

| Option | Pros | Cons |
|--------|------|------|
| **Dev Container (Compose-based)** | Reuses L1 definitions; zero duplication; health-gated startup | Requires Docker Desktop |
| **Dev Container (single image)** | Simpler config | Duplicates infrastructure definitions; no health gating |
| **GitHub Codespaces only** | Zero local install | Requires paid plan; latency; no offline support |
| **Manual setup guide** | No tooling overhead | Error-prone; OS-specific; the problem we are solving |

## Consequences

- New developers need only Docker Desktop and VS Code to start coding.
- The `postCreateCommand` guarantees a passing build on first open.
- Infrastructure definitions remain in a single source of truth
  (`compose.yml`), consumed by both L1 and L2.
- Future tooling additions (linters, formatters, debuggers) are added
  to the Dockerfile or `devcontainer.json` once and propagate to all
  developers automatically.