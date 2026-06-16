# Architecture Decision Records (ADRs)

This directory contains all Architecture Decision Records (ADRs) for the Camp Fit Fur Dogs system.  
ADRs document **significant, irreversible, or high‑impact decisions** in the architecture, tooling, workflows, and documentation structure.

ADRs are **permanent historical records** — they are never deleted, only superseded.

---

## Template

New ADRs should follow the simplified template:

- [`ADR_TEMPLATE.md`](ADR_TEMPLATE.md)

This template matches the structure used in ADRs 0001–0032 and all new ADRs going forward.

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

# Index

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
| [0023](0023-story-grammar-as-canonical-story-format.md) | Story Grammar as Canonical Story Format | Accepted |
| [0024](0024-retire-planning-automation.md) | Retire Planning Automation | Accepted |
| [0025](0025-github-native-sprint-workflow.md) | GitHub‑Native Sprint Workflow | Accepted |
| [0026](0026-three-rulebooks-documentation-architecture.md) | Three Rulebooks Documentation Architecture | Accepted |
| [0027](0027-runbooks-as-operational-procedures.md) | Runbooks as Operational Procedures | Accepted |
| [0028](0028-sprint-reviews-as-historical-artifacts.md) | Sprint Reviews as Historical Artifacts | Accepted |
| [0029](0029-selective-ci-testing-architecture.md) | Selective CI Testing Architecture | Accepted |
| [0030](0030-preview-environment-architecture-v2.md) | Preview Environment Architecture v2 | Accepted |
| [0031](0031-form-architecture-rhf-zod.md) | Form Architecture (RHF + Zod) | Accepted |
| [0032](0032-create-account-contract.md) | Create Account Contract | Accepted |
| [0033](0033-domain-invariant-hardening-roadmap.md) | Domain Invariant Hardening Roadmap | Accepted |
| [0034](0034-email-value-object-hardening.md) | Email Value Object Hardening | Accepted |
| [0035](0035-phone-number-value-object-hardening.md) | Phone Number Value Object Hardening | Accepted |
| [0036](0036-name-value-object-hardening.md) | Name Value Object Hardening | Accepted |
| [0037](0037-password-strength-strategy.md) | Password Strength Strategy | Accepted |
| [0038](0038-cors-policy-architecture.md) | CORS Policy Architecture | Accepted |
| [0040](0040-middleware-ordering-for-security-and-cors.md) | Middleware Ordering for Security and CORS | Accepted |
| [0041](0041-authentication-callback-pipeline-architecture.md) | Authentication Callback Pipeline Architecture | **Superseded by ADR‑0054** |
| [0042](0042-environment-aware-configuration-architecture.md) | Environment‑Aware Configuration Architecture | Accepted |
| [0043](0043-hosting-provider-abstraction-model.md) | Hosting Provider Abstraction Model | Accepted |
| [0044](0044-security-header-architecture.md) | Security Header Architecture | Accepted |
| [0045](0045-session-store-scaling-strategy.md) | Session Store Scaling Strategy | Accepted |
| [0046](0046-form-command-state-machine-architecture.md) | Form Command State Machine Architecture | Accepted |
| [0047](0047-api-error-boundary-architecture.md) | API Error Boundary Architecture | Accepted |
| [0048](0048-reader-repository-separation-model.md) | Reader/Repository Separation Model | Accepted |
| [0049](0049-pr-preview-url-resolution-architecture.md) | PR Preview URL Resolution Architecture | Accepted |
| [0050](0050-api-request-response-dto-architecture.md) | API Request/Response DTO Architecture | Accepted |
| [0051](0051-domain-event-delivery-guarantees.md) | Domain Event Delivery Guarantees | Accepted |
| [0052](0052-vertical-slice-isolation-model.md) | Vertical Slice Isolation Model | Accepted |
| [0053](0053-immutable-context-builder.md) | Immutable Context Builder | Accepted |
| [0054](0054-deprecation-of-step-engine.md) | Deprecation of Step Engine | Accepted |
| [0055](0055-authentication-callback-architecture.md) | Authentication Callback Architecture (Builder‑Based) | Accepted |
| [0056](0056-session-token-architecture.md) | Session Token Architecture | Accepted |
| [0057](0057-identity-mapping-architecture.md) | Identity Mapping Architecture | Accepted |
| [0058](0058-session-cookie-architecture.md) | Session Cookie Architecture | Accepted |

---

## Notes

- ADRs reflect **what the system decided**, not necessarily what the system does today.  
- Superseded ADRs remain for historical context.  
- ADRs are the **source of truth** for architectural intent.  
- Conventions and guides must remain aligned with ADRs to avoid drift.
