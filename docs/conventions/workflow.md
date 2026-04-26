# Workflow Conventions

## TDD Discipline

Work follows a red–green–refactor cycle:

1. Start with a failing test (red).
2. Make the test pass with the simplest possible change (green).
3. Refactor while keeping tests green (refactor).

This discipline applies across all layers: Api, Application, Domain, Infrastructure, and Frontend.  
New behavior should be driven by tests, not by ad‑hoc manual verification.

## Story Grammar

Stories use this grammar:

As a `<role>`, I must or should be able to `<verb>` so that `<value>`.

Stories must clearly express:

- The role (who).
- The capability (what).
- The business value (why).

Technical details belong in tasks, not in the story sentence.

## Debugging Discipline

Bug reports are treated as debugging sessions, not feature requests.

- Restate the problem clearly.
- Identify the smallest reproducible surface (failing test, endpoint, method).
- Prefer reasoning from test failures or debugger output instead of guessing.
- Avoid architectural changes until the problem is well understood.
- Minimize blast radius with local, reversible changes.
- Be explicit when inference is required; request missing signals instead of assuming them.

This discipline keeps debugging stable, predictable, and low‑churn.

## Universal Patch Rule

When a patch is required, regenerate and return the **entire file** with the patch already applied.  
This applies to all file types, including code and documentation.

- No partial edits.
- No diffs.
- No search‑and‑replace instructions.

Full file regeneration:

- Prevents corruption.
- Eliminates ambiguity about where changes belong.
- Aligns with script‑first and debugging discipline conventions.

## Script‑First And Fencing Rules

Scripts are the primary way to create or update files.

- Use PowerShell as the primary shell.
- Scripts must be copy‑pasteable.
- Generated files must use `utf8NoBOM` encoding.

Fencing and quoting rules:

- Avoid nested triple‑backtick fences in Markdown.
- When inner fences are required, use outer fences with four backticks.
- PowerShell here‑strings for PR bodies must be single‑quoted.
- Avoid literal double‑quote characters inside here‑strings.
- Avoid the sequences `@'` and `'@` inside here‑strings.
- Avoid backticks inside here‑strings.
- Avoid nested here‑strings.
- When documentation requires showing tricky syntax, describe it in words instead of using literal characters.

When generating files via scripts:

- Write content using single‑quoted here‑strings.
- Avoid fenced code blocks inside here‑strings.
- Prefer script‑driven, idempotent file generation.

## Makefile Conventions

Make targets are stack‑scoped:

- `backend-*`
- `frontend-*`
- `infra-*`

Bare targets are aggregates:

- `restore`
- `build`
- `test`
- `clean`
- `dev`

Each bare target orchestrates the corresponding stack‑scoped targets.

## Changelog Conventions

The changelog:

- Uses an `[Unreleased]` section for in‑progress work.
- References **issue numbers**, not PR numbers.
- Includes only user‑facing or architectural changes.

Internal refactors without user impact generally do not need entries.

## Tooling Conventions

- Primary shell: PowerShell.
- Scripts must be copy‑pasteable without manual fixes.
- Encoding for generated files: `utf8NoBOM`.

Tooling and automation must respect the fencing and script‑first rules.

## Identity Resolution Workflow

Endpoints never accept identity from the request body.  
Identity is resolved via `ICurrentUserService`.

- Application defines the abstraction.
- Infrastructure provides a concrete implementation.
- Api tests provide a test implementation.

This keeps identity resolution consistent, testable, and decoupled from transport details.
