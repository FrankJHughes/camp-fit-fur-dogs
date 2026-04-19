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

## Story Grammar (Required)

All backlog stories **must** begin with the following structure:

**As a <role>, I must/should be able to <verb>… so that <value>…**

Valid roles:
- Owner
- Staff
- Admin

Invalid roles:
- System
- API
- Backend
- Infrastructure
- Database

Stories must always reflect a **human actor**, never a technical component.

### TDD Discipline

- **Red-green-refactor on every layer**: Domain → Application → Infrastructure → API.
- Always write the failing test first (red), then the minimum production code to pass (green). Never skip the red step.
- Never deliver production code without a preceding test.

### Backlog and Sprint Process

- **Story files are the backlog.** Stories live in `product/stories/` and are the single source of truth.
- **GitHub Issues are only created when a story is pulled into a sprint.** No pre-created issues for uncommitted work.
- Sprint board: GitHub Projects board #14.

### Repo Content Lookup

- **Always look in the public GitHub repo first** (browse the website or use raw URLs) before asking the developer to paste content.
- The repo is public at `frankjhughes/camp-fit-fur-dogs` — there is no reason to block on the developer for file contents that are already available.

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

### Identity Resolution

Endpoints never accept user identity (e.g., `OwnerId`) from the request body. Identity is resolved server-side via `ICurrentUserService`.

- **Abstraction:** `ICurrentUserService` lives in the Application layer (`Application/Abstractions/`).
- **Production:** `DummyCurrentUserService` (Infrastructure) returns a hardcoded placeholder. Replace with a real implementation when authentication lands — the replacement will need `Scoped` lifetime (reads from `HttpContext`).
- **Tests:** `TestCurrentUserService` (Api.Tests) exposes a settable `CurrentUserId`. The test factory overrides the DI registration so tests control which user the endpoint sees.
- **Pattern:** Endpoint binds a DTO (e.g., `RegisterDogRequest`), injects `ICurrentUserService`, and constructs the command by combining both.

### Password Hashing

- **Algorithm:** BCrypt via `BCrypt.Net-Next` (default work factor 11).
- **Location:** `PasswordHash` value object in the Domain layer — BCrypt is pure computation, not an infrastructure concern.
- **Two entry points:**
  - `PasswordHash.Create(plaintext)` — write path (registration, password change). Hashes and returns a new value object.
  - `PasswordHash.Verify(plaintext)` — read path (login). Compares plaintext against the stored hash.
  - `PasswordHash.From(hashedValue)` — rehydration path (EF Core loading from DB). Wraps an existing hash string.
- **Never store plaintext or reversible encodings** (base64, hex) in the database.

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
| `backend-restore` | Restore NuGet packages |
| `backend-build` | Build the .NET solution |
| `backend-test` | Run backend tests |
| `backend-clean` | Remove backend build artifacts |
| `backend-up` | Run the API |
| `frontend-install` | Install frontend dependencies (`npm ci`) |
| `frontend-build` | Build the frontend |
| `frontend-test` | Run frontend tests |
| `frontend-lint` | Lint the frontend |
| `frontend-clean` | Remove `.next` and `node_modules` |
| `frontend-up` | Start the frontend dev server |
| `infra-up` / `infra-down` | Start / stop Docker containers |
| `restore` | `backend-restore` + `frontend-install` |
| `build` | `backend-build` + `frontend-build` |
| `test` | `backend-test` + `frontend-test` |
| `clean` | `backend-clean` + `frontend-clean` |
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
| 4 | Makefile targets (`restore`, `build`, `test`, `clean`) only covered backend | Scoped all targets by stack (`backend-*`, `frontend-*`). Bare names are aggregates. `backend-up` includes `infra-up`. Naming convention: `*-up` / `*-down` for services. |
| 5 | Version drift across 11 csproj files caused MSB3277 (EF Core Relational conflict) and silent dependency mismatches | Introduced Central Package Management (`Directory.Packages.props`). New packages must be added there — not in csproj. `CentralPackageTransitivePinningEnabled` prevents transitive conflicts. |
| 5 | Guardrail tests mixed pure-reflection and DI-dependent tests in one project, slowing CI feedback | Split into `Architecture.Tests` (pure reflection, no host) and `Api.Tests/Guardrails/` (DI-dependent via `GuardrailTestBase`). Routing rule: if it doesn't need `Get<T>()`, it goes in Architecture.Tests. |
| 6 | Documentation duplicated across 4–6 files (source control rules, DoR, DoD, milestones, conventions, nav hubs) — updates drifted and contradicted each other | Established canonical ownership map: each topic has exactly one authoritative file, all others link to it. Single nav hub in `docs/README.md`. Debloated ~540 lines with zero information loss. |


