# Documentation

Single entry point for all Camp Fit Fur Dogs documentation.

---

## Guides

### Role Guides

| Guide | Audience | Purpose |
|-------|----------|---------|
| [Contributing](../CONTRIBUTING.md) | All | Branch naming, commit messages, PR workflow |
| [Developer Guide](guides/developer-guide.md) | Developers | Onboarding, dev loop, quick links |
| [Product Owner Guide](guides/product-owner-guide.md) | Product Owner | Backlog, stories, DoR, milestones |
| [Scrum Master Guide](guides/scrum-master-guide.md) | Scrum Master | Ceremonies, board, sprint workflow |

### Architecture & Technical Guides

| Guide | Purpose |
|-------|---------|
| [Folder Structure](guides/developer/folder-structure.md) | Solution layout and vertical slice organization |
| [Abstractions Contract](guides/developer/abstractions-contract.md) | Commands, queries, and the public Application API |
| [Dispatcher Pipeline](guides/developer/dispatcher-pipeline.md) | Command and query flow through the system |
| [Domain Events](guides/developer/domain-events.md) | Raising, collecting, and dispatching domain events |
| [API Endpoint Purity](guides/developer/api-endpoint-purity.md) | Keeping endpoints thin and HTTP-only |
| [Shared Kernel](guides/developer/shared-kernel.md) | What belongs in SharedKernel and why |
| [Purity Rules](guides/developer/purity-rules.md) | Cross-layer architectural purity constraints |
| [Test Architecture](guides/developer/test-architecture.md) | Guardrails, test patterns, layer-specific testing |
| [DI Conventions](guides/developer/di-conventions.md) | Dependency injection registration rules |
| [Frontend Testing](guides/developer/frontend-testing.md) | Patterns and conventions for frontend tests |
| [Feature Slice Walkthrough](guides/developer/feature-slice-walkthrough.md) | Step-by-step TDD walkthrough for adding command and query slices |

---

## Architecture Decisions

| Document | Purpose |
|----------|---------|
| [ADR Index](adr/README.md) | All architecture decision records |

---

## Product

| Document | Purpose |
|----------|---------|
| [Product Vision](../product/VISION.md) | Vision, customer profile, scope |
| [Product Stories](../product/stories/) | Feature specs, acceptance criteria, emotional safety guarantees |

---

## Governance

| Document | Purpose |
|----------|---------|
| [Governance](governance/governance.md) | Roles, ceremonies, ADR process, security, CI |

---

## Operations

| Document | Purpose |
|----------|---------|
| [Runbooks](runbooks/) | Local setup, migrations, deployments, incident response |
| [Sprint Reviews](sprint-reviews/) | Sprint-by-sprint progress and retrospectives |
| [Changelog](../CHANGELOG.md) | All notable changes by sprint |

---

## Roadmap

See the [Product Owner Guide — Milestones](guides/product-owner-guide.md#milestones) for the authoritative milestone table and progress tracking.

[View milestone progress on GitHub](https://github.com/frankjhughes/camp-fit-fur-dogs/milestones)