# Developer Actions Index

> Central catalog of all custom GitHub Actions maintained in this repository.

## Overview

Custom actions live under `.github/actions/` and encapsulate reusable logic
that is shared across one or more workflows. Each action has its own directory
containing an `action.yml` manifest and a `README.md` that follows the
[Action README Template](action-readme-template.md).

## Actions

| Action | Path | Purpose | Primary Workflow |
|--------|------|---------|-----------------|
| [postgres-uri-to-npgsql-connection-string](../../../.github/actions/postgres-uri-to-npgsql-connection-string/README.md) | `.github/actions/postgres-uri-to-npgsql-connection-string/` | Converts a PostgreSQL URI to an Npgsql-compatible connection string for .NET applications. | `pr-preview.yml` |
| [wait-for-endpoint](../../../.github/actions/wait-for-endpoint/README.md) | `.github/actions/wait-for-endpoint/` | Polls an HTTP endpoint until it returns a healthy response or a timeout is reached. | `pr-preview.yml` |

## Conventions

1. **One action per directory** — each action occupies its own folder directly
   under `.github/actions/`.
2. **Composite actions preferred** — use composite run steps (`using:
   composite`) with Bash unless the action requires Node.js or Docker.
3. **README required** — every action must include a `README.md` that follows
   the [Action README Template](action-readme-template.md).
4. **Minimal surface area** — actions should do one thing well. If an action
   grows beyond a single responsibility, split it.
5. **Pin dependencies** — when an action calls other actions, pin them to a
   full SHA, not a mutable tag.

## Adding a New Action

1. Create a new directory under `.github/actions/` named after the action
   (lowercase, hyphen-separated).
2. Add an `action.yml` file defining the action's inputs, outputs, and
   run steps.
3. Copy the [Action README Template](action-readme-template.md) into the
   directory as `README.md` and fill in all sections.
4. Add a row to the **Actions** table above.
5. If the action is consumed by a workflow documented in
   [Workflow Conventions](../../conventions/workflow.md), update that document
   to reference the new action.

## Related Documentation

- [Action README Template](action-readme-template.md) — canonical format for
  action documentation.
- [Workflow Conventions](../../conventions/workflow.md) — how workflows are
  structured and how actions fit into them.
- [Developer Guide](developer-guide.md) — onboarding guide and quick links
  for contributors.
