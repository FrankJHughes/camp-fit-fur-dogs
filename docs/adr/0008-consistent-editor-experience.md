# ADR-0008: Consistent Editor Experience

**Status:** Accepted
**Date:** 2026-04-07

## Context

Without shared editor configuration, developers make ad-hoc formatting
choices — tabs vs. spaces, inconsistent line endings, missing
extensions, manual debug setup. These differences create noisy diffs,
slow onboarding, and preventable friction. The Diamond Model (ADR-003)
defines **L3 – Editor Configuration** as the layer that standardizes
the editor experience across the team.

## Decision

Commit a portable `.editorconfig` at the repository root and a
`.vscode/` directory with four configuration files:

| File | Purpose |
|------|---------|
| `.editorconfig` | Editor-agnostic formatting: indent style, charset, line endings, C# conventions |
| `.vscode/extensions.json` | Recommended extensions: C# Dev Kit, Docker, EditorConfig |
| `.vscode/settings.json` | VS Code behaviors: format-on-save, file nesting, default solution |
| `.vscode/launch.json` | Debug profile: F5 launches the API with Development environment |
| `.vscode/tasks.json` | Editor tasks: Ctrl+Shift+B builds, test task, watch task |

### Key Design Choices

- **`.editorconfig` as the foundation:** It is supported by every
  major editor and IDE. Language-specific overrides (Makefile tabs,
  markdown trailing whitespace, shell script LF endings) prevent
  accidental corruption.
- **Recommended, not required, extensions:** `extensions.json` uses
  the `recommendations` array, which prompts developers to install
  but does not force them. This respects editor customization while
  ensuring discoverability.
- **`dotnet` commands in tasks, not `make`:** Tasks use `dotnet`
  directly so they work on Windows without `make` — consistent with
  the `bootstrap.ps1` approach from ADR-0007.
- **`serverReadyAction` in launch config:** Automatically opens the
  browser when the API starts listening, eliminating the manual step
  of copying the URL.

## Alternatives Considered

| Option | Pros | Cons |
|--------|------|------|
| **`.editorconfig` + `.vscode/`** | Portable formatting + VS Code productivity | Two layers to maintain |
| **`.editorconfig` only** | Single file, universal | No debug, no tasks, no extension prompts |
| **`.vscode/` only** | Full VS Code experience | Excludes Rider, Visual Studio, vim users |
| **No editor config** | Zero maintenance | Noisy diffs, slow onboarding, inconsistent style |

## Consequences

- All editors enforce consistent formatting from the first file save.
- VS Code prompts for essential extensions on first open.
- F5 launches the API with zero manual configuration.
- Ctrl+Shift+B builds the solution; test task runs all tests.
- Future extensions (linters, formatters, debugger profiles) are
  added to these files and propagate to all developers automatically.