# Planning Runbook

## User Story

As a team member, I want a step-by-step planning runbook — so that
anyone can create stories, start sprints, run ceremonies, and close
sprints independently, without relying on tribal knowledge.

## Context

The project follows a streamlined planning workflow built on three
sources of truth (ADR-0010):

| Source | Owns | Location |
|--------|------|----------|
| Product stories | The WHAT — intent, AC, emotional guarantees | `product/stories/<domain>/` |
| GitHub Issues + Board | The WHEN — sprint assignment, status, points | GitHub Projects |
| ADRs | The WHY — architecture decisions | `docs/adr/` |

During Sprints 0–2, planning ceremonies were performed ad hoc by the
Product Owner. The runbook codifies these ceremonies so that any team
member can execute them, reducing bus-factor risk and onboarding time.

### What Has Changed

This story replaces an earlier version that targeted a YAML-based
planning pipeline (product spec → YAML authoring → schema rules →
script execution → board verification). That pipeline was retired
by ADR-0010. The planning workflow is now two steps: write a product
story, create a GitHub Issue.

## Scope

| Deliverable | Path |
|-------------|------|
| Planning runbook | `docs/runbooks/planning-runbook.md` |
| Product story | `product/stories/infra/US-022-planning-runbook.md` |

## Acceptance Criteria

### Story Creation

- [ ] Documents the 2-step story creation workflow:
      1. Write `product/stories/<domain>/US-NNN-title.md` using the
         template (`product/stories/_template.md`). Commit via PR.
      2. Create a GitHub Issue:
         `gh issue create --title "US-NNN: Title" --milestone "Sprint N"`
- [ ] Explains how to determine the next available story number
      (check highest `US-NNN` in `product/stories/`).
- [ ] Explains the naming convention (`US-{NNN}-{kebab-name}.md`)
      and links to ADR-0009.
- [ ] Lists the available story domains (`infra`, `docs`, `customer`)
      and when to use each.

### Sprint Planning

- [ ] Documents how to start a new sprint:
      1. Create a GitHub Milestone with sprint name, dates, and goal.
      2. Pull stories from the backlog onto the sprint board.
      3. Assign story points via the project board's custom fields.
      4. Verify total capacity does not exceed team velocity.
- [ ] Explains how to set sprint goals that are specific and
      measurable.

### Sprint Execution

- [ ] Documents the branch-per-story workflow:
      one story = one branch = one PR (squash-merge, delete after).
- [ ] Explains branch naming conventions (`feature/`, `chore/`,
      `fix/`, `docs/`).
- [ ] Documents the PR review and merge process.
- [ ] Links to `CONTRIBUTING.md` for commit message conventions.

### Sprint Closure

- [ ] Documents how to close a sprint:
      1. Copy `docs/sprint-reviews/_template.md` to
         `docs/sprint-reviews/sprint-N.md` and fill in all sections.
      2. Add a new section to `CHANGELOG.md` with all changes.
      3. Update the "Current Status" section in `README.md`.
      4. Close the GitHub Milestone.
- [ ] Explains the sprint review document format (What Shipped,
      Key Decisions, Metrics, What Went Well, What Could Improve,
      Next Sprint Focus).

### Backlog Grooming

- [ ] Documents how to audit the backlog for stale or obsolete
      stories (check AC against current repo state, verify paths
      and dependencies still exist).
- [ ] Documents how to retire a story (delete the file, close the
      Issue as "not planned," note the reason).
- [ ] Documents how to mark a story as fulfilled (close the Issue,
      optionally update the story file with completion status).

### Troubleshooting

- [ ] Includes a troubleshooting section covering:
      - Story number collision (two stories claim the same US-NNN)
      - Orphaned Issues (Issue exists but no product story file)
      - Orphaned stories (product story exists but no GitHub Issue)
      - Merge conflicts in `CHANGELOG.md` or `README.md` status

### Cross-References

- [ ] Linked from `docs/README.md` (operations section).
- [ ] Linked from `CONTRIBUTING.md` (story workflow section).
- [ ] Linked from `docs/runbooks/` directory.

## Dependencies

- ADR-0009: Story naming convention (merged).
- ADR-0010: Planning YAML infrastructure retired (merged).
- `docs/sprint-reviews/_template.md` (merged).
- `CHANGELOG.md` (merged).
- `CONTRIBUTING.md` with story workflow section (merged).

## Open Questions

- Should the runbook include a "Sprint 0 from Scratch" section for
  bootstrapping a brand-new project, or focus only on steady-state
  sprints?
- Should the runbook cover how to add a new story domain directory,
  or is that self-evident from the convention?

## Decision Record

- ADR-0010: [Retire Planning YAML Infrastructure](../../docs/adr/0010-retire-planning-yaml-infrastructure.md)
