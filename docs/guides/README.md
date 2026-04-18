# Guides Index

This folder contains contributor guides for all roles involved in the Camp Fit Fur Dogs project: Developers, Product Owners, and Scrum Masters. Each guide provides role‑specific workflows, responsibilities, and conventions.

The `developer/` subfolder contains the full architecture handbook and technical contributor documentation.

---

## Developer Guides

These guides support engineers working inside the codebase.  
They cover onboarding, workflow, architecture, purity rules, and testing strategy.

### High‑Level Developer Guide
| Guide | Description |
|-------|-------------|
| [Developer Guide](developer-guide.md) | Onboarding, TDD, developer loop, Git hooks, and workflow |

### Architecture & Technical Guides (in `developer/`)
| Guide | Description |
|-------|-------------|
| [Folder Structure](developer/folder-structure.md) | How the solution and vertical slices are organized |
| [Abstractions Contract](developer/abstractions-contract.md) | Rules for commands, queries, and the public Application API |
| [Dispatcher Pipeline](developer/dispatcher-pipeline.md) | How commands and queries flow through the system |
| [Domain Events](developer/domain-events.md) | How domain events are raised, collected, and dispatched |
| [API Endpoint Purity](developer/api-endpoint-purity.md) | Rules for keeping endpoints thin and HTTP‑only |
| [Shared Kernel](developer/shared-kernel.md) | What belongs in SharedKernel and why |
| [Purity Rules](developer/purity-rules.md) | Cross‑layer architectural purity constraints |
| [Test Architecture](developer/test-architecture.md) | Guardrails, test patterns, and layer‑specific testing rules |
| [Frontend Testing Guide](frontend-testing.md) | Patterns and conventions for frontend tests |

---

## Product Owner Guides

These guides support product owners in planning, prioritization, and backlog management.

| Guide | Description |
|-------|-------------|
| [Product Owner Guide](product-owner-guide.md) | Responsibilities, backlog management, and workflow |

---

## Scrum Master Guides

These guides support scrum masters in facilitating ceremonies and maintaining team flow.

| Guide | Description |
|-------|-------------|
| [Scrum Master Guide](scrum-master-guide.md) | Ceremonies, facilitation, and team process |

---

## Purpose of This Folder

This folder provides:

- Role‑specific contributor documentation  
- Architecture and purity rules  
- Vertical slice conventions  
- Testing strategy and guardrails  
- Onboarding and workflow guidance  

Documentation should be updated whenever architecture, workflow, or team practices evolve.

---

## Related Documents

- ADRs in `docs/architecture/` — Formal architectural decisions and rationale
