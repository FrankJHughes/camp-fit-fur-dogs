# Sprint 3 Review — 2026-04-11

## Sprint Goal

Ship the first customer vertical (M1) end-to-end and complete contributor onboarding guides for all three Scrum roles.

## What Shipped

| Story  | Title                        | Issue | Notes |
|--------|------------------------------|-------|-------|
| US-009 | Developer Contributor Guide  | #98   | Full TDD workflow, branching model, source control safety |
| US-027 | Create Customer Account      | #95   | POST /api/customers — first vertical slice |
| US-028 | Register a Dog               | #96   | POST /api/dogs — ownership guard established |
| US-029 | View Dog Profile             | #97   | GET /api/dogs/{id} — query pipeline introduced |
| US-045 | Product Owner Workflow Guide | #99   | Role-specific contributor guide |
| US-046 | Scrum Master Workflow Guide  | #100  | Role-specific contributor guide |

## Key Decisions

- Introduced CQRS query pipeline (`IQuery<TResponse>`, `IQueryHandler`, `IQueryDispatcher`) alongside existing command pipeline
- Consolidated endpoint routing into `Endpoints.cs` with `MapGroup` pattern
- Added two-layer source control safety: GitHub branch rule (require PR) + local pre-push hook
- Introduced `.github/copilot-instructions.md` as a living conventions document — shared by GitHub Copilot and AI-assisted sessions

## Metrics

| Metric               | Value |
|----------------------|-------|
| Stories committed     | 6     |
| Stories completed     | 6     |
| PRs merged            | 10    |
| Completion rate       | 100%  |
| Tests passing (end)   | 92+   |

## What Went Well

- **M1 milestone complete** — all three vertical slices shipped (Create Customer, Register Dog, View Dog Profile)
- **TDD discipline held** — red-green-refactor followed on every feature slice with no shortcuts
- **CQRS patterns established** — both command and query pipelines reusable for future stories
- **Contributor guides shipped** — all three roles (Dev, PO, SM) have onboarding paths
- **100% completion rate** — every committed story shipped

## What Could Improve

- **Source control mishap** — accidental direct push to `main` mid-sprint; now mitigated with pre-push hook + branch protection rule
- **CHANGELOG hygiene** — fell behind during feature work; had to do a bulk catch-up at the end of sprint
- **Issue vs PR number confusion** — wrong numbers crept into CHANGELOG (PR numbers used instead of issue numbers); need to double-check during PR review

## Next Sprint Focus

> Pull next stories from the backlog, continue building out M2 capabilities, and close the CQRS documentation gap (ADR-0011, developer guide walkthrough, scaffold script).

## Closing Checklist

- [x] CHANGELOG `[Unreleased]` promoted to `[Sprint 3]`
- [x] `.github/copilot-instructions.md` reviewed for staleness
- [x] Lessons learned from retro added to conventions
