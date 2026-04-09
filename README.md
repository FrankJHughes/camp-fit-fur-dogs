# :paw_prints: Camp Fit Fur Dogs
### A Story-First, Emotionally-Safe Product Built with Modern Engineering Discipline

<p align="center">

  <!-- Project Identity -->
  <a href="https://github.com/frankjhughes/camp-fit-fur-dogs">
    <img src="https://img.shields.io/badge/Camp%20Fit%20Fur%20Dogs-Portfolio%20Project-blueviolet?style=flat-square" alt="Project Identity">
  </a>

  <!-- Repo Size -->
  <a href="https://github.com/frankjhughes/camp-fit-fur-dogs">
    <img src="https://img.shields.io/github/repo-size/frankjhughes/camp-fit-fur-dogs?style=flat-square&label=Repo%20Size" alt="Repository Size">
  </a>

  <!-- Issues -->
  <a href="https://github.com/frankjhughes/camp-fit-fur-dogs/issues">
    <img src="https://img.shields.io/github/issues/frankjhughes/camp-fit-fur-dogs?style=flat-square&label=Open%20Issues" alt="Issues">
  </a>

  <!-- PRs -->
  <a href="https://github.com/frankjhughes/camp-fit-fur-dogs/pulls">
    <img src="https://img.shields.io/github/issues-pr/frankjhughes/camp-fit-fur-dogs?style=flat-square&label=Open%20PRs" alt="Pull Requests">
  </a>

  <!-- Last Commit -->
  <a href="https://github.com/frankjhughes/camp-fit-fur-dogs/commits/main">
    <img src="https://img.shields.io/github/last-commit/frankjhughes/camp-fit-fur-dogs?style=flat-square&color=brightgreen&label=Last%20Commit" alt="Last Commit">
  </a>

  <!-- License -->
  <a href="LICENSE">
    <img src="https://img.shields.io/badge/License-Unlicensed-lightgrey?style=flat-square" alt="License">
  </a>

</p>

---

## Overview

Camp Fit Fur Dogs is a **portfolio-grade software project** demonstrating:

- Product ownership and story-first design
- Architecture clarity (DDD layered architecture)
- Agile process leadership
- Emotional-safety-driven design
- Disciplined engineering practices
- Developer-friendly governance
- Reproducible developer onboarding

The repo itself is part of the product:
a living demonstration of how to build, document, and govern a modern software system with intention.

---

## Repository Structure

```
camp-fit-fur-dogs/
|
+-- src/                        # Application code (DDD layers)
|   +-- CampFitFurDogs.Api/
|   +-- CampFitFurDogs.Application/
|   +-- CampFitFurDogs.Domain/
|   +-- CampFitFurDogs.Infrastructure/
|   +-- CampFitFurDogs.SharedKernel/
|
+-- tests/                      # Mirror of src/ for unit tests
|
+-- docs/                       # Documentation hub
|   +-- adr/                    # Architecture Decision Records
|   +-- governance/             # Governance model
|   +-- runbooks/               # Operational runbooks
|   +-- sprint-reviews/         # Sprint review records
|
+-- product/                    # Product artifacts
|   +-- VISION.md
|   +-- briefs/
|   +-- capabilities/
|   +-- definition-of-ready/
|   +-- emotional-guarantees/
|   +-- stories/                # Product backlog (story files)
|       +-- customer/
|       +-- docs/
|       +-- infra/
|
+-- portfolio/                  # Employer-facing artifacts
|
+-- .devcontainer/              # Dev container configuration
+-- .github/                    # CI workflows, PR template
+-- .vscode/                    # Editor settings
+-- CHANGELOG.md
+-- CONTRIBUTING.md
+-- CODEOWNERS
+-- Makefile
+-- compose.yml
+-- bootstrap.ps1 / bootstrap.sh
+-- README.md                   # You are here
```

---

## Key Documentation

### Product
- **Product Vision** — [`product/VISION.md`](product/VISION.md)
- **Product Backlog (Stories)** — [`product/stories/`](product/stories/)
- **Emotional Guarantees** — [`product/emotional-guarantees/`](product/emotional-guarantees/)
- **Definition of Ready** — [`product/definition-of-ready/`](product/definition-of-ready/)

### Architecture & Governance
- **ADRs** — [`docs/adr/`](docs/adr/) (10 accepted decisions)
- **Governance Model** — [`docs/governance/governance.md`](docs/governance/governance.md)
- **Sprint Reviews** — [`docs/sprint-reviews/`](docs/sprint-reviews/)

### Developer Experience
- **Contributing Guide** — [`CONTRIBUTING.md`](CONTRIBUTING.md)
- **Runbooks** — [`docs/runbooks/`](docs/runbooks/)
- **Documentation Hub** — [`docs/README.md`](docs/README.md)

### Portfolio (Employer-Facing)
- **Stakeholder Definition** — [`portfolio/STAKEHOLDER.md`](portfolio/STAKEHOLDER.md)
- **Use Cases** — [`portfolio/USE-CASES.md`](portfolio/USE-CASES.md)
- **Surfacing Strategy** — [`portfolio/SURFACING-STRATEGY.md`](portfolio/SURFACING-STRATEGY.md)

---

## Engineering Practices

This repository demonstrates:

- **Story-first product design** with emotional safety guarantees
- **DDD layered architecture** with SharedKernel, Domain, Application, Infrastructure, API
- **Deterministic CI/CD** with GitHub Actions
- **Explicit merge governance** with branch protection and CODEOWNERS
- **Reproducible developer onboarding** via devcontainer and bootstrap scripts
- **Architecture Decision Records** for traceable technical choices
- **Strict markdown fencing discipline** in all technical artifacts

---

## Sprint Board

[GitHub Projects Sprint Board](https://github.com/frankjhughes/camp-fit-fur-dogs/projects/14)

---

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for:

- Branch naming and commit conventions
- 2-step story workflow (story file + GitHub Issue)
- PR review and merge process
- Story naming convention (ADR-0009)

---

## License

This project is part of a personal portfolio and is not licensed for commercial use.

---

## Current Status

**Engineering foundation is complete.** DDD architecture, CI pipeline,
developer environment, and governance shipped across Sprints 0-2.
Customer features are unblocked.

### Milestone Progress

| Milestone | Goal | Stories | Status |
|-----------|------|---------|--------|
| [M1: First Customer Vertical](https://github.com/frankjhughes/camp-fit-fur-dogs/milestones) | Create account, register dog, view profile | 3 | Not started |
| [M2: Complete Dog Management](https://github.com/frankjhughes/camp-fit-fur-dogs/milestones) | Full dog CRUD with graceful edge cases | 7 | Not started |
| [M3: Portfolio Showcase](https://github.com/frankjhughes/camp-fit-fur-dogs/milestones) | Docs, tooling, onboarding ready for review | 4 | Not started |

**22 stories remaining** across 3 milestones | **17 completed** | **10 ADRs** | Next story: US-045

[Milestone Tracker](https://github.com/frankjhughes/camp-fit-fur-dogs/milestones) ·
[Sprint History](CHANGELOG.md) ·
[Sprint Board](https://github.com/frankjhughes/camp-fit-fur-dogs/projects/14)