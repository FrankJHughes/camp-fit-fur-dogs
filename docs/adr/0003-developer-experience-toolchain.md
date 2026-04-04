# ADR-003: Developer Experience Toolchain

| Field     | Value        |
|-----------|--------------|
| Status    | Accepted     |
| Date      | 2026-04-03   |
| Deciders  | Frank Hughes |

## Context
The Camp Fit Fur Dogs backlog contains five implementation-agnostic developer
experience stories covering containerized environments, local bootstrap,
infrastructure dependencies, standardized commands, and editor configuration.

These stories interact: the local bootstrap must start the same infrastructure
services that CI uses, and the containerized environment must call the same
build commands the developer uses locally. Choosing tools for each layer in
isolation risks incompatibility.

This ADR records the selected architecture, the dependency relationships,
and the rationale so that implementation proceeds from a shared plan.

## Decision

### Architecture: Diamond Model

The five layers form a diamond dependency graph, not a linear stack.
Two foundation layers (L1 and L4) are consumed by two peer environment
layers (L2 and L3). A fifth layer (L5) is independent.

```
           ┌─────────────────────┐
           │  L5  Editor Config  │  (independent, any time)
           └─────────────────────┘

  ┌──────────────────┐    ┌───────────────────┐
  │  L3  Local       │    │  L2  Containerized │
  │  Bootstrap       │    │  Dev Environment   │
  │  (PRIMARY)       │    │  (ALTERNATIVE)     │
  └────┬────────┬────┘    └────┬──────────┬────┘
       │        │              │          │
       ▼        ▼              ▼          ▼
  ┌─────────┐  ┌──────────────────────────────┐
  │  L1     │  │  L4  Task Runner             │
  │  Infra  │  │  (command interface)          │
  └─────────┘  └──────────────────────────────┘
       ▲                       ▲
       │                       │
       └────── CI workflows ───┘
```

### Layer Responsibilities

**Layer 1 — Infrastructure Definitions (foundation)**

Owns all infrastructure service definitions (database, cache, message
broker). Provides a declarative, version-controlled specification that
every consumer — L2, L3, and CI — uses identically. Does not own
application source, build logic, or editor settings.

Inputs: none (foundation).
Outputs: running, network-accessible services; a declarative definition
consumed by L2, L3, and CI.

**Layer 4 — Task Runner (foundation)**

Owns the canonical command interface: build, test, run, clean, and any
future standard targets. Single source of truth for how to build and test
the application. CI, L2, and L3 all call these commands instead of raw
framework CLI invocations. Does not own infrastructure lifecycle, environment
provisioning, or editor settings.

Inputs: application source code; assumes L1 services are available.
Outputs: standardized commands consumed by L2, L3, CI, and developers.

**Layer 3 — Local Bootstrap (primary onboarding path)**

Owns the first-run local developer experience. Validates prerequisites
(Docker running, correct SDK version, ports available), starts L1
infrastructure services via container orchestration, calls L4 commands
for restore/build/test, and prints a readiness report. Docker is a stated
prerequisite. Application code runs on the host with a natively installed
SDK; only infrastructure runs in containers.

Inputs: L1 definitions (infrastructure), L4 commands (build/test).
Outputs: a working local environment with services up, project built,
tests green.

**Layer 2 — Containerized Dev Environment (alternative onboarding path)**

Owns the dev container configuration: SDK version, CLI tools, editor
extensions, port forwarding, and lifecycle hooks. References L1 for
infrastructure services and L4 for post-create build/test. Both the
application code and infrastructure run inside containers. Cloud
environment compatibility (e.g., Codespaces) is a side effect of
shipping a dev container configuration, not an explicit goal.

Inputs: L1 definitions (infrastructure), L4 commands (post-create hook).
Outputs: a ready-to-code containerized workspace.

**Layer 5 — Editor Config (independent)**

Owns formatting rules, debug launch configurations, extension
recommendations, and workspace settings. Fully independent of layers
1–4. Can ship at any time.

Inputs: none from other layers.
Outputs: consistent formatting and debugging across all contributors.

### Story Mapping

| Layer | Enables Story                              | Points | Priority |
|-------|--------------------------------------------|--------|----------|
| 1     | Declarative Infrastructure Dependencies    | 3      | P1       |
| 4     | Standardized Developer Commands            | 2      | P1       |
| 3     | One-Command Local Bootstrap                | 3      | P0       |
| 2     | Containerized Development Environment      | 3      | P1       |
| 5     | Consistent Editor Experience               | 2      | P2       |

Total: 13 points (revised from 17).

### Implementation Order

```
L1 → L4 → L3 → L2
                        L5 (parallel, any time)
```

Foundations first (L1, L4), then the primary onboarding path (L3), then
the alternative (L2). L5 has no dependency and can ship in parallel.

This order resolves a cross-dependency in the original linear stack: L2
and L3 both consume L1 (down) and L4 (across). Building L4 before L2
and L3 ensures both environment layers can be fully verified on first
implementation.

## Alternatives Considered

### Dependency Model

**Linear stack (1→2→3→4→5) — rejected.** The original ADR proposed a
strict bottom-up stack where each layer depended only on layers below
it. Analysis revealed L2 and L3 both reach up to L4, creating a
bootstrapping problem: L2 and L3 cannot be fully verified until L4
exists, but the linear order builds L4 after both.

**Diamond with dual foundations (chosen).** L1 and L4 are peer
foundations. L2 and L3 are peer consumers. This honestly represents the
actual dependencies and produces a clean implementation order.

### L2 Scope

**Codespaces-first (5pts, P0) — rejected.** The original stories
positioned cloud-hosted development as the default path and the highest
priority. For a solo portfolio project, no contributor needs cloud-hosted
access. The primary audience (employers reviewing the repo) reads code,
not launches Codespaces.

**Dev container as alternative (3pts, P1) — chosen.** The dev container
configuration is valuable as a reproducible environment declaration. It
works locally with Docker and happens to be Codespaces-compatible as a
side effect. The local bootstrap (L3) is the primary onboarding path.

### L3 Infrastructure Strategy

**Native local installation — rejected.** Provisioning infrastructure
natively on the host (e.g., installing Postgres directly) creates a
second infrastructure path that diverges from what L2 and CI use. This
is exactly the "works on my machine" problem the DX initiative exists
to eliminate.

**Containers via L1 — chosen.** L3 calls the same L1 infrastructure
definitions that L2 and CI use. Docker is the single stated prerequisite.
One infrastructure specification, zero divergence.

### Tool Selection

Specific tool choices (Docker Compose vs. Podman, Make vs. Just, etc.)
are deferred to implementation. This ADR governs layer architecture,
dependencies, and implementation order — not tool selection.

## Consequences

- Docker is a required prerequisite for all development paths
- CI workflows must call L4 task runner commands, not raw framework CLI
- Any new infrastructure dependency must be added to L1 first
- L2 and L3 are peer alternatives — neither is deprecated or secondary
  in capability, but L3 is the recommended starting point
- This ADR must be updated if the team changes any layer's architecture
- Tool selection for each layer is a separate, deferred decision
