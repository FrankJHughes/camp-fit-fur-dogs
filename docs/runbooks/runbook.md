# Planning Runbook

> The end-to-end ceremony for turning an idea into a tracked, board-ready
> sprint item.  Follow these phases in order.
>
> **Related docs:**
> - [Planning Conventions](../planning/README.md) — naming rules,
>   artifact organization, atomic-PR convention
> - [Sprint Bootstrap Process](process/sprint-bootstrap.md) — script
>   internals, idempotency guarantees, troubleshooting

---

## Phase 1 — Write the Product Spec

Every story begins as a product specification that captures **why** the
work matters before **how** it will be built.

### Where specs live

```
product/stories/<theme>/<slug>.md
```

Themes group related specs by domain (e.g., `infra`, `customer`,
`developer-experience`).

### What a spec contains

| Section | Required | Purpose |
|---|---|---|
| **Intent** | Yes | One sentence: what does this change achieve? |
| **Value** | Yes | Why does this matter to the product or team? |
| **Acceptance Criteria** | Yes | Checkboxes — testable conditions for Done |
| **Emotional Guarantees** | If applicable | Safety promises the product makes to users |

### Rules

- Write in plain English.  A non-engineer should understand the intent.
- Acceptance criteria are pass/fail — no subjective language.
- One spec per story.  If the spec grows beyond one screen, split it.

---

## Phase 2 — Author the Story YAML

The story YAML adds implementation metadata to the product spec and
becomes the source of truth for automation.

### Where YAMLs live

```
planning/stories/<sprint>/<slug>.yml
```

### Schema reference

This project uses two story schemas.  Automation detects which schema
to use based on the presence of a `.description` field.

#### Schema 1 — Infrastructure stories

For internal/tooling work where the YAML itself carries all content:

```yaml
title: "Story Title"
description: |
  Inline description of the work.
tasks:
  - "Task one"
  - "Task two"
acceptance:
  - "Criterion one"
  - "Criterion two"
epic: "Epic Name"
labels:
  - infra
  - automation
points: 2
priority: P0
branch: "chore/story-slug"
source:
  productFile: "/product/stories/infra/story-slug.md"
```

Required fields: `title`, `description`, `tasks`, `acceptance`, `epic`,
`labels`, `points`, `priority`, `branch`, `source.productFile`.

#### Schema 2 — Customer stories

For user-facing work where the product spec markdown is the body:

```yaml
id: STORY-ID
title: "Story Title"
type: story
capability: "Capability Name"
epic: "Epic Name"
labels:
  - customer
  - feature
points: 3
priority: P0
branch: "feature/story-slug"
source:
  productFile: "/product/stories/customer/story-slug.md"
```

Required fields: `id`, `title`, `type`, `capability`, `epic`, `labels`,
`points`, `priority`, `branch`, `source.productFile`.

The issue body is built from the linked product spec markdown, not from
inline YAML fields.

### Validation checklist

Before committing a story YAML:

