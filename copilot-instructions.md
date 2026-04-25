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
