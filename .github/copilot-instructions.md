# Conventions

Living document — version-controlled rules, preferences, and lessons learned for this project. Update via PR like any other change. GitHub Copilot reads this file automatically.

---

## Maintaining This Document

Three triggers, zero new ceremonies:

1. **Convention-changing PRs** — when a PR introduces or changes a convention, the same PR updates this file. Not a follow-up, not a separate story. Same diff, same review.
2. **Sprint closing** — the sprint review template includes a checklist item to review this document for staleness. Lessons learned from the retrospective become rows in the table below.
3. **"We got burned" moments** — when something breaks mid-sprint because a convention wasn't documented, the fix PR also adds the lesson. Don't wait for the retro.

**Ownership rule:** whoever introduces the change owns the update. PR reviewers should flag convention changes that lack a corresponding update here.

---

## Standing Rules

### TDD Discipline

- **Red-green-refactor on every layer**: Domain → Application → Infrastructure → API.
- Always write the failing test first (red), then the minimum production code to pass (green). Never skip the red step.
- Never deliver production code without a preceding test.

### Backlog and Sprint Process

- **Story files are the backlog.** Stories live in `product/stories/` and are the single source of truth.
- **GitHub Issues are only created when a story is pulled into a sprint.** No pre-created issues for uncommitted work.
- Sprint board: GitHub Projects board #14.

---

## Source Control

### Branch Protection

- `main` requires a pull request — no direct pushes.
- Approval requirement is disabled (solo repo — can't approve your own PR).
- Local pre-push hook provides a second safety layer. Install with:

```powershell
Copy-Item hooks/pre-push .git/hooks/pre-push
```

> **Note:** Do not run `git update-index --chmod=+x` on `.git/hooks/` files — that command only works on tracked files. Git on Windows executes hooks via the shebang regardless of the executable bit.

### Branching Model

- Feature branches off `main`: `feat/`, `fix/`, `docs/`, `backlog/`
- Squash-merge PRs into `main`
- Delete the remote branch after merge; clean up local with `git branch -d <branch>`

### Commit Style

Conventional commits:

| Prefix     | Use                                    |
|------------|----------------------------------------|
| `feat:`    | New feature or vertical slice          |
| `fix:`     | Bug fix                                |
| `docs:`    | Documentation only                     |
| `backlog:` | Adding or updating backlog story files |
| `test:`    | Test-only changes                      |
| `chore:`   | Tooling, CI, config                    |

---

## Pull Request Conventions

- **Always include `Closes #<issue-number>`** in the PR body when an issue exists.
- **Always include the 6-item merge checklist** from `.github/PULL_REQUEST_TEMPLATE.md` — `gh pr create --body` bypasses the template entirely, so embed it manually.
- Use **four-backtick outer fences** whenever a code block contains inner code fences. Fence collisions are a recurring issue in PowerShell scripts that embed markdown PR bodies.

### Merge Checklist

```
- [ ] PR description is complete and linked to an issue
- [ ] CI (`Build & Test`) is passing
- [ ] Self-review completed
- [ ] Docs updated (if applicable)
- [ ] Changelog updated under Unreleased (if user-facing)
- [ ] No secrets or credentials committed
```

---

## Architecture

### DDD Layered Architecture (ADR-0002)

Four layers with strict dependency direction:

```
API → Application → Domain
         ↓
   Infrastructure
```

### CQRS Pipelines

Writes and reads are separated at the application layer:

**Command (write) pipeline:**
`ICommand` → `ICommandHandler` → `ICommandDispatcher`

**Query (read) pipeline:**
`IQuery<TResponse>` → `IQueryHandler` → `IQueryDispatcher`

> See ADR-0011 (pending — US-051) for full rationale.

### Endpoint Routing

Consolidated into `Endpoints.cs` using the `MapGroup` pattern.

---

## Tooling

- **Primary shell:** PowerShell on Windows. Provide all scripts in PowerShell.
- **Git Bash (MINGW64):** Available but not preferred.
- **IDE:** Scripts and commands should be copy-pasteable from conversation into terminal.

---

## CHANGELOG

- Use `[Unreleased]` for in-progress work; promote to `[Sprint N] — YYYY-MM-DD` at sprint close.
- **Always reference the GitHub Issue number, not the PR number.** This has caused errors in past sprints.
- Only add entries for user-facing or architecturally significant changes — backlog stories don't need entries.

---

## Lessons Learned

| Sprint | Lesson | Mitigation |
|--------|--------|------------|
| 3 | Accidental direct push to `main` | Added branch protection rule + local pre-push hook |
| 3 | `git update-index --chmod=+x` fails on `.git/hooks/` | Removed from docs — copy alone is sufficient |
| 3 | CHANGELOG had PR numbers instead of Issue numbers | Added standing rule to always use Issue numbers |
| 3 | `gh pr create --body` bypasses PR template | Added standing rule to always embed merge checklist manually |