- [ ] `title` matches the product spec heading
- [ ] `source.productFile` path is correct and the file exists
- [ ] `branch` follows the naming convention (see [Planning Conventions](../planning/README.md#branches))
- [ ] `labels` include at least one theme label
- [ ] `points` reflects relative effort (1 = trivial, 5 = large)
- [ ] `priority` is set (P0 = must-have, P1 = should-have, P2 = nice-to-have)
- [ ] Story appears in the sprint manifest's `.stories` array

---

## Phase 3 — Update the Sprint Manifest

The sprint manifest lists every story committed to the sprint and
provides metadata for milestone creation.

### Where manifests live

```
planning/sprints/<sprint-name>.yml
```

### Manifest structure

```yaml
sprint:
  name: "Sprint N"
  start: "YYYY-MM-DD"
  end: "YYYY-MM-DD"
  goal: "One-sentence sprint goal"
stories:
  - planning/stories/sprint-n/story-one.yml
  - planning/stories/sprint-n/story-two.yml
```

### Rules

- Every story YAML committed for this sprint **must** appear in the
  `.stories` array.
- The `name` field becomes the GitHub Milestone title — keep it stable.
- Dates drive milestone due-date and are used for reporting.

---

## Phase 4 — Ship the Atomic PR

Product spec and story YAML ship together in the same PR.  This is the
[Atomic-PR Convention](../planning/README.md#atomic-pr-convention).

### PR checklist for planning artifacts

- [ ] Product spec exists at `product/stories/<theme>/<slug>.md`
- [ ] Story YAML exists at `planning/stories/<sprint>/<slug>.yml`
- [ ] Story is listed in the sprint manifest `.stories` array
- [ ] Branch name matches the story's `.branch` field
- [ ] Commit message follows [Conventional Commits](../docs/CONTRIBUTING.md#commit-messages)
- [ ] CI passes

Merge the PR to `main`.  Planning artifacts must be on `main` before
the bootstrap scripts can process them.

---

## Phase 5 — Bootstrap the Sprint

After planning artifacts are merged, run the bootstrap script to
create the milestone, issues, and board assignments.

### One-command standup

```bash
.github/scripts/bootstrap-sprint.sh planning/sprints/sprint-n.yml
```

### What the script does

1. Creates the GitHub Milestone (or reuses an existing one)
2. Creates a GitHub Issue for each story YAML
3. Applies labels and assigns issues to the milestone
4. Adds each issue to the project board
5. Stamps idempotency metadata into the YAML files
6. Runs 5 automated verification checks

> **Full details:** [Sprint Bootstrap Process](process/sprint-bootstrap.md)

### Preview mode

```bash
.github/scripts/bootstrap-sprint.sh --dry-run planning/sprints/sprint-n.yml
```

---

## Phase 6 — Commit the Stamps

The bootstrap script modifies YAML files on disk (adding `.issue_number`,
`.created`, `.board_synced`).  These stamps **must** be committed back
to `main` so future re-runs are idempotent.

```bash
git checkout -b chore/sprint-n-board-stamps
git add planning/
git commit -m "chore: stamp Sprint N board-sync metadata"
git push -u origin chore/sprint-n-board-stamps
gh pr create \
    --title "chore: stamp Sprint N board-sync metadata" \
    --body "Persists idempotency stamps from bootstrap-sprint.sh." \
    --base main
```

After the stamps PR merges, the sprint is fully stood up and ready
for work.

---

## Phase 7 — Verify the Board

Open the project board and confirm:

- [ ] All sprint issues appear in the **Todo** column
- [ ] Each issue has the correct labels and milestone
- [ ] No duplicate issues exist
- [ ] Prior sprint items remain in their correct columns

**Board URL:** https://github.com/users/frankjhughes/projects/14

---

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `bootstrap-sprint.sh` reports "Missing required tools" | `gh`, `yq`, or `jq` not installed | Install: `brew install gh yq jq` or `choco install gh yq jq` |
| "Could not find project board #14" | `PROJECT_NUMBER` mismatch or `gh` not authenticated | Run `gh auth status` then `gh project view 14 --owner frankjhughes` |
| Duplicate issues created | Stamps weren't committed after a previous run | Delete duplicates in GitHub UI, then re-run with `--clean` |
| Issue body is empty or malformed | Schema detection failed — `.description` present but empty | Ensure `.description` has content (Schema 1) or remove it entirely (Schema 2) |
| "Milestone already exists" (not an error) | Expected — `create-sprint.sh` reuses existing milestones | No action needed; this is idempotent behavior |
| Labels not created | `ensure_labels` failed silently | Run `gh label list -R frankjhughes/camp-fit-fur-dogs` to verify; create manually if needed |
| Story YAML not processed | File missing from sprint manifest `.stories` array | Add the path to the manifest and re-run |
| PR template not rendered | `.github/PULL_REQUEST_TEMPLATE.md` missing | Verify the file exists on `main` (shipped in #47) |
| Board shows items in wrong column | Items default to first column on creation | Drag items manually or set column via GraphQL |

---

## Quick Reference

| Phase | Command or Action | Output |
|---|---|---|
| 1. Product Spec | Write `product/stories/<theme>/<slug>.md` | Spec file |
| 2. Story YAML | Write `planning/stories/<sprint>/<slug>.yml` | YAML file |
| 3. Sprint Manifest | Add story path to `planning/sprints/<sprint>.yml` | Updated manifest |
| 4. Atomic PR | `gh pr create ...` | Merged PR |
| 5. Bootstrap | `bootstrap-sprint.sh <manifest>` | Milestone + issues + board |
| 6. Stamp PR | Commit stamps → merge | Idempotency persisted |
| 7. Verify | Check board UI | Sprint ready |