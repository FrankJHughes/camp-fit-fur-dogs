---
id: US-022
title: "Planning Runbook"
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
# Planning Runbook

## User Story

As a team member, I want a comprehensive planning and process runbook
— so that anyone can create stories, run ceremonies, manage governance
artifacts, and close sprints independently, without relying on tribal
knowledge.

## Context

The project follows a streamlined planning workflow built on three
sources of truth (ADR-0010):

| Source | Owns | Location |
|--------|------|----------|
| Product stories | The WHAT — intent, AC, emotional guarantees | `product/stories/<domain>/` |
| GitHub Issues + Board | The WHEN — sprint assignment, status, points | GitHub Projects |
| ADRs | The WHY — architecture decisions | `docs/adr/` |

This runbook consolidates all planning, process, and governance
knowledge into a single reference. It absorbs content previously
scoped to separate PO and SM guides (US-010, US-011) to eliminate
overlap and ensure every fact lives in one place.

### What Has Changed

- Replaces YAML-era planning runbook (Sprint 2 version).
- Absorbs US-010 (Product Owner Contributor Guide) — AC patterns,
  story sizing, backlog prioritization.
- Absorbs US-011 (Scrum Master Contributor Guide) — DoR, DoD,
  ceremony facilitation, governance artifacts, impediment tracking.

## Scope

| Deliverable | Path |
|-------------|------|
| Planning runbook | `docs/runbooks/planning-runbook.md` |
| Product story | `product/stories/infra/US-022-planning-runbook.md` |

## Acceptance Criteria

### Story Creation

- [x] Documents the 2-step story creation workflow:
      1. Write `product/stories/<domain>/US-NNN-title.md` using the
         template (`product/stories/_template.md`). Commit via PR.
      2. Create a GitHub Issue:
         `gh issue create --title "US-NNN: Title" --milestone "Sprint N"`
- [x] Explains how to determine the next available story number
      (check highest `US-NNN` in `product/stories/`).
- [x] Explains the naming convention (`US-{NNN}-{kebab-name}.md`)
      and links to ADR-0009.
- [x] Lists the available story domains (`infra`, `docs`, `customer`)
      and when to use each.
- [x] Documents when a story requires a companion ADR (technology
      or tool selection, structural patterns, cross-cutting
      constraints).

### Acceptance Criteria Patterns *(absorbed from US-010)*

- [x] Explains what makes AC testable: observable, binary
      (pass/fail), independent of implementation details.
- [x] Provides 3–5 examples of well-written AC with annotations
      explaining why they work.
- [x] Provides 3–5 anti-pattern examples (vague language, compound
      conditions, implementation-coupled) with corrected versions.
- [x] Documents the emotional safety guarantee section — when to
      include it, how to write guarantees that are verifiable.

### Story Sizing and Prioritization *(absorbed from US-010)*

- [x] Explains the point scale used by the project and what each
      level represents (complexity, not time).
- [x] Documents prioritization criteria: customer value, technical
      dependency, risk reduction, team capacity.
- [x] Explains how to split stories that exceed the sprint capacity
      threshold.

### Sprint Planning

- [x] Documents how to start a new sprint:
      1. Create a GitHub Milestone with sprint name, dates, and goal.
      2. Pull stories from the backlog onto the sprint board.
      3. Assign story points via the project board's custom fields.
      4. Verify total capacity does not exceed team velocity.
- [x] Explains how to set sprint goals that are specific and
      measurable.
- [x] Documents board column flow (Backlog → Ready → In Progress →
      In Review → Done) and what triggers each transition.

### Definition of Ready *(absorbed from US-011)*

- [x] Provides a DoR checklist that a story must satisfy before
      entering a sprint:
      - Product story file exists with all required sections.
      - AC are testable and reviewed.
      - Dependencies are identified and unblocked.
      - Points are assigned.
      - GitHub Issue exists and is linked to the milestone.
- [x] Documents how to push back when a story does not meet DoR
      (move to backlog, add "needs-refinement" label, note the gap).

### Definition of Done *(absorbed from US-011)*

