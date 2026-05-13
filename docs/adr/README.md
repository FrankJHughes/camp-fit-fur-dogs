# Architecture Decision Records (ADRs)

This directory contains all Architecture Decision Records (ADRs) for the Camp Fit Fur Dogs system.  
ADRs document **significant, irreversible, or high‑impact decisions** in the architecture, tooling, workflows, and documentation structure.

ADRs are **permanent historical records** — they are never deleted, only superseded.

---

## Template

New ADRs should follow the simplified template:

- [`TEMPLATE.md`](TEMPLATE.md)

This template matches the structure used in ADRs 0001–0022 and all new ADRs going forward.

---

## How ADRs Work

- Each ADR is numbered sequentially (`0001`, `0002`, …).  
- Once accepted, an ADR becomes part of the system’s architectural history.  
- New ADRs may **supersede** older ones but never remove them.  
- ADRs should be written when:
  - a major architectural choice is made  
  - a workflow or convention changes  
  - a subsystem is introduced or retired  
  - documentation structure changes  
  - CI/CD architecture changes  

ADRs are **not** for:
- minor refactors  
- small code decisions  
- temporary experiments  
- sprint‑level planning  

---

## Index

> **This index is intentionally incomplete.**  
> It will be regenerated after ADRs 0023–0030 are added.

| ADR | Title | Status |
|-----|--------|--------|
| [0001](0001-use-lightweight-adrs.md) | Use Lightweight ADRs | Accepted |
| [0002](0002-ddd-layered-architecture.md) | DDD Layered Architecture | Accepted |
| [0003](0003-developer-experience-toolchain.md) | Developer Experience Toolchain | Accepted |
| [0004](0004-docker-compose-for-infrastructure.md) | Docker Compose for Infrastructure | Accepted |
| [0005](0005-makefile-for-standardized-commands.md) | Makefile for Standardized Commands | Accepted |
| [0006](0006-dev-container-for-development.md) | Dev Container for Development | Accepted |
| [0007](0007-one-command-local-bootstrap.md) | One‑Command Local Bootstrap | Accepted |
| [0008](0008-consistent-editor-experience.md) | Consistent Editor Experience | Accepted |
| [0009](0009-story-naming-convention.md) | Story Naming Convention | Accepted |
| [0010](0010-retire-planning-yaml-infrastructure.md) | Retire Planning YAML Infrastructure | Accepted |
| [0011](0011-cqrs-command-query-pipelines.md) | CQRS Command/Query Pipelines | Accepted |
| [0012](0012-frontend-technology-react-nextjs.md) | Frontend Technology (React + Next.js) | Accepted |
| [0013](0013-server-side-identity-resolution.md) | Server‑Side Identity Resolution | Accepted |
| [0014](0014-dispatcher-pipeline.md) | Application Dispatcher Pipeline | Accepted |
| [0015](0015-convention-based-auto-registration.md) | Convention‑Based Auto‑Registration | Accepted |
| [0016](0016-domain-events-architecture.md) | Domain Events Architecture | Accepted |
| [0017](0017-unit-of-work-pattern.md) | Unit of Work Pattern | Accepted |
| [0018](0018-central-package-management.md) | Central Package Management | Accepted |
| [0019](0019-test-suite-organization.md) | Test Suite Organization | Accepted |
| [0020](0020-endpoint-auto-discovery.md) | Endpoint Auto‑Discovery | Accepted |
| [0021](0021-query-side-reader-isolation.md) | Query‑Side Reader Isolation | Accepted |
| [0022](0022-pr-preview-pipeline.md) | PR Preview Pipeline | Accepted |

> ADRs 0023–0030 will be added after generation.

---

## Notes

- ADRs reflect **what the system decided**, not necessarily what the system does today.  
- Superseded ADRs remain for historical context.  
- ADRs are the **source of truth** for architectural intent.

