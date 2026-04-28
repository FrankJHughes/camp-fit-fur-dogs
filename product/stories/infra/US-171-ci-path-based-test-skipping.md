---
id: US-171
title: "CI Path-Based Test Skipping"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-015
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
- [ ] Changes to **infra** zone trigger ALL test suites (pipeline/tooling changes affect everything)
- [ ] Changes to **docs-only** zone skip ALL test suites (no code affected)
- [ ] Changes to **shared-kernel** zone trigger both backend and shared-kernel tests (Frank is a backend dependency)
- [ ] Changes to **backend** zone trigger backend and shared-kernel tests
- [ ] Changes to **frontend** zone trigger frontend tests only
- [ ] Changes spanning multiple zones trigger the union of affected suites

### Workflow structure
- [ ] Each test suite runs as a separate job with a `needs` dependency on path detection
- [ ] Skipped jobs report as "skipped" (not "failed") in the PR status checks
- [ ] GitHub branch protection rules accept skipped jobs as passing (use `if: always()` pattern or required-status workaround)
- [ ] The path detection step uses `dorny/paths-filter` or equivalent action
- [ ] Full pipeline (all suites) can be manually triggered via `workflow_dispatch` regardless of paths

### Safety guardrails
- [ ] Merges to `main` always run ALL test suites (no skipping on the default branch)
- [ ] Skipped suite names are logged in the workflow summary so developers know what was skipped and why
- [ ] A scheduled nightly run executes ALL suites as a safety net

## Emotional Guarantees

- **EG-01 No Surprises** — Developers see which suites were skipped and why
- **EG-05 Responsible Partner** — CI feedback arrives in under a minute for docs-only changes

## Notes

- Depends on US-015 (CI Baseline Build and Test) — the existing workflow that runs everything
- `dorny/paths-filter` is the most popular GitHub Action for this — well-maintained, used by thousands of repos
- Branch protection quirk: GitHub requires all listed status checks to pass OR be skipped. If a job is skipped via `if:`, it reports as skipped (passing). But if the job is omitted entirely, it is treated as missing (blocking). Use the `if:` condition pattern, not job omission.
- Consider: caching (`actions/cache`) for node_modules and NuGet packages to further speed up non-skipped jobs
- Consider: test splitting within suites for parallelism (future optimization)
- **Demo:** Push a frontend-only change — see backend tests skipped in under 10 seconds. Push a SharedKernel change — see both backend and SharedKernel tests run. Push a README edit — see all tests skipped.
