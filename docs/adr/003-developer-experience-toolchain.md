# ADR-003: Developer Experience Toolchain

## Status
Proposed

## Context
The Camp Fit Fur Dogs backlog contains five implementation-agnostic developer
experience stories covering cloud environments, local bootstrap, infrastructure
dependencies, standardized commands, and editor configuration. These stories
interact — the cloud environment must reuse the same infrastructure definitions
as local development, and CI must call the same commands developers use.

Choosing tools for each layer in isolation risks incompatibility. This ADR
records the selected toolchain, the dependency order, and the rationale so
that implementation proceeds from a shared plan.

## Decision

### Toolchain Layers (dependency order)

```
┌─────────────────────────────────────────────────┐
│ 5. Editor Config (.editorconfig, .vscode/)      │
├─────────────────────────────────────────────────┤
│ 4. Task Runner (Makefile / Just / build.ps1)    │
├─────────────────────────────────────────────────┤
│ 3. Local Setup Script (setup.ps1 / setup.sh)    │
├─────────────────────────────────────────────────┤
│ 2. Dev Container (.devcontainer/ + Codespaces)  │
├─────────────────────────────────────────────────┤
│ 1. Docker Compose (docker-compose.yml)          │  ← foundation
└─────────────────────────────────────────────────┘
```

**Layer 1 → Docker Compose** is the foundation. It defines all infrastructure
dependencies (database, cache, etc.) declaratively. Every layer above reuses
this definition rather than defining its own.

**Layer 2 → Dev Container** references the Docker Compose file for service
dependencies and adds the SDK, CLI tools, and editor extensions. It is the
primary zero-setup path via GitHub Codespaces.

**Layer 3 → Local Setup Script** validates prerequisites, starts Docker
Compose services if not running, then calls the task runner for restore,
build, and test. It is the local alternative to the Dev Container.

**Layer 4 → Task Runner** provides the standardized command interface
(build, test, run, clean, seed). Called by the setup script, the Dev
Container post-create hook, and CI workflows. This is the single source
of truth for how to build and test.

**Layer 5 → Editor Config** is independent of the other layers but ships
on the same branch. It ensures formatting, debugging, and extension
recommendations are consistent.

### Story Mapping

| Layer | Tool Category       | Enables Story                              |
|-------|--------------------|--------------------------------------------|
| 1     | Container orchestration | Declarative Infrastructure Dependencies |
| 2     | Dev environment container | Zero-Setup Cloud Development Environment |
| 3     | Bootstrap scripting | One-Command Local Bootstrap               |
| 4     | Task runner         | Standardized Developer Commands            |
| 5     | Editor configuration | Consistent Editor Experience              |

### Implementation Order
Layers are implemented bottom-up: 1 → 2 → 3 → 4 → 5. Each layer can be
verified independently, but higher layers depend on lower ones being in place.

Layer 5 (Editor Config) has no dependency on layers 1–4 and may be
implemented in parallel.

## Alternatives Considered

### Nix / Devenv
- Pros: Reproducible, declarative, covers SDK + tools + services in one config
- Cons: Steep learning curve, limited Windows support, team unfamiliarity
- Decision: Revisit if the team adopts Nix for other projects

### Vagrant
- Pros: Mature, full-VM isolation
- Cons: Heavy resource usage, slow startup, Docker-based alternatives are lighter
- Decision: Not justified for a .NET API project

### No Task Runner (raw dotnet CLI)
- Pros: Zero additional tooling
- Cons: Multi-step commands, CI/local divergence, no discoverability
- Decision: Rejected — the consistency and discoverability benefits outweigh
  the cost of one additional file

### Gitpod (instead of Dev Containers)
- Pros: Cloud-native, fast startup
- Cons: Vendor lock-in, less IDE flexibility, no local fallback
- Decision: Dev Containers are more portable and work with multiple editors

## Consequences
- Docker is a required dependency for all developers (local and container)
- CI workflows must call the task runner, not raw CLI commands
- Any new infrastructure dependency must be added to Docker Compose first
- ADR must be updated if the team changes any layer's tool choice
