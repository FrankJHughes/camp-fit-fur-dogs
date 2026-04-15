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
- Local pre-push hook (`hooks/pre-push`) provides a second safety layer.
- Hooks are activated via `core.hooksPath hooks/` — **not** copied into `.git/hooks/`.
- The devcontainer sets this automatically (`postCreateCommand`). For local setup:

```powershell
git config core.hooksPath hooks/
```

### Line Endings

CRLF line endings break shebang lines in git hooks and shell scripts on Windows. Two layers enforce correct endings:

- **`.gitattributes`** — repo-level, enforced for everyone:
  - `hooks/*`, `*.sh`, `*.mjs`, `*.mts` → LF (Unix scripts)
  - `*.cmd`, `*.bat` → CRLF (Windows batch files)
  - Binary files (`*.png`, `*.jpg`, etc.) → no conversion
- **`.editorconfig`** — editor-level, catches new files:
  - Default `end_of_line = lf` for all files
  - VS Code respects `.editorconfig` natively

> **If a hook fails with `cannot exec`**, the most likely cause is CRLF line endings. Fix with:
> ```powershell
> (Get-Content hooks/pre-push -Raw) -replace "`r`n", "`n" | Set-Content -Path hooks/pre-push -NoNewline -Encoding utf8NoBOM
> ```

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
- Use **single-quoted here-strings** (`@'...'@`) in PowerShell when the PR body contains backticks. Double-quoted here-strings (`@"..."@`) interpret backticks as escape characters, corrupting inline code.

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

## Frontend

### Technology

- **Framework:** React with Next.js (ADR-0012 pending — US-055)
- **Runtime:** Node 22 (installed via devcontainer feature)

### Project Layout

```
frontend/
├── package.json          # Dependencies and scripts
├── tsconfig.json         # TypeScript config
├── vitest.config.mts     # Test runner config
├── next.config.mjs       # Next.js config
├── src/                  # Application source code only
│   └── app/
└── test/                 # Test files — mirrors src/ structure
    └── app/
```

Configs live at `frontend/`. Source in `frontend/src/`. Tests in `frontend/test/`. This layout ensures `node_modules` is an ancestor of both `src/` and `test/` for correct module resolution.

### Testing Stack

- **Test runner:** Vitest 3 + jsdom environment
- **Assertions:** React Testing Library + jest-dom (`toBeInTheDocument`, etc.)
- **Path aliases:** `vite-tsconfig-paths` resolves `@/*` aliases in tests

```bash
cd frontend
npm test -- --run
```

---

## Tooling

- **Primary shell:** PowerShell on Windows. Provide all scripts in PowerShell.
- **Git Bash (MINGW64):** Available but not preferred.
- **IDE:** Scripts and commands should be copy-pasteable from conversation into terminal.
### Makefile

Targets are scoped by stack. Bare names are aggregates that run both.

| Target | Purpose |
|---|---|
| `api-restore` | Restore NuGet packages |
| `api-build` | Build the .NET solution |
| `api-test` | Run backend tests |
| `api-clean` | Remove backend build artifacts |
| `api-up` | Run the API |
| `frontend-install` | Install frontend dependencies (`npm ci`) |
| `frontend-build` | Build the frontend |
| `frontend-test` | Run frontend tests |
| `frontend-lint` | Lint the frontend |
| `frontend-clean` | Remove `.next` and `node_modules` |
| `frontend-up` | Start the frontend dev server |
| `infra-up` / `infra-down` | Start / stop Docker containers |
| `restore` | `api-restore` + `frontend-install` |
| `build` | `api-build` + `frontend-build` |
| `test` | `api-test` + `frontend-test` |
| `clean` | `api-clean` + `frontend-clean` |
| `all` | Full pipeline: `restore` > `build` > `test` |
| `dev` | Start full stack (infra + API + frontend). Ctrl+C kills all. |
| `dev-down` | Stop Docker containers |

- **Editor config:** `.editorconfig` at repo root — LF line endings, UTF-8, consistent indentation. VS Code respects it natively.

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
| 3 | `git update-index --chmod=+x` fails on `.git/hooks/` files | Removed from docs — hooks now live in tracked `hooks/` dir with `core.hooksPath` |
| 3 | CHANGELOG had PR numbers instead of Issue numbers | Added standing rule to always use Issue numbers |
| 3 | `gh pr create --body` bypasses PR template | Added standing rule to always embed merge checklist manually |
| 4 | CRLF line endings broke pre-push hook shebang on Windows | Added `.gitattributes` LF enforcement for `hooks/*` and `*.sh`; added `.editorconfig` |
| 4 | PowerShell double-quoted here-strings corrupt backticks in PR bodies | Added standing rule to use single-quoted here-strings (`@'...'@`) for PR bodies |
| 4 | Makefile targets (`restore`, `build`, `test`, `clean`) only covered backend | Scoped all targets by stack (`api-*`, `frontend-*`). Bare names are aggregates. Naming convention: `*-up` / `*-down` for services. |