- [x] Provides a DoD checklist that a story must satisfy before
      closing:
      - All AC pass.
      - PR is squash-merged to main.
      - Branch is deleted.
      - CI passes on main after merge.
      - GitHub Issue is closed.
      - Sprint review entry is drafted (if sprint boundary).
      - No regressions introduced (existing tests pass).
- [x] Documents the distinction between "merged" and "done" — a
      story is not done until post-merge verification is complete.

### Sprint Execution

- [x] Documents the branch-per-story workflow:
      one story = one branch = one PR (squash-merge, delete after).
- [x] Explains branch naming conventions (`feature/`, `chore/`,
      `fix/`, `docs/`).
- [x] Documents the PR review and merge process.
- [x] Links to `CONTRIBUTING.md` for commit message conventions.

### Ceremony Facilitation *(absorbed from US-011)*

- [x] Documents sprint review format: What Shipped table, Key
      Decisions, Metrics, What Went Well, What Could Improve,
      Next Sprint Focus.
- [x] Documents retrospective approach: what formats to use
      (Start/Stop/Continue, 4Ls, timeline), how to capture
      action items, how to track follow-through.
- [x] Documents refinement cadence: when to groom, how many
      stories to refine per session, exit criteria for a
      grooming session.
- [x] Documents async standup format for solo/distributed work:
      what was done, what's next, any blockers.

### Sprint Closure

- [x] Documents how to close a sprint:
      1. Copy `docs/sprint-reviews/_template.md` to
         `docs/sprint-reviews/sprint-N.md` and fill in all sections.
      2. Add a new section to `CHANGELOG.md` with all changes.
      3. Update the "Current Status" section in `README.md`.
      4. Close the GitHub Milestone.

### Governance Artifacts *(absorbed from US-011)*

- [x] Documents the ADR lifecycle: when to create, required
      sections, review process, status transitions (Proposed →
      Accepted → Superseded/Deprecated).
- [x] Documents branch protection rules and who can override.
- [x] Documents the merge checklist: CI green, PR approved,
      no unresolved conversations, squash-merge only.
- [x] References `docs/governance/governance.md` for role
      definitions and ceremony schedules.

### Backlog Grooming

- [x] Documents how to audit the backlog for stale or obsolete
      stories (check AC against current repo state, verify paths
      and dependencies still exist).
- [x] Documents how to retire a story (delete the file, close the
      Issue as "not planned," note the reason).
- [x] Documents how to mark a story as fulfilled (close the Issue,
      optionally update the story file with completion status).

### Impediment Tracking *(absorbed from US-011)*

- [x] Documents how to flag a blocked story (add "blocked" label,
      note the blocker in the Issue, escalate if needed).
- [x] Documents escalation paths: technical blockers (spike story),
      external blockers (document and defer), scope disputes
      (PO decision).

### Troubleshooting

- [x] Includes a troubleshooting section covering:
      - Story number collision (two stories claim the same US-NNN)
      - Orphaned Issues (Issue exists but no product story file)
      - Orphaned stories (product story exists but no GitHub Issue)
      - Merge conflicts in `CHANGELOG.md` or `README.md` status

### Cross-References

- [x] Linked from `docs/README.md` (operations section).
- [x] Linked from `CONTRIBUTING.md` (story workflow section).
- [x] Linked from `docs/runbooks/` directory.

## Dependencies

- ADR-0009: Story naming convention (merged).
- ADR-0010: Planning YAML infrastructure retired (merged).
- `docs/sprint-reviews/_template.md` (merged).
- `CHANGELOG.md` (merged).
- `CONTRIBUTING.md` with story workflow section (merged).
- `docs/governance/governance.md` (merged).

## Open Questions

- Should the runbook include a "Sprint 0 from Scratch" section for
  bootstrapping a brand-new project, or focus only on steady-state
  sprints?
- Should the retrospective section include specific templates (e.g.,
  a markdown retro template in `docs/sprint-reviews/`)?

## Decision Record

- ADR-0010: [Retire Planning YAML Infrastructure](../../docs/adr/0010-retire-planning-yaml-infrastructure.md)
- US-010 absorbed: Product Owner Contributor Guide (retired).
- US-011 absorbed: Scrum Master Contributor Guide (retired).

