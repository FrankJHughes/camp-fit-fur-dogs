# Planning Conventions

> **Audience:** Anyone creating, reviewing, or syncing planning artifacts.
> **Scope:** Conventions only — sprint ceremonies are covered by the
> [Planning Runbook](../product/stories/infra/planning-runbook.md).

---

## Artifact Organization

Planning artifacts live in three collections, plus a vision anchor:

| Collection | Purpose |
|---|---|
| **epics** | High-level themes that group related stories |
| **sprints** | Sprint manifests listing the stories committed to a sprint |
| **stories** | Individual units of deliverable work, grouped by sprint |
| **VISION.md** | Stable product vision — revisit only when direction changes |

Each collection contains YAML files that serve as the source of truth
for GitHub Issues, milestones, and the project board.  Automation
scripts read these files to create and sync GitHub artifacts.

---

## Naming Conventions

### Files

- **Lowercase kebab-case** for all planning YAML files
  (e.g., `merge-protection-governance.yml`)
- File names match the story slug used in branch names and issue titles
- Use `.yml` (not `.yaml`) for consistency with existing artifacts

### Branches

Planning work follows the same branch conventions as all repo work
(see [CONTRIBUTING.md](../docs/CONTRIBUTING.md#branch-naming)):

| Type | Pattern | Example |
|---|---|---|
| Feature | `feature/<story-slug>` | `feature/ci-baseline-build-and-test` |
| Chore | `chore/<story-slug>` | `chore/planning-conventions-readme` |
| Fix | `fix/<description>` | `fix/sprint-manifest-typo` |
| Docs | `docs/<description>` | `docs/vision-update` |

Always branch from `main`.  Keep names lowercase, kebab-case.

---

## Story Lifecycle

A story moves through five stages from idea to completion:

```
Idea → Product Spec → Story YAML → Sprint Board → Done
```

### 1. Idea

A need is identified — a gap, a dependency, or a capability the
product requires.  Ideas may surface during sprint retrospectives,
backlog grooming, or ad-hoc discovery.

### 2. Product Spec

The idea is written up as a product specification in `product/stories/`
under the appropriate theme directory.  The spec captures intent,
value, acceptance criteria, and emotional guarantees (if applicable).

### 3. Story YAML

A corresponding YAML file is created in `planning/stories/` under the
target sprint directory.  The YAML file references the product spec
and adds implementation metadata: points, priority, labels, branch
name, and tasks.

### 4. Sprint Board

> **Full bootstrap guide:**
> [Sprint Bootstrap Process](../docs/process/sprint-bootstrap.md)

Automation scripts (`create-sprint.sh`, `create-stories.sh`) read the
YAML files and create GitHub Issues, assign milestones, apply labels,
and add items to the project board.  Scripts stamp each YAML with
`.issue_number` and `.created` to ensure idempotent re-runs.

### 5. Done

The story is implemented via a PR against `main`, passes CI, and is
squash-merged.  The issue moves to Done on the project board.

---

## Atomic-PR Convention

**Product spec and story YAML ship together in the same PR.**

When a new story is introduced, the PR must include both:

1. The product specification (`product/stories/<theme>/<slug>.md`)
2. The planning YAML (`planning/stories/<sprint>/<slug>.yml`)

This prevents orphaned specs (product files with no corresponding
planning artifact) and orphaned YAMLs (planning files with no product
rationale).  The only exception is when updating metadata on an
existing story YAML (e.g., adding `.issue_number` stamps after a board
sync) — those changes do not require a product spec update.

---

## Branch Hygiene for Planning Work

- **One story = one branch = one PR.**
  Never bundle unrelated planning changes.
- **Delete the branch after merge.**
  Stale branches create confusion about what's in flight.
- **Squash-merge into `main`.**
  Keeps the history linear and each commit meaningful.
- **Rebase, don't merge, when catching up with `main`.**
  Avoids merge commits that obscure the planning changelog.
- **Stamp files before committing.**
  If a script stamps YAMLs (e.g., `.issue_number`, `.created`,
  `.board_synced`), commit those stamps promptly so re-runs are
  idempotent.

---

## When an ADR Is Required

An [Architecture Decision Record](../docs/adr/README.md) is required
when a change:

- **Selects or replaces a technology, framework, or tool** that other
  work depends on (e.g., task runner, container orchestration, ORM)
- **Establishes a structural pattern** that future stories must follow
  (e.g., folder conventions, dependency flow, API contract style)
- **Changes a previously accepted decision** — superseding an existing
  ADR requires a new ADR that references the old one
- **Introduces a cross-cutting constraint** that affects multiple
  epics or themes (e.g., authentication strategy, CI pipeline design)

An ADR is **not** required for:

- Routine story implementation that follows established patterns
- Documentation-only changes
- Bug fixes or cosmetic adjustments
- Planning artifact updates (YAML, specs, manifests)

When in doubt, write the ADR.  A lightweight record is cheaper than
an undocumented decision that someone re-derives later.