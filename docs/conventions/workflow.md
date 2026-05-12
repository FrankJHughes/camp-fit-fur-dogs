````markdown
# Workflow Conventions

Canonical standards for GitHub Actions workflows in this repository.  
These conventions reflect the actual behavior of `ci.yaml` and `preview.yaml`.

## Related Documents

- `architecture.md` — hosting, layering, preview architecture  
- `Code.md` — preview‑safe rules, script‑first patterns  
- `Docs.md` — documentation structure  
- `adr/` — decision history for CI/CD and automation

---

# Overview

All CI/CD automation lives under `.github/workflows/`.

Two primary pipelines:

1. **Build & Test (`ci.yaml`)**  
2. **Preview Environments (`preview.yaml`)**

Both workflows emphasize determinism, minimal permissions, and reproducibility.

---

# 1. Build & Test Workflow (`ci.yaml`)

## Purpose

Ensures:

- Correct dependency graph  
- Targeted test execution  
- Full test coverage on `main` and nightly runs  
- Deterministic execution of backend, frontend, and SharedKernel tests

---

## Triggers

- Pushes to `main`  
- PRs targeting `main`  
- Manual dispatch  
- Nightly schedule (`0 9 * * *`)

---

## Job Structure

### `determine-changes`

Uses `dorny/paths-filter` to compute:

- `backend`  
- `frontend`  
- `shared-kernel`  
- `infra`  
- `docs-only`

These outputs drive conditional execution.

---

### `validate-ci-deps`

Runs `scripts/ci/validate-ci-dependencies.mjs`.  
Ensures workflow dependency graph remains correct.

---

### `shared-kernel`

Runs when:

- On `main`  
- SharedKernel changed  
- Infra changed

Uses composite action `run-dotnet-tests`.

Projects:

- `tests/SharedKernel.Tests`  
- `tests/SharedKernel.Api.Tests`  
- `tests/SharedKernel.Infrastructure.EntityFrameworkCore.Tests`

---

### `backend`

Runs when:

- On `main`  
- Backend changed  
- SharedKernel changed  
- Infra changed

Projects:

- `tests/CampFitFurDogs.Api.Tests`  
- `tests/CampFitFurDogs.Application.Tests`  
- `tests/CampFitFurDogs.Domain.Tests`  
- `tests/CampFitFurDogs.Infrastructure.Tests`

---

### `frontend`

Runs when:

- On `main`  
- Frontend changed  
- Backend changed  
- Infra changed

Steps:

- Checkout  
- Setup Node  
- `npm ci`  
- `npm test`

---

## CI Conventions

- Path‑based test selection  
- Dependency validation  
- Composite actions for .NET tests  
- Explicit job ordering  
- Fail‑fast behavior  
- Nightly full runs  
- Script‑first logic (no complex inline shell)

---

# 2. Preview Environment Workflow (`preview.yaml`)

## Purpose

Provisions a **full ephemeral environment per PR**, including:

- Neon DB branch  
- Render API deployment  
- Vercel frontend deployment  
- Health checks  
- Integration tests  
- Artifact publication

Also destroys stale preview resources.

---

## Triggers

- `opened`  
- `reopened`  
- `synchronize`  
- `closed`

---

## Concurrency

```yaml
group: preview-pr-<number>
cancel-in-progress: true
```

---

## Job Structure

### `preview_context`

Computes:

- `common_prefix`  
- Artifact names  
- Neon branch name  
- API base URL  
- Flags: `should_destroy`, `should_deploy`

---

# Destroy Phase (conditional)

Executed when `should_destroy == true`.

### `destroy_stale_api`

- Remove `render-preview` label  
- Sleep 300 seconds  
- Confirm teardown via `wait-for-endpoint` (`/api/health` → `404`)

### `destroy_stale_db_artifact`  
### `destroy_stale_frontend_artifact`  
### `destroy_stale_db`

- Delete Neon branch

### `destroy_stale_frontend`

- Remove Vercel preview deployments

---

# Deploy Phase (conditional)

Executed when `should_deploy == true`.

### `deploy_fresh_frontend`

- Remove `.vercel`  
- `vercel pull`  
- Write `.env.local` with `NEXT_PUBLIC_API_URL`  
- `vercel build`  
- `vercel deploy --prebuilt --target=preview`  
- Publish `frontend-url.txt`

### `deploy_fresh_db`

- Create Neon branch  
- Convert URI → Npgsql  
- Apply EF Core migrations  
- Run infrastructure integration tests  
- Publish `db-conn.txt`

### `deploy_fresh_api`

- Add `render-preview` label  
- Poll `/api/dogs` until healthy  
- Run API integration tests

---

# Timeouts & Retries

**Teardown waits**

- Timeout: 600s  
- Consecutive successes: 5  
- Interval: 60s  

**Readiness waits**

- Timeout: 900s  
- Consecutive successes: 5  
- Interval: 60s  

These values must remain aligned with `architecture.md`.

---

# Artifacts

- `db-conn.txt` — Npgsql connection string  
- `frontend-url.txt` — Vercel preview URL  

Artifacts must be deterministic and sensitive.

---

# Composite Actions

| Action | Purpose |
|--------|---------|
| `postgres-uri-to-npgsql-connection-string` | Convert Neon URI → Npgsql |
| `wait-for-endpoint` | Poll API health |

These must remain stable and script‑first compatible.

---

# Preview Pipeline Flow

```text
PR opened / updated
  |
  v
preview_context
  |
  +--> destroy_stale_* (if should_destroy)
  |
  v
deploy_fresh_frontend (if should_deploy)
deploy_fresh_db       (if should_deploy)
  |
  v
deploy_fresh_api      (if should_deploy)
  |
  v
API integration tests
```

---

# General Workflow Conventions

### Composite actions first  
### No complex inline shell  
### Pin third‑party actions  
### Explicit permissions  
### Explicit timeouts  
### Deterministic artifacts  
### Reproducibility via scripts and documented env vars  

---

# Related Documentation

- `architecture.md`  
- `Code.md`  
- `Docs.md`  
- ADR index under `adr/`
````
