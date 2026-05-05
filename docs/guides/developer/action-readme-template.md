# `{action-name}`

> One-line description of what this action does.

## Purpose

Explain the problem this action solves and why it exists as a reusable action
rather than inline workflow steps.

## Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `input-name` | Yes | — | What this input controls. |
| `optional-input` | No | `default-value` | What this optional input controls. |

## Outputs

| Name | Description |
|------|-------------|
| `output-name` | What this output contains. |

## Usage

```yaml
- uses: ./.github/actions/{action-name}
  with:
    input-name: value
```

## Behavior

Describe the runtime behavior step by step:

1. What the action does first.
2. What happens next.
3. What the action produces or modifies.

## Error Handling

Describe failure modes:

- **Missing required input** — action fails with a descriptive error message.
- **Invalid value** — describe what happens and what the user should check.

## Dependencies

List any tools, runtimes, or packages the action requires on the runner:

- Runtime or tool (e.g., `bash`, `node20`, `python3`)
- Any external binaries or packages

## Workflow Integration

Reference which workflow(s) consume this action and at what stage:

- **`workflow-name.yml`** — used during the `{stage}` stage to accomplish `{goal}`.

## Maintainer Notes

- Any caveats, known limitations, or future improvement plans.
- Link to related issues or ADRs if applicable.
