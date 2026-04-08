# ADR-0010: Retire Planning YAML Infrastructure

| Field     | Value              |
|-----------|--------------------|
| Status    | Accepted           |
| Date      | 2026-04-08         |
| Deciders  | Frank Hughes       |

## Context

The project maintained a parallel planning system alongside GitHub's
native project management tools:

- **36 planning YAMLs** (`planning/stories/`) — one per product story,
  containing metadata (id, points, priority, labels, branch, issue
  number, source file path).
- **3 sprint manifests** (`planning/sprints/`) — listing stories per
  sprint with dates, capacity, and goals.
- **7 bootstrap scripts** (`.github/scripts/`) — designed to read
  YAMLs and create GitHub Issues, board cards, and milestones.

This system was designed for a "backlog as code" model where planning
artifacts lived in version control and automation scripts synchronized
them to GitHub. In practice:

1. The bootstrap scripts became stale after Sprint 0 — all Issues were
   created manually or by Copilot, not by the scripts.
2. Every story existed in four places: product story file, planning
   YAML, GitHub Issue, and project board card. Changes to one required
   updating the others manually.
3. Sprint manifests duplicated information already captured by GitHub
   Milestones.
4. Creating one story required touching five artifacts — a workflow
   the Product Owner described as "painful."

The planning YAMLs had no active consumer. The metadata they carried
(points, priority, labels) lives naturally on GitHub Issues via project
board custom fields. The sprint history they recorded is now captured
by sprint review documents (`docs/sprint-reviews/`).

## Decision

Retire the `planning/` directory and `.github/scripts/` entirely.
Establish three sources of truth:

| Source | Owns | Location |
|--------|------|----------|
| Product stories | The WHAT — intent, AC, emotional guarantees | `product/stories/<domain>/` |
| GitHub Issues + Board | The WHEN — sprint assignment, status, points | GitHub Projects |
| ADRs | The WHY — architecture decisions | `docs/adr/` |

New story creation workflow:

1. Write `product/stories/<domain>/US-NNN-title.md` — commit via PR.
2. `gh issue create --title "US-NNN: Title" --milestone "Sprint N"`
   — board auto-populates.

Product vision relocates from `planning/VISION.md` to
`product/VISION.md`. Planning conventions merge into `CONTRIBUTING.md`.

## Consequences

### Positive

- Story creation drops from 5 artifacts to 2.
- No dual-maintenance burden — each fact lives in exactly one place.
- Contributors interact with standard GitHub workflows, not a custom
  YAML system.
- 46 files removed from the repository.

### Negative

- Sprint-level metadata (capacity, velocity, goal) is no longer
  version-controlled. Sprint review documents serve as the historical
  record instead.
- Backlog-as-code versioning is lost — GitHub Issues do not provide
  a git-auditable change history for metadata like points or priority.

### Neutral

- The planning YAMLs served their purpose during the bootstrap phase
  (Sprints 0-2). They established the discipline that made this
  transition possible.
- The 36 retired YAMLs and 3 manifests remain in git history and can
  be recovered if the model needs to be revisited.
