---
id: US-008
title: "Doc Audit And Defragmentation"
epic: ""
milestone: ""
status: shipped
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# Documentation Audit and Defragmentation

**Status:** Complete (implemented in this PR)

## User Story

As a contributor joining the project, I want documentation that lives
in predictable locations with no duplication, no dead links, and no
stale references — so that I can trust what I read and find what I
need without second-guessing which version is current.

## Completed Work

All acceptance criteria have been fulfilled:

### Phase 1: Remove — Done

- [x] `.github/scripts/` directory and all contents deleted.
- [x] Stale process docs deleted: `docs/process/`, `docs/ci/`,
      `docs/governance/story-yaml-governance.md`.
- [x] Empty placeholders deleted: `docs/changelog.md`,
      `docs/process/definition-of-done.md`.
- [x] `docs/decisions/` (empty `.gitkeep`) deleted.

### Phase 2: Consolidate — Done

- [x] Root `CONTRIBUTING.md` is the single contributing reference.
      `docs/CONTRIBUTING.md` merged and deleted.
- [x] `docs/governance.md` merged into `docs/governance/governance.md`.
      Loose file deleted.
- [x] `docs/runbook.md` merged into `docs/runbooks/runbook.md`.
      Loose file deleted.
- [x] No file or directory in `docs/` is empty after consolidation.

### Beyond Original Scope

- [x] `planning/` directory retired entirely (ADR-0010). VISION.md
      relocated to `product/VISION.md`. Conventions merged into
      `CONTRIBUTING.md`.
- [x] `docs/README.md` navigation hub created.
- [x] Stale references in `docs/governance/governance.md` updated.

### Verification — Done

- [x] `git grep` for `story-yaml` and `sprint-yaml` returns zero
      matches in non-archived files.
- [x] Every internal markdown link in `docs/` resolves to an
      existing file.
- [x] No two files in the repo serve the same purpose.

## Decision Record

- ADR-0010: Planning YAML infrastructure retired.
- US-012 / ADR-0009: Naming convention formalized.
- `planning/` directory: Retired (April 2026 backlog grooming).

