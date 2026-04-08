# ADR-0004: Docker Compose for Infrastructure Definitions

| Field     | Value                  |
|-----------|------------------------|
| Status    | Accepted               |
| Date      | 2026-04-07             |
| Deciders  | Frank Hughes           |

## Context

ADR-003 established the Diamond Model for developer experience, designating Layer 1
(Infrastructure Definitions) as the foundation that all other layers consume. ADR-003
explicitly deferred tool selection — "Specific tool choices (Docker Compose vs. Podman,
Make vs. Just, etc.) are deferred to implementation" — to be resolved when each layer is
implemented.

Layer 1 requires a declarative, version-controlled specification for infrastructure
services (database, cache, message broker) that can be consumed identically by local
bootstrap (L3), containerized dev environments (L2), and CI pipelines.

## Decision

Use Docker Compose (Compose Specification) as the declarative infrastructure tool for
Layer 1. The canonical file is `compose.yml` at the repository root, with environment
variable interpolation from `.env`.

Docker Compose is selected because:

- It is the de facto standard for declarative container orchestration in development environments.
- Native support in GitHub Codespaces, VS Code Dev Containers, and all major CI platforms.
- The Compose Specification is an open standard, not vendor-locked.
- Docker is already a stated prerequisite for all development paths (ADR-003).

## Consequences

### Positive

- Single compose.yml consumed by L2, L3, and CI — one specification, zero divergence.
- Health checks, named volumes, and environment variable interpolation are first-class Compose features.
- No additional tooling beyond Docker (already required).
- Broad ecosystem support for extensions (profiles, overrides) as needs grow.

### Negative

- Developers must have Docker Desktop or Docker Engine installed (already a prerequisite per ADR-003).
- Podman users need podman-compose compatibility or a CLI alias.

### Neutral

- If the project later needs Kubernetes or cloud-native orchestration, Compose definitions can be translated via Kompose or similar tools.
