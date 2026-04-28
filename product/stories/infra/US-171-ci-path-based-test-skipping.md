---
id: US-171
title: "CI Path-Based Test Skipping"
epic: Infrastructure
milestone: M1
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-015
emotional_guarantees:
  - EG-01
  - EG-05
legal_guarantees: []
---

# US-171: CI Path-Based Test Skipping

## Intent

As a **developer**, I need the CI pipeline to skip test suites that are
irrelevant to my changes so that feedback is faster and compute is not wasted
on unaffected code.

## Value

The Camp Fit Fur Dogs repo is a monorepo with three independently testable
layers: backend (.NET), frontend (Next.js), and SharedKernel (Frank). A
frontend-only CSS fix should not trigger a 2-minute `dotnet test` run.
A backend domain model change should not trigger `npm test`. Path-based
filtering cuts CI time by 40–60% for single-layer changes while maintaining
full coverage for cross-cutting changes.

## Acceptance Criteria

### Path detection
- [ ] CI workflow detects which paths changed in the PR (compared to the base branch)
- [ ] Changed paths are classified into zones:
  - **backend**: `src/CampFitFurDogs.*/**`, `tests/CampFitFurDogs.*/**`
  - **frontend**: `frontend/**`
  - **shared-kernel**: `src/SharedKernel/**`, `tests/SharedKernel.*/**`
  - **infra**: `.github/**`, `Makefile`, `docker-compose*`, `*.sln`
  - **docs-only**: `*.md`, `product/**`, `docs/**`, `LICENSE`, `.editorconfig`
- [ ] Changes to **infra** zone trigger ALL test suites
- [ ] Changes to **docs-only** zone skip ALL test suites
- [ ] Changes to **shared-kernel** zone trigger backend + shared-kernel tests
- [ ] Changes to **backend** zone trigger backend + shared-kernel tests
- [ ] Changes to **frontend** zone trigger frontend tests only
- [ ] Multi-zone changes trigger the union of affected suites

### Workflow structure
- [ ] Each test suite runs as a separate job with a `needs` dependency on path detection
- [ ] Skipped jobs report as "skipped" (not "failed")
- [ ] Branch protection rules accept skipped jobs (job must exist, not be omitted)
- [ ] Path detection uses `dorny/paths-filter`
- [ ] Full pipeline can be manually triggered via `workflow_dispatch`
- [ ] CI dependency validator job ensures workflow matches `.github/ci/ci-deps.json`

### Governance hardening
- [ ] CI must never write or mutate repository files
- [ ] Catalog validation uses `Generate-Catalog.ps1 -Check`
- [ ] Frontmatter validation uses `Sync-Frontmatter.ps1 -CheckOnly`
- [ ] Governance checks fail if catalog or frontmatter drift is detected

### Safety guardrails
- [ ] Merges to `main` always run ALL test suites
- [ ] Skipped suite names appear in workflow summary
- [ ] Nightly scheduled run executes ALL suites

## Emotional Guarantees

- **EG-01 No Surprises** — Developers see which suites were skipped and why.
- **EG-05 Responsible Partner** — CI feedback arrives quickly for docs-only or single-layer changes.

## Notes

- Depends on US-015 (CI Baseline Build and Test)
- Backend → frontend dependency must be enforced (backend changes trigger frontend tests)
- CI dependency graph lives in `.github/ci/ci-deps.json`
- Governance scripts must support read-only check modes
- Branch protection requires jobs to exist even when skipped
- Consider caching for node_modules and NuGet packages
- Consider future test splitting for parallelism
- Demo scenarios:
  - Frontend-only change → backend skipped
  - SharedKernel change → backend + shared-kernel run
  - README edit → all suites skipped
