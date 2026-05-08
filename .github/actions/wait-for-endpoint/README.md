# `wait-for-endpoint`

> Polls an HTTP endpoint until it returns a healthy response or a timeout is reached.

## Purpose

Deployed preview environments and freshly started services are not immediately
available after the deployment step completes. Subsequent workflow steps —
integration tests, smoke tests, Lighthouse audits — will fail if they run
before the endpoint is ready. This action eliminates race conditions by polling
the target URL at a configurable interval and only succeeding once the endpoint
responds with the expected status code.

## Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `url` | Yes | — | The HTTP(S) URL to poll. |
| `timeout` | No | `300` | Maximum time to wait in seconds before the action fails. |
| `interval` | No | `5` | Seconds to wait between poll attempts. |
| `expected-status` | No | `200` | HTTP status code that indicates the endpoint is healthy. |
| `follow-redirects` | No | `true` | Whether to follow HTTP 3xx redirects when polling. |

## Outputs

| Name | Description |
|------|-------------|
| `elapsed` | Total seconds elapsed before the endpoint became healthy. |
| `attempts` | Number of poll attempts made before success. |

## Usage

```yaml
- uses: ./.github/actions/wait-for-endpoint
  with:
    url: https://pr-${{ github.event.number }}.preview.example.com
    timeout: 120
    interval: 10

- name: Run smoke tests
  run: npm run test:smoke
  env:
    BASE_URL: https://pr-${{ github.event.number }}.preview.example.com
```

## Behavior

1. Reads the `url`, `timeout`, `interval`, `expected-status`, and
   `follow-redirects` inputs.
2. Begins polling the URL using `curl` with the configured redirect behavior.
3. On each attempt:
   - If the response status matches `expected-status`, the action writes the
     `elapsed` and `attempts` outputs and exits successfully.
   - If the response status does not match, or the request fails (connection
     refused, DNS error, TLS error), the action logs the attempt number and
     response details, then sleeps for `interval` seconds.
4. If `timeout` seconds elapse without a healthy response, the action fails
   with a summary of the last observed status and total attempts made.

## Error Handling

- **Missing `url`** — the action fails immediately with
  `Error: url is required`.
- **Timeout exceeded** — the action fails with
  `Error: endpoint did not return {expected-status} within {timeout}s
  (last status: {last-status}, attempts: {attempts})`.
- **DNS resolution failure** — logged per attempt; the action continues
  retrying until the timeout is reached, since DNS propagation may be in
  progress.
- **TLS certificate errors** — treated as a failed attempt and retried. The
  action does not bypass certificate validation.

## Dependencies

- `bash` — the action runs as a composite action using a Bash shell step.
- `curl` — must be available on the runner. Pre-installed on all
  GitHub-hosted runner images.

## Workflow Integration

- **`pr-preview.yml`** — used during the **health-check** stage after the
  preview environment deployment completes and before any tests or audits
  execute against the preview URL.

## Maintainer Notes

- The default timeout of 300 seconds (5 minutes) is generous. Most preview
  environments become healthy within 60–90 seconds. Consider setting a
  tighter timeout per workflow to surface deployment failures faster.
- The action intentionally does not validate the response body. If body
  validation is needed in the future, a `expected-body-contains` input
  could be added.
- See the [Action README Template](../../../docs/guides/developer/action-readme-template.md)
  for the canonical documentation format.
