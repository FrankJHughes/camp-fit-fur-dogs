# Copilot Instructions

This file provides top level guidance for how copilot should behave in this repository.
All detailed conventions live in separate files under docs/conventions.

## Purpose

Copilot must follow the established architecture, workflow, coding, and documentation rules defined in the conventions folder.
This file defines how copilot interprets and applies those rules, how it generates files, and how it avoids corruption or drift.

## Conventions index

All conventions are defined in the following files:

- docs/conventions/architecture.md
- docs/conventions/workflow.md
- docs/conventions/code.md
- docs/conventions/docs.md

These four files are the single source of truth for all repository rules.

## How copilot must use the conventions

- Copilot must always consult the conventions files before generating or modifying code, documentation, or scripts.
- Copilot must not invent new patterns or workflows that contradict the conventions.
- When conventions appear to conflict, copilot must ask for clarification rather than guessing.
- When generating files, copilot must follow the rules defined in the conventions, including script generation rules, quoting rules, workflow rules, and architectural boundaries.
- Guardrail tests exist to ensure copilot aligns with established rules.

### Hosting & Deployment Guidance

Copilot must follow the hosting and deployment conventions defined in:

- `docs/conventions/architecture.md` (Hosting & Deployment Architecture)
- `docs/conventions/workflow.md` (API Deployment Workflow)

These documents define:
- API hosting platform (Render)
- Database hosting platform (Neon)
- Environment variable requirements
- Health check conventions
- CORS requirements
- Deployment triggers and expectations

Copilot must not propose alternative hosting platforms or deployment models unless explicitly requested by the user.

## Lessons learned

| Number | Sprint | Lesson | Mitigation |
|--------|--------|--------|------------|
| 1 | 3 | Accidental direct push to main | Added branch protection rule and local pre push hook |
| 2 | 3 | git update index chmod fails on hooks directory | Removed from docs and moved hooks into tracked hooks directory with core dot hooksPath |
| 3 | 3 | Changelog had pr numbers instead of issue numbers | Added standing rule to always use issue numbers |
| 4 | 3 | gh pr create body bypasses pr template | Added standing rule to embed merge checklist manually |
| 5 | 4 | CRLF line endings broke pre push hook shebang on Windows | Added gitattributes lf enforcement and editorconfig |
| 6 | 4 | Power shell double quoted here strings corrupted backticks in pr bodies | Added standing rule to use single quoted here strings for pr bodies |
| 7 | 4 | Makefile targets only covered backend | Scoped all targets by stack and established naming conventions for up and down targets |
| 8 | 5 | Version drift across csproj files caused dependency conflicts | Introduced central package management with transitive pinning |
| 9 | 5 | Guardrail tests mixed reflection and di dependent tests | Split into architecture dot tests and api dot tests/guardrails with routing rules |
| 10 | 6 | Documentation duplicated across multiple files causing drift | Established canonical ownership map and single navigation hub |
| 11 | 7 | Quote characters inside here strings caused corruption | Added quote safety rules and fencing conventions |
| 12 | 8 | Manual copy paste of generated file content caused corruption | Added script first file generation rule |
| 13 | 9 | Di tests required a separate assembly due to speculative debugging | Added debugging discipline rule requiring deliberate reasoning before proposing architectural changes |
| 14 | 10 | Partial file edits and patching attempts caused corruption and drift across code and documentation | Added universal patch rule requiring full file regeneration with patches already applied for all file types |
| 15 | 6 | PowerShell treats `[id]` in Next.js dynamic route folder names as a wildcard character class, causing silent file-operation failures | Use `-LiteralPath` in all PowerShell file operations involving Next.js dynamic route folders (e.g., `Get-Content -LiteralPath`, `Set-Content -LiteralPath`). Never use `-Path` with bracket characters. |
| 16 | 6 | PR body used a custom format instead of the repo PR template (Summary with Closes #, Changes, Merge Checklist) | Always follow the full PR template structure when using gh pr create body, not just the checklist |
| 17 | 6 | Changelog entry was missing for a user-facing change | Add a CHANGELOG.md entry under Unreleased for every user-facing change before opening the PR |
| 18 | 11 | Cross-aggregate frontend files placed under `components/shared/` or `lib/shared/` — violated the convention that `lib/` is the shared infrastructure home. | Never create `shared/` subfolders under aggregate-grouped layers. Cross-aggregate components → `lib/components/`; cross-aggregate types → directly under `lib/`. |
| 19 | 11 | New test directories under `test/lib/` silently matched the unit Vitest project (`environment: 'node'`), causing `document is not defined` for hooks and silent skipping of `.tsx` component tests. | When adding test directories under `test/lib/`, verify which Vitest project include pattern matches. Update `vitest.config.ts` includes/excludes so React and hook tests run in jsdom. |
| 20 | 11 | PowerShell `.Replace()` matched every occurrence of a markdown heading, causing double insertion in a doc file. | Use `.IndexOf()` + `.Insert()` for single-occurrence text insertions. `.Replace()` is global by default. |
| 21 | 11 | `gh pr create --body` bypasses `.github/PULL_REQUEST_TEMPLATE.md` entirely — the template never loads, so the PR body silently drifts from the expected structure. | Before drafting `--body` content, `cat .github/PULL_REQUEST_TEMPLATE.md` and follow its exact headings and checklist. |
