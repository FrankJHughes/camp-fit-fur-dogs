# Action Name

> One‑sentence summary of what this action does and why it exists.

## Overview

Explain the purpose of the action, when it should be used, and what problem it solves.  
Keep this section high‑level and focused on intent.

## Inputs

| Name | Required | Description |
|------|----------|-------------|
| `example_input` | yes/no | What this input controls and how it affects behavior. |

Describe any constraints, defaults, or formatting rules for inputs.

## Outputs

| Name | Description |
|------|-------------|
| `example_output` | What the output represents and how workflows should consume it. |

If the action has no outputs, explicitly state that.

## Usage

Minimal example showing how to call the action:

```yaml
- name: Example usage
  uses: ./.github/actions/<action-name>
  with:
    example_input: value
```

If the action is commonly used in a specific workflow (CI, preview, etc.), include a second example showing real‑world usage.

## Behavior

Describe what the action does step‑by‑step:

- What commands it runs  
- How it handles errors  
- How it determines success/failure  
- Any retries, timeouts, or polling behavior  
- Any assumptions about environment variables or tools  

This section should be precise and implementation‑aligned.

## Failure Modes

Document all ways the action can fail:

- Invalid inputs  
- Missing environment variables  
- External service failures  
- Non‑zero exit codes  
- Timeouts  
- Unexpected HTTP responses (if applicable)  

For each failure mode, describe:

- What the workflow will see  
- How developers should diagnose it  
- How to fix it  

## Security Considerations

Document any sensitive behavior:

- Inputs that contain secrets  
- Outputs that must not be logged  
- Whether the action touches credentials  
- Whether artifacts contain sensitive data  

If the action is safe and handles no secrets, explicitly state that.

## Testing

Describe how to test the action locally or in CI:

- Required environment variables  
- How to run it with `act` (if applicable)  
- How to simulate failure conditions  
- How to validate outputs  

If the action is used in a workflow with integration tests, link to them.

## Related Documentation

- [Actions Index](actions-index.md)  
- [Workflow Conventions](../../conventions/workflow.md)  
- [Developer Guide](developer-guide.md)  

