# Sprint Bootstrap Process

> How to stand up a new sprint after planning artifacts are merged to `main`.

---

## Prerequisites

| Requirement | How to verify |
|---|---|
| `gh` CLI authenticated with repo admin scope | `gh auth status` |
| `yq` installed (v4+) | `yq --version` |
| `jq` installed | `jq --version` |
| Planning artifacts merged to `main` | Sprint manifest + story YAMLs + product specs all on `main` |
| Project board exists | `gh project view 14 --owner frankjhughes` |

---

## The Sequence

Sprint standup follows a fixed four-phase sequence.  The wrapper script
`bootstrap-sprint.sh` executes all phases in order.

### Phase 1 — Clean stamps (optional)

Strip idempotency stamps from story YAMLs and the sprint manifest.
**Only needed when re-creating a sprint from scratch** (e.g., after
deleting issues to start over).  Normal runs skip this phase.

What gets removed:

- Sprint manifest: `.sprint.board_synced`, `.sprint.milestone`
- Story YAMLs: `.issue_number`, `.created`

### Phase 2 — Create milestone

The script checks whether a milestone matching `.sprint.name` already
exists.  If not, it creates one with the due date from `.sprint.end`.

### Phase 3 — Create issues and assign to board

For each story listed in the sprint manifest:

1. Check if `.issue_number` is already stamped (skip if so)
2. Detect the story schema (infra vs. customer)
3. Build the issue body from YAML fields or product spec markdown
4. Create the GitHub Issue with labels
5. Assign the issue to the milestone
6. Add the issue to the project board
7. Stamp `.issue_number` and `.created` back into the YAML

### Phase 4 — Verify

Five automated checks confirm the sprint is correctly stood up:

| Check | What it validates |
|---|---|
| Milestone exists | The milestone was created and is reachable via API |
| Issue stamps | Every story YAML has a `.issue_number` value |
| Manifest stamp | The sprint manifest has `.sprint.board_synced` |
| Board item count | The project board has at least as many items as stories |
| Milestone assignment | The milestone has at least as many issues as stories |

---

## Running It

### One-command standup (normal)

```bash
.github/scripts/bootstrap-sprint.sh planning/sprints/sprint-3.yml
```

### Dry run (preview without API calls)

```bash
.github/scripts/bootstrap-sprint.sh --dry-run planning/sprints/sprint-3.yml
```

### Full re-creation (clean + create)

```bash
.github/scripts/bootstrap-sprint.sh --clean planning/sprints/sprint-3.yml
```

> **Warning:** `--clean` strips stamps but does **not** delete existing
> GitHub Issues or milestones.  If you need to delete and re-create,
> do that manually in the GitHub UI first, then run with `--clean`.

---

## Idempotency Guarantees

Every script in the bootstrap chain is idempotent:

| Script | Guard |
|---|---|
| `bootstrap-sprint.sh` | Delegates to idempotent scripts below |
| `create-sprint.sh` | Skips if `.sprint.board_synced` is stamped |
| `create-stories.sh` | Skips if `.issue_number` is stamped |
| `create-board.sh` | One-time use (board persists across sprints) |
| Milestone creation | GET-then-POST — reuses existing milestone |
| Board assignment | GraphQL mutation is inherently idempotent |

**Safe to re-run at any time.**  If a run is interrupted (network
error, rate limit), simply re-run — completed items will be skipped.

---

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| "Missing required tools: yq" | `yq` not installed or not in PATH | Install via `brew install yq` or `choco install yq` |
| "Could not find project board #14" | PROJECT_NUMBER mismatch or auth issue | Verify `gh project view 14 --owner frankjhughes` |
| Duplicate issues created | Stamps not committed after a previous run | Commit stamps, or delete duplicates and re-run with `--clean` |
| Milestone has fewer issues than expected | Some story YAMLs are missing from the manifest | Check the `.stories` array in the sprint YAML |
| Verification fails on board count | Board items from prior sprints skew the count | This is expected — the check confirms items exist, not an exact match |

---

## Post-Bootstrap Checklist

After a successful bootstrap:

1. **Commit the stamps** — the scripts modified YAML files on disk.
   Create a PR to persist the `.issue_number`, `.created`, and
   `.board_synced` stamps to `main`.
2. **Verify the board UI** — open the project board in GitHub and
   confirm all issues appear in the correct columns.
3. **Announce the sprint** — the board is ready for work.