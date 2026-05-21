# Copilot Instructions

This file provides top‑level guidance for how Copilot should behave in this repository.
All detailed conventions live in the `docs/conventions` folder.
All governance documents live in `docs/governance`.

Copilot must always follow the repository’s **[conventions](ca://s?q=Show_repository_conventions)**, **[governance](ca://s?q=Show_governance_rules)**, and **[guardrails](ca://s?q=Show_guardrail_tests)**.

---

# Purpose

Copilot must follow the established architecture, workflow, coding, documentation, and governance rules defined in this repository.
This file defines how Copilot interprets and applies those rules, how it generates files, and how it avoids corruption or drift.

Copilot’s behavior must always align with:

- the **[architectural boundaries](ca://s?q=Explain_architecture_boundaries)**
- the **[workflow and deployment model](ca://s?q=Explain_workflow_and_deployment_model)**
- the **[code and documentation conventions](ca://s?q=Explain_code_and_docs_conventions)**
- the **[script‑first and patch‑first rules](ca://s?q=Explain_script_first_and_patch_first_rules)**
- the **[guardrail tests](ca://s?q=List_guardrail_tests)** that enforce repository expectations
- the **[governance rules](ca://s?q=Explain_governance_rules)** that define process, responsibilities, and enforcement

---

# Conventions Index (Canonical)

All conventions are defined in the following files:

- `docs/conventions/architecture.md`
- `docs/conventions/workflow.md`
- `docs/conventions/code.md`
- `docs/conventions/docs.md`

These four files are the **single source of truth** for all repository conventions.
Copilot must treat them as canonical.

---

# Governance Index (Reference Only)

Governance defines **process**, **responsibilities**, **boundaries**, and **enforcement**.
Governance overrides conventions when they conflict.

Governance lives in:

- `docs/governance/product/`
- `docs/governance/technical/`
- `docs/governance/enforcement/`

Copilot must respect governance rules but must not duplicate or reinterpret them.

---

# How Copilot Must Use the Conventions

- Copilot must always consult the conventions files before generating or modifying code, documentation, or scripts.
- Copilot must not invent new patterns or workflows that contradict the conventions.
- When conventions appear to conflict, Copilot must ask for clarification rather than guessing.
- When generating files, Copilot must follow the rules defined in the conventions, including:
  - **[script‑first rules](ca://s?q=Explain_script_first_rules)**
  - **[quoting and fencing rules](ca://s?q=Explain_fencing_rules)**
  - **[workflow rules](ca://s?q=Explain_workflow_rules)**
  - **[architectural boundaries](ca://s?q=Explain_architecture_boundaries)**
  - **[TDD expectations](ca://s?q=Explain_TDD_expectations)**

- Guardrail tests exist to ensure Copilot aligns with established rules.
- Copilot must not modify governance files unless explicitly instructed.

---

# Hosting & Deployment Guidance

Copilot must follow the hosting and deployment conventions defined in:

- `docs/conventions/architecture.md` (Hosting & Deployment Architecture)
- `docs/conventions/workflow.md` (API Deployment Workflow and PR Preview Workflow)
- `docs/guides/developer/api-hosting.md` (Developer API Hosting Guide)

These documents define:

- API hosting platform: **Render**
- API service name: **`campfitfurdogsapi`**
- Database hosting platform: **Neon**
- PR Preview model: Git‑backed Render PR Previews + Neon ephemeral branches
- Required environment variables:
  - `ConnectionStrings__DefaultConnection`
  - `PREVIEW_DB_CONNECTION_STRING`
- Health check conventions (`/api/health`, `/api/dogs`)
- Deployment triggers and expectations

Copilot must not propose alternative hosting platforms or deployment models unless explicitly requested.

---

# File Generation Rules

Copilot must:

- Regenerate **entire files** when applying patches (**[Universal Patch Rule](ca://s?q=Explain_Universal_Patch_Rule)**)
- Use script‑first generation for any file creation or updates
- Respect quoting, fencing, and here‑string safety rules
- Avoid partial edits, inline diffs, or search‑and‑replace instructions
- Ensure generated files use `utf8NoBOM` encoding

These rules prevent corruption, drift, and ambiguity.

---

# Lessons Learned

| Number | Sprint | Lesson | Mitigation |
|--------|--------|--------|------------|
| 1 | 3 | Accidental direct push to main | Added branch protection rule and local pre‑push hook |
| 2 | 3 | `git update-index --chmod` fails on hooks directory | Moved hooks into tracked directory with `core.hooksPath` |
| 3 | 3 | Changelog used PR numbers instead of issue numbers | Always use issue numbers |
| 4 | 3 | `gh pr create` body bypasses PR template | Embed merge checklist manually |
| 5 | 4 | CRLF line endings broke pre‑push hook | Added `.gitattributes` LF enforcement |
| 6 | 4 | Double‑quoted here‑strings corrupted backticks | Use single‑quoted here‑strings |
| 7 | 4 | Makefile targets only covered backend | Added stack‑scoped naming conventions |
| 8 | 5 | Version drift across csproj files | Introduced Central Package Management |
| 9 | 5 | Guardrail tests mixed reflection + DI | Split into Architecture.Tests + Api.Tests/Guardrails |
| 10 | 6 | Documentation duplicated | Added canonical ownership map |
| 11 | 7 | Quote characters inside here‑strings caused corruption | Added quote‑safety rules |
| 12 | 8 | Manual copy‑paste corrupted generated files | Added script‑first rule |
| 13 | 9 | DI tests required separate assembly | Added debugging discipline rule |
| 14 | 10 | Partial file edits caused drift | Added Universal Patch Rule |
| 15 | 6 | PowerShell treated `[id]` as wildcard | Use `-LiteralPath` |
| 16 | 6 | PR body bypassed template | Always follow PR template |
| 17 | 6 | Missing changelog entry | Add under `[Unreleased]` |
| 18 | 11 | Shared frontend files placed under `components/shared/` | Use `lib/components/` instead |
| 19 | 11 | Vitest config matched wrong test dirs | Updated include/exclude patterns |
| 20 | 11 | `.Replace()` matched all headings | Use `.IndexOf()` + `.Insert()` |
| 21 | 11 | `gh pr create --body` bypassed template | Load template manually |
| 22 | 12 | Renaming Render service broke previews | Never rename Git‑backed preview services |
| 23 | 12 | Enabling previews after creation broke linkage | Enable previews at creation time |
| 24 | 12 | Workflow scraped GitHub Checks for URLs | Use deterministic preview URL patterns |
| 25 | 12 | Legacy image‑backed preview logic conflicted | Remove all image‑backed preview logic |
| 26 | 12 | Manual Render API calls conflicted | Never manually create preview instances |
| 27 | 12 | Preview URL detection failed on reopened PRs | Compute URLs deterministically |
| 28 | 12 | Misunderstanding of Render env var injection | Document Render’s environment variable rules |

