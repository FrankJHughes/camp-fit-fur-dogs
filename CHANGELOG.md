# Changelog

All notable changes to Camp Fit Fur Dogs are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Pending

- US-008: Documentation audit and defragmentation
- US-006: Story scaffold tool
- US-007: Planning artifact validation
- US-009: Contributor quick-start guide
- US-010: PR review checklist
- US-011: Issue triage guide
- US-027–US-044: Customer-facing feature stories

## [Sprint 2 Grooming] — 2026-04-08

### Added

- US-012 product story — Story Naming Convention
- ADR-0009 — Story Naming Convention decision record

### Changed

- Renamed all 44 product stories to `US-{NNN}-{kebab-name}.md` (PR #79)
- Renamed 36 planning YAMLs to match; migrated `.yaml` → `.yml` (PR #79)
- Updated 3 sprint manifests with new file paths (PR #79)
- Prefixed 18 GitHub issue titles with `US-{NNN}:` format
- Rewrote US-008 scope — retained `planning/` directory, removed
  completed work (PR #80)
- Updated US-006 and US-007 specs for `US-{NNN}` naming pattern (PR #81)

### Removed

- 3 duplicate/superseded files (PR #79)
- Closed Issue #14 (superseded by US-016 / Issue #17)
- Closed Issue #5 (Epic: Repo Foundation — all children complete)

## [Sprint 2] — 2026-04-24

Sprint goal: Establish planning infrastructure and DX architecture
foundation so that future sprints can onboard contributors and ship
features efficiently.

### Added — Sprint Planning Experience (11 pts)

- US-020: Merge Protection Governance (Issue #47)
- US-021: Post-Merge Sprint Bootstrap (Issue #48)
- US-022: Planning Runbook (Issue #49)
- US-023: Sprint Manifest Template (Issue #50)

### Added — Backlog Grooming Experience (7 pts)

- US-024: Planning Conventions README (Issue #52)

### Added — Developer Onboarding (15 pts)

- US-025: DX Architecture Decision (Issue #54)
- US-005: One-Command Local Bootstrap (Issue #55)
- US-026: Declarative Infrastructure Dependencies (Issue #56)
- US-004: Standardized Developer Commands (Issue #57)
- US-002: Containerized Development Environment (Issue #58)
- US-003: Consistent Editor Experience (Issue #59)

**Velocity:** 33 / 33 capacity

## [Sprint 1] — 2026-04-10

Sprint goal: Stand up the architecture spine — ADR foundation and
domain model wiring.

### Added — Architecture Spine

- US-016: ADR Foundation (Issue #17)
- US-017: Shared Kernel DDD Building Blocks (Issue #18)
- US-018: Domain Model Core Aggregates (Issue #19)
- US-019: API DDD Layer Wiring (Issue #20)

## [Sprint 0] — 2026-04-03

Sprint goal: Establish repo foundation — CI, contributing guide,
and solution skeleton.

### Added — Repo Foundation

- US-015: CI Baseline Build and Test (Issue #8)
- US-013: CONTRIBUTING Guide, PR Template, .gitignore (Issue #6)
- US-014: .NET Solution Skeleton (Issue #7)
