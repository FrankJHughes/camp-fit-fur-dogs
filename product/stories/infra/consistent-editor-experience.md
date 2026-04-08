# Consistent Editor Experience

**Issue:** #59
**Layer:** L3 – Editor Configuration (Diamond Model, ADR-003)

## User Story

As a developer opening the repo in VS Code, I want the editor to
automatically apply formatting rules, recommend essential extensions,
provide debug launch profiles, and wire build/test tasks — so that my
editor is productive from the first keystroke without manual
configuration.

## Acceptance Criteria

- [ ] `.editorconfig` enforces portable formatting rules across all
      editors (indent style, charset, line endings, C# conventions).
- [ ] `.vscode/extensions.json` prompts new developers to install
      recommended extensions (C# Dev Kit, Docker, EditorConfig).
- [ ] `.vscode/settings.json` enables format-on-save and complements
      `.editorconfig` with VS Code-specific behaviors.
- [ ] `.vscode/launch.json` provides a one-click debug profile (F5)
      for the API project.
- [ ] `.vscode/tasks.json` wires Ctrl+Shift+B to build and provides
      a test task.

## Dependencies

None — this layer is independent of all other Diamond Model layers.

## Decision Record

See [ADR-0008](../../../docs/adr/0008-consistent-editor-experience.md).