# Workflow Conventions

> Standards and structure for GitHub Actions workflows in this repository.

## Overview

All CI/CD automation is defined as GitHub Actions workflows stored in
`.github/workflows/`. Workflows orchestrate jobs and steps that build, test,
deploy, and validate the project on every push and pull request.

## Workflow Structure

Each workflow file should follow this general layout:

1. **Trigger block** — define the events that start the workflow (`on:`).
2. **Permissions block** — declare the minimum `GITHUB_TOKEN` permissions
   required.
3. **Jobs** — group related steps into logically named jobs.
4. **Steps** — within each job, order steps so that setup precedes action
   precedes validation.

## Naming Conventions

- Workflow files: lowercase, hyphen-separated (e.g., `pr-preview.yml`).
- Job IDs: lowercase, hyphen-separated, descriptive (e.g., `deploy-preview`).
- Step names: sentence case, imperative mood (e.g., `Build application`,
  `Run integration tests`).

## Custom GitHub Actions in the PR Preview Workflow

The PR Preview workflow (`pr-preview.yml`) uses custom composite actions to
encapsulate complex or reusable logic that would otherwise clutter the workflow
file. These actions live under `.github/actions/` and are invoked with
`uses: ./.github/actions/{action-name}`.

### Role of Custom Actions

Custom actions serve three purposes within the PR Preview pipeline:

1. **Abstraction** — they hide implementation details (URI parsing, polling
   loops) behind a clean inputs/outputs interface, keeping the workflow
   readable.
2. **Reusability** — the same action can be called from multiple workflows
   or multiple jobs within a single workflow without duplicating shell
   scripts.
3. **Testability** — because each action is self-contained, it can be tested
   in isolation with a dedicated test workflow.

### Actions Used in PR Preview

| Stage | Action | Purpose |
|-------|--------|---------|
| Database provisioning | [`postgres-uri-to-npgsql-connection-string`](../../.github/actions/postgres-uri-to-npgsql-connection-string/README.md) | Converts the Neon branch URI into an Npgsql connection string consumed by the .NET application container. |
| Health check | [`wait-for-endpoint`](../../.github/actions/wait-for-endpoint/README.md) | Polls the deployed preview URL until it returns HTTP 200, gating all downstream test and audit steps. |

### Pipeline Flow

```text
PR opened / updated
  |
  v
Build application
  |
  v
Provision database branch
  |
  v
postgres-uri-to-npgsql-connection-string   <-- custom action
  |
  v
Deploy preview environment
  |
  v
wait-for-endpoint                           <-- custom action
  |
  v
Run tests and audits
  |
  v
Post results to PR
```

### Adding a New Action to the Pipeline

1. Create the action directory and files under `.github/actions/`.
2. Document it using the
   [Action README Template](../guides/developer/action-readme-template.md).
3. Register it in the
   [Actions Index](../guides/developer/actions-index.md).
4. Add a row to the **Actions Used in PR Preview** table above if the action
   participates in the PR Preview pipeline.
5. Update the **Pipeline Flow** diagram if the action introduces a new stage.

## General Conventions

- **Prefer composite actions over inline scripts** — if a block of shell
  logic exceeds ~10 lines or is used in more than one place, extract it into
  a custom action.
- **Pin action versions** — reference third-party actions by full commit SHA,
  not mutable tags.
- **Minimize permissions** — request only the `GITHUB_TOKEN` scopes the
  workflow actually needs.
- **Fail fast** — set `fail-fast: true` on matrix strategies unless partial
  results are valuable.
- **Use `timeout-minutes`** — set explicit timeouts on jobs to prevent
  runaway builds.

## Related Documentation

- [Actions Index](../guides/developer/actions-index.md) — catalog of all
  custom actions.
- [Action README Template](../guides/developer/action-readme-template.md) —
  canonical format for action documentation.
- [Developer Guide](../guides/developer/developer-guide.md) — onboarding
  guide and quick links.
