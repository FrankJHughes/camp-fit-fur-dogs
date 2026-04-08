# Documentation Audit and Defragmentation

## User Story

As a contributor joining the project, I want documentation that lives
in predictable locations with no duplication, no dead links, and no
stale references — so that I can trust what I read and find what I
need without second-guessing which version is current.

## Context

An audit of the repository at the close of Sprint 2 revealed
significant documentation fragmentation. The project evolved through
three planning eras (YAML-based, hybrid, markdown-based), and each
left artifacts behind without cleaning up the previous generation.

### Fragmentation Inventory

**Duplicate files — same concept, two locations:**

- `CONTRIBUTING.md` (root, 381B) AND `docs/CONTRIBUTING.md` (1960B)
- `docs/decisions/` (empty `.gitkeep`) AND `docs/adr/` (8 real ADRs)
- `docs/governance.md` (loose file) AND `docs/governance/` (directory)
- `docs/runbook.md` (loose file) AND `docs/runbooks/` (directory)
- `product/definition-of-ready/` AND `docs/process/definition-of-ready.md`

**Empty placeholder files — never populated:**

- `docs/changelog.md` (0 bytes)
- `docs/process/definition-of-done.md` (0 bytes)

**Stale YAML-era process docs — reference artifacts that no longer exist:**

- `docs/process/story-yaml-spec.md`
- `docs/process/sprint-yaml-spec.md`
- `docs/process/story-lifecycle.md`
- `docs/governance/story-yaml-governance.md`
- `docs/ci/sprint-readiness-validation.md`
- `docs/process/sprint-bootstrap.md`
- `docs/process/sprint-planning-workflow.md`
- `docs/process/backlog-hygiene.md`

**Stale YAML-era scripts — automate a workflow that no longer exists:**

- `.github/scripts/create-board.sh`
- `.github/scripts/create-epics.sh`
- `.github/scripts/create-sprint.sh`
- `.github/scripts/create-stories.sh`
- `.github/scripts/bootstrap-sprint.sh`
- `.github/scripts/lib.sh`
- `.github/scripts/repo.env`

**Stale planning directory — entire tree from the YAML era:**

- `planning/epics/repo-foundation.yml`
- `planning/sprints/TEMPLATE.yml`, `sprint-0.yml`, `sprint-1.yml`, `sprint-2.yml`
- `planning/stories/US-C01.yaml` through `US-C18.yaml`
- `planning/stories/sprint-0/` (3 YAML files)
- `planning/stories/sprint-1/` (4 YAML files)
- `planning/stories/sprint-2/` (12 YAML files)

**Stale product stories — superseded by updated versions:**

- `product/stories/infra/story.doc-architecture.md` (replaced by this story)
- `product/stories/infra/story.ci-sprint-readiness.md` (subsumed by `planning-artifact-validation.md`)
- `product/stories/infra/story-scaffold-script.md` (superseded by `story-scaffold-tool.md`)
- `product/stories/infra/validate-planning-workflow.md` (superseded by `planning-artifact-validation.md`)

### Emotional Safety Guarantees

- Contributors never encounter contradictory documentation
- Every file a contributor opens is current and trustworthy
- Navigation is predictable: one location per concept
- Changes to documentation do not cascade into broken references
  elsewhere in the repo

## Scope

This story covers two phases:

1. **Remove** — delete stale, duplicate, and empty files
2. **Consolidate** — merge duplicates into single sources of truth

| Phase | Action | Files Affected |
|-------|--------|----------------|
| Remove | Delete stale YAML-era planning artifacts | `planning/` (entire tree) |
| Remove | Delete stale YAML-era scripts | `.github/scripts/` (7 files) |
| Remove | Delete stale process docs | 8 files under `docs/process/`, `docs/ci/`, `docs/governance/` |
| Remove | Delete empty placeholders | `docs/changelog.md`, `docs/process/definition-of-done.md` |
| Remove | Delete empty skeleton dir | `docs/decisions/` |
| Remove | Delete stale product stories | 4 superseded stories under `product/stories/infra/` |
| Consolidate | Merge CONTRIBUTING into root | Root `CONTRIBUTING.md` becomes single source; `docs/CONTRIBUTING.md` deleted |
| Consolidate | Merge governance loose file into dir | `docs/governance.md` content moves into `docs/governance/`; loose file deleted |
| Consolidate | Merge runbook loose file into dir | `docs/runbook.md` content moves into `docs/runbooks/`; loose file deleted |
| Consolidate | Merge Definition of Ready | Single location chosen; duplicate deleted |
| Consolidate | Rename product stories | All stories follow naming convention (see Acceptance Criteria) |

## Acceptance Criteria

### Phase 1: Remove

- [ ] `planning/` directory and all contents are deleted from the repo.
- [ ] `.github/scripts/` directory and all contents are deleted.
- [ ] The following stale process docs are deleted:
      `docs/process/story-yaml-spec.md`,
      `docs/process/sprint-yaml-spec.md`,
      `docs/process/story-lifecycle.md`,
      `docs/process/sprint-bootstrap.md`,
      `docs/process/sprint-planning-workflow.md`,
      `docs/process/backlog-hygiene.md`,
      `docs/governance/story-yaml-governance.md`,
      `docs/ci/sprint-readiness-validation.md`.
- [ ] Empty placeholders are deleted: `docs/changelog.md`,
      `docs/process/definition-of-done.md`.
- [ ] `docs/decisions/` (empty `.gitkeep` only) is deleted.
- [ ] The following superseded product stories are deleted:
      `story.doc-architecture.md`,
      `story.ci-sprint-readiness.md`,
      `story-scaffold-script.md`,
      `validate-planning-workflow.md`.

### Phase 2: Consolidate

- [ ] Root `CONTRIBUTING.md` is the single contributing reference.
      `docs/CONTRIBUTING.md` is deleted. Content is merged if the
      docs version contained material not in the root version.
- [ ] `docs/governance.md` content is merged into a file inside
      `docs/governance/`. The loose file is deleted.
- [ ] `docs/runbook.md` content is merged into a file inside
      `docs/runbooks/`. The loose file is deleted.
- [ ] Definition of Ready exists in exactly one location. The
      duplicate is deleted and any references are updated.
- [ ] All product stories under `product/stories/` follow the
      naming convention defined in the Decision Record below.
- [ ] No file or directory in `docs/` is empty (0 bytes or
      `.gitkeep`-only) after consolidation.

### Verification

- [ ] `git grep` for `story-yaml`, `sprint-yaml`, `source.productFile`,
      and `planning/stories` returns zero matches in any non-archived
      file.
- [ ] Every internal markdown link (`[text](path)`) in `docs/`
      resolves to an existing file.
- [ ] `README.md` doc links (if any) resolve to existing files.
- [ ] No two files in the repo serve the same purpose (verified by
      manual review during PR).

## Dependencies

- Sprint 2 merged work (Diamond Model, ADRs 0001–0008).
- Updated product stories for #51 and #53 (merged).
- Contributor guide stories (merged).

## Open Questions

- Should `docs/process/` survive as a directory, or should its
  remaining valid content move into `docs/governance/`?
- Should the customer stories under `product/stories/customer/`
  be preserved, rewritten in current format, or archived?
- Should a `docs/README.md` index be created as a navigation hub
  for all documentation?
- What naming convention should product stories follow?
  (Pending decision — see elicitation.)

## Decision Record

TBD — naming convention ADR to be created once the convention is
finalized.