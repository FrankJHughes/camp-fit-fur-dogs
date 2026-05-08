# Copilot Instructions

This file provides top‑level guidance for how Copilot should behave in this repository.
All detailed conventions live in the `docs/conventions` folder.

## Purpose

Copilot must follow the established architecture, workflow, coding, and documentation rules defined in the conventions folder.
This file defines how Copilot interprets and applies those rules, how it generates files, and how it avoids corruption or drift.

Copilot’s behavior must always align with:

- the architectural boundaries
- the workflow and deployment model
- the code and documentation conventions
- the script‑first and patch‑first rules
- the guardrail tests that enforce repository expectations

## Conventions Index

All conventions are defined in the following files:

- `docs/conventions/architecture.md`
- `docs/conventions/workflow.md`
- `docs/conventions/code.md`
- `docs/conventions/docs.md`

These four files are the **single source of truth** for all repository rules.
Copilot must treat them as canonical.

## How Copilot Must Use the Conventions

- Copilot must always consult the conventions files before generating or modifying code, documentation, or scripts.
- Copilot must not invent new patterns or workflows that contradict the conventions.
- When conventions appear to conflict, Copilot must ask for clarification rather than guessing.
- When generating files, Copilot must follow the rules defined in the conventions, including:
  - script‑first rules
  - quoting and fencing rules
  - workflow rules
  - architectural boundaries
  - test‑driven development expectations
- Guardrail tests exist to ensure Copilot aligns with established rules.

## Hosting & Deployment Guidance

Copilot must follow the hosting and deployment conventions defined in:

- `docs/conventions/architecture.md` (Hosting & Deployment Architecture)
- `docs/conventions/workflow.md` (API Deployment Workflow and PR Preview Workflow)
- `docs/guides/developer/api-hosting.md` (**Developer API Hosting Guide**)

These documents define:

- API hosting platform: **Render**
- API service name: **`campfitfurdogsapi`**
- Database hosting platform: **Neon**
- PR Preview model: Git‑backed Render PR Previews + Neon ephemeral branches
- Environment variable requirements, including:
  - `ConnectionStrings__DefaultConnection`
  - `PREVIEW_DB_CONNECTION_STRING` for PR previews
- Health check conventions (`/health`, `/api/dogs`)
- Deployment triggers and expectations

Copilot must not propose alternative hosting platforms or deployment models unless explicitly requested by the user.

## File Generation Rules

Copilot must:

- Regenerate **entire files** when applying patches (Universal Patch Rule)
- Use script‑first generation for any file creation or updates
- Respect quoting, fencing, and here‑string safety rules
- Avoid partial edits, inline diffs, or search‑and‑replace instructions
- Ensure generated files use `utf8NoBOM` encoding

These rules prevent corruption, drift, and ambiguity.

## Lessons Learned

| Number | Sprint | Lesson | Mitigation |
|--------|--------|--------|------------|
| 1 | 3 | Accidental direct push to main | Added branch protection rule and local pre‑push hook |
| 2 | 3 | `git update-index --chmod` fails on hooks directory | Removed from docs and moved hooks into tracked hooks directory with `core.hooksPath` |
| 3 | 3 | Changelog used PR numbers instead of issue numbers | Added standing rule to always use issue numbers |
| 4 | 3 | `gh pr create` body bypasses PR template | Added standing rule to embed merge checklist manually |
| 5 | 4 | CRLF line endings broke pre‑push hook shebang on Windows | Added `.gitattributes` LF enforcement and `.editorconfig` |
| 6 | 4 | PowerShell double‑quoted here‑strings corrupted backticks | Added standing rule to use single‑quoted here‑strings for PR bodies |
| 7 | 4 | Makefile targets only covered backend | Scoped all targets by stack and established naming conventions |
| 8 | 5 | Version drift across csproj files caused dependency conflicts | Introduced Central Package Management with transitive pinning |
| 9 | 5 | Guardrail tests mixed reflection and DI‑dependent tests | Split into `Architecture.Tests` and `Api.Tests/Guardrails` with routing rules |
| 10 | 6 | Documentation duplicated across multiple files | Established canonical ownership map and single navigation hub |
| 11 | 7 | Quote characters inside here‑strings caused corruption | Added quote‑safety rules and fencing conventions |
| 12 | 8 | Manual copy‑paste of generated file content caused corruption | Added script‑first file generation rule |
| 13 | 9 | DI tests required a separate assembly due to speculative debugging | Added debugging discipline rule requiring deliberate reasoning |
| 14 | 10 | Partial file edits caused corruption and drift | Added Universal Patch Rule requiring full file regeneration |
| 15 | 6 | PowerShell treats `[id]` in Next.js route folders as wildcard | Use `-LiteralPath` for all file operations involving bracketed paths |
| 16 | 6 | PR body used custom format instead of repo template | Always follow `.github/PULL_REQUEST_TEMPLATE.md` exactly |
| 17 | 6 | Missing changelog entry for user‑facing change | Add entry under `[Unreleased]` before opening PR |
| 18 | 11 | Cross‑aggregate frontend files placed under `components/shared/` | Never create `shared/` under aggregate layers; use `lib/components/` |
| 19 | 11 | Test directories under `test/lib/` matched wrong Vitest project | Update `vitest.config.ts` include/exclude patterns |
| 20 | 11 | PowerShell `.Replace()` matched all headings | Use `.IndexOf()` + `.Insert()` for single‑occurrence insertions |
| 21 | 11 | `gh pr create --body` bypasses PR template | Always load and follow the PR template before drafting body |
| 22 | 12 | Renaming a Render service broke PR Preview GitHub linkage | Never rename a Git‑backed Render service with PR Previews enabled; recreate the service if renaming is required |
| 23 | 12 | Enabling PR Previews after service creation prevented GitHub notifications | Enable PR Previews during initial service creation or recreate the service |
| 24 | 12 | Workflow scraped GitHub Checks for preview URLs, but Render PR Previews do not create checks | Use deterministic preview URL patterns instead of scraping GitHub metadata |
| 25 | 12 | Legacy image‑backed preview logic conflicted with Git‑backed PR Previews | Remove all image‑backed preview logic when switching to Git‑backed PR Previews |
| 26 | 12 | Manual Render API calls conflicted with native PR Preview behavior | Never manually create preview instances for Git‑backed services |
| 27 | 12 | Preview URL detection failed because reopened PRs do not receive new comments | Compute preview URLs deterministically instead of relying on PR comments |
| 28 | 12 | Misunderstanding of Render environment variable injection | Document that Render only reads variables available in the Render build environment and injected via `previewValue` |
