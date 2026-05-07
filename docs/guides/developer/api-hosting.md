# API Hosting Guide

**Path:** `docs/guides/developer/api-hosting.md`

This guide documents how the Camp Fit Fur Dogs API is hosted, deployed, tested, and previewed using **Render** and **Neon**, and how developers can work with these environments safely and effectively.

---

## Overview

- **API platform:** Render (Dockerized .NET 10 Web Service)  
- **Database platform:** Neon (PostgreSQL)  
- **Preview orchestration:** Render PR Previews in **Manual** mode  
- **CI orchestration:** GitHub Actions manages DB provisioning, migrations, tests, and preview lifecycle  
- **Deployment model:**  
  - `main` → automatic Render deploy  
  - PRs → preview deploy controlled by the `render-preview` label  

This guide explains the full lifecycle, local workflows, troubleshooting, and operational expectations.

---

## Render Hosting Model

### Main Branch Deployments

- Render is configured for **Git-backed automatic deploys** on push to `main`.  
- No GitHub Actions workflow triggers deployment.  
- Render builds the Dockerfile at:  
  `src/CampFitFurDogs.Api/Dockerfile`

### PR Preview Deployments (Manual Mode)

Render PR Previews are configured in **Manual** mode:

- **Label added** → Render creates a preview instance  
- **Label removed** → Render destroys the preview instance  
- **PR synchronize** → Render redeploys *only if the label is present*

This label-driven model gives CI full control over preview lifecycle.

### Preview URL Format

```
https://campfitfurdogsapi-pr-<number>.onrender.com
```

---

## Neon Database Hosting

### Production

- Persistent Neon branch  
- Connection string stored in Render environment variables  
- SSL required

### PR Preview Branches

- CI creates a fresh Neon branch named:  
  `pr-<number>`
- Branch expires automatically after 14 days  
- Branch is deleted when the PR closes  
- EF Core migrations are applied in CI before the API preview is deployed  
- Connection string is converted from PostgreSQL URI → Npgsql format using the repo’s composite action

---

## CI Preview Lifecycle (Canonical Sequence)

The GitHub Actions workflow orchestrates the entire preview lifecycle:

### 1. Destroy stale preview

- Remove `render-preview` label  
- Wait for old preview to be destroyed  
- Teardown probe:  
  - Endpoint: `/health`  
  - Expected: `404`  
  - **3 consecutive** successes  
  - Timeout: **300 seconds**  
  - Poll: **5 seconds**

### 2. Provision fresh Neon branch

- Create branch `pr-<number>`  
- Set expiration (14 days)  
- Convert Neon URI → Npgsql connection string  
- Apply EF Core migrations  
- Run Infrastructure Integration Tests  
- Upload `db-conn.txt` artifact

### 3. Deploy fresh API preview

- Add `render-preview` label  
- Render builds and deploys automatically  
- Startup probe:  
  - Endpoint: `/api/dogs`  
  - Healthy: `200, 400, 401`  
  - **3 consecutive** successes  
  - Timeout: **300 seconds**

### 4. Run API Integration Tests

- Tests run against the live preview URL  
- Validate routing, middleware, auth, and end‑to‑end flows

### 5. Cleanup

- On PR close:  
  - Neon branch deleted  
  - Render preview destroyed (label removed)

---

## Environment Variables and Secrets

### Where secrets live

- **GitHub Secrets** — Neon API key, workflow secrets  
- **Render Environment** — production and preview runtime variables

### Important variables

- `ConnectionStrings__DefaultConnection`  
- `ASPNETCORE_ENVIRONMENT`  
- `Frontend__BaseUrl`  
- `PREVIEW_DB_CONNECTION_STRING` (used in docs and Render preview config)

### Security rules

- Never print secrets in logs  
- Treat artifacts containing connection strings as sensitive  
- Do not commit `.env` files or local secrets

---

## Local Development

### Running API against a local PostgreSQL instance

```powershell
docker run --rm `
  -e POSTGRES_PASSWORD=pass `
  -e POSTGRES_USER=dev `
  -e POSTGRES_DB=campfitfurdogs `
  -p 5432:5432 postgres:15
```

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=campfitfurdogs;Username=dev;Password=pass;"
dotnet run --project src/CampFitFurDogs.Api
```

### Running API against a Neon preview branch

- Download `db-conn.txt` from CI  
- Set `ConnectionStrings__DefaultConnection` to its contents  
- Run migrations and tests locally

---

## Troubleshooting Guide

### Preview never tears down

**Symptom:** CI times out waiting for `/health` → `404`  
**Fix:**  
- Ensure label was removed  
- Check Render dashboard  
- Manually delete preview instance if stuck

### Migrations fail

**Symptom:** EF Core migration errors  
**Fix:**  
- Inspect `db-conn.txt`  
- Reproduce locally using the same connection string  
- Fix migration and re-run CI

### API never becomes ready

**Symptom:** `/api/dogs` never stabilizes  
**Fix:**  
- Check Render logs (build + runtime)  
- Verify DB connection string  
- Confirm migrations succeeded

### CI-only failures

**Fix:**  
- Reproduce locally using `db-conn.txt`  
- Match CI environment variables  
- Run failing test suite locally

---

## Operational Checklist for PR Authors

1. Open PR  
2. Ensure migrations compile  
3. Add `render-preview` label to deploy preview  
4. Remove + re-add label to rebuild preview  
5. Use `db-conn.txt` to debug DB issues  
6. Preview is destroyed automatically when PR closes

---

## Reproducing CI Failures Locally

```powershell
$cs = Get-Content .\db-conn.txt -Raw
$env:ConnectionStrings__DefaultConnection = $cs

dotnet ef database update `
  -s src/CampFitFurDogs.Api `
  -p src/CampFitFurDogs.Infrastructure

dotnet test integration-tests/CampFitFurDogs.Infrastructure.IntegrationTests `
  --logger "trx"
```

---

## FAQ

**Why does CI use `/api/dogs` for readiness?**  
It exercises routing, middleware, and auth — a stronger signal than `/health`.

**Why does teardown expect `404`?**  
Render returns `404` once the preview instance is fully destroyed.

**How long does a preview take to warm up?**  
Up to 300 seconds due to cold starts and route propagation.

---

## References

- Render dashboard  
- Neon dashboard  
- `.github/workflows/preview.yml`  
- Composite actions:  
  - `postgres-uri-to-npgsql-connection-string`  
  - `wait-for-endpoint`

---

## Change History

- **2026‑05‑05** — Initial version created

