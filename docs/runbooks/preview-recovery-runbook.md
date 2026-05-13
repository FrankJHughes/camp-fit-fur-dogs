# Preview Environment Recovery Runbook

Manual procedures for fixing broken preview environments.

## Destroy stale preview

1. Delete Neon branch.
2. Delete Vercel preview deployment.
3. Remove \ender-preview\ label from API service.

## Rebuild preview

Trigger a new push to the PR branch.

This will:

- Create a new Neon branch
- Deploy a fresh API
- Deploy a fresh frontend
- Run integration tests

## Validate

- API responds at \/api/health\
- Frontend loads
- DB migrations applied