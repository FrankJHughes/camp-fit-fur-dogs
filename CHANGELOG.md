# Changelog

All notable changes to this project will be documented in this file.

## [Sprint 2 Cleanup] — 2026-04-08

### Added

- ADR-0010 — Retire Planning YAML Infrastructure
- ADR-0009 — Story Naming Convention
- `docs/README.md` navigation hub
- Sprint review template and Sprint 2 review
- CHANGELOG.md, README status section

### Changed

- All 44 stories renamed to `US-{NNN}-{kebab-name}.md` (ADR-0009)
- VISION.md relocated from `planning/` to `product/`
- CONTRIBUTING.md rewritten — merged planning conventions, added 2-step story workflow
- `docs/governance/governance.md` — fixed 6 stale cross-references
- US-006 directory convention fixed (`features/` → `customer/`)
- US-009/010/011 scope paths fixed (`docs/contributing/` → `docs/guides/`)
- US-022 rewritten for 2-step planning workflow, absorbed US-010/US-011

### Removed

- `planning/` directory (36 YAMLs, 3 manifests, 1 epic, 1 README)
- `.github/scripts/` (7 stale bootstrap scripts)
- `docs/process/`, `docs/ci/`, `docs/decisions/` (stale YAML-era docs)
- `docs/changelog.md`, `docs/CONTRIBUTING.md`, `docs/governance.md`, `docs/runbook.md` (duplicates consolidated)
- US-021 (Post-Merge Sprint Bootstrap) — obsoleted by ADR-0010
- US-023 (Sprint Manifest Template) — obsoleted by ADR-0010
- US-010 (PO Contributor Guide) — absorbed into US-022
- US-011 (SM Contributor Guide) — absorbed into US-022

### Completed

- US-008: Doc Audit & Defragmentation
- US-012: Story Naming Convention
- US-024: Planning Conventions README

## [Sprint 2 Grooming] — 2026-04-08

### Added

- All 44 product stories renamed to `US-{NNN}` convention (PR #79)
- ADR-0009: Story Naming Convention decision record
- US-012: Story Naming Convention product story
- US-006 and US-007 rewritten for markdown-only workflow

## [Sprint 2] — 2026-04-03

### Added

- ADR-0008: Consistent Editor Experience
- US-008: Doc Audit & Defragmentation story

## [Sprint 1] — 2026-03-30

### Added

- ADR-0003 through ADR-0007 (DX toolchain decisions)
- Product story scaffold (US-006, US-007)
- Sprint board (#14) and milestone structure
- `.editorconfig`, `.vscode/` settings
- `Makefile` with standardized commands
- `compose.yml` for infrastructure
- `.devcontainer/` configuration

## [Sprint 0] — 2026-03-27

### Added

- Repository initialization
- ADR-0001: Use lightweight ADRs
- ADR-0002: DDD layered architecture
- CONTRIBUTING.md, PR template, `.gitignore`
- .NET solution skeleton
- CI baseline (build and test)
- Product vision, capability map, emotional guarantees
- 44 product stories across infra, docs, and customer domains
