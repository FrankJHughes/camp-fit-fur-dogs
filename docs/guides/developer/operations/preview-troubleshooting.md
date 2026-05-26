# Preview Troubleshooting Guide

> How to diagnose and resolve failures in PR Preview environments  
> (Neon branch → migrations → infrastructure tests → frontend preview → Render API preview → API tests)

This guide covers the most common failure modes in the PR Preview pipeline and how to resolve them quickly.

---

# Overview of the Preview Lifecycle

A PR Preview consists of:

1. **Neon preview branch** (`pr-<number>`)
2. **EF Core migrations** applied in CI
3. **Infrastructure integration tests** (against Neon)
4. **Vercel frontend preview**
5. **Render API preview**
6. **API integration tests** (against the live preview)

Failures can occur at any stage.  
This guide helps you identify *where* the failure happened and *what to do next*.

---

# 1. Neon Branch Creation Failures

### Symptoms

- CI logs show:  
  `Error: failed to create branch`  
- Neon API returns 4xx or 5xx errors  
- `db-conn.txt` artifact is missing  
- Migrations never run

### Likely Causes

- Neon API key missing or invalid  
- Neon rate limiting  
- Branch name collision (rare)  
- Neon service outage

### How to Fix

- Verify the Neon API key in GitHub Secrets  
- Re-run the workflow (Neon outages are often transient)  
- Check Neon dashboard for existing `pr-<number>` branch  
- If stuck, manually delete the branch in Neon and re-run CI

---

# 2. Connection String Conversion Failures

### Symptoms

- CI logs show:  
  `postgres-uri-to-npgsql-connection-string: failed`  
- EF Core migrations fail immediately  
- Connection string in logs is empty or malformed

### Likely Causes

- Neon returned an unexpected URI format  
- Missing `postgres://` prefix  
- URI contained special characters not URL‑encoded

### How to Fix

- Inspect the Neon branch details in the dashboard  
- Ensure the URI starts with `postgres://`  
- Re-run CI (conversion action is deterministic)

---

# 3. Migration Failures

### Symptoms

- CI logs show EF Core errors  
- `dotnet ef database update` fails  
- Infrastructure tests never run  
- API preview never starts

### Likely Causes

- Breaking schema change  
- Missing migration  
- Migration order issue  
- Model drift between branches

### How to Fix

- Run migrations locally against a fresh Neon branch  
- Add missing migrations  
- Fix ordering issues  
- Re-run CI after pushing fixes

---

# 4. Infrastructure Integration Test Failures

### Symptoms

- CI logs show test failures in:  
  `CampFitFurDogs.Infrastructure.IntegrationTests`  
- DB connection artifact exists  
- Frontend and API previews never deploy

### Likely Causes

- Tests assume existing data  
- Tests assume schema state  
- Tests not updated after domain changes  
- Tests rely on external services

### How to Fix

- Ensure tests create their own data  
- Ensure tests do not apply migrations  
- Update tests to match new domain rules  
- Re-run CI

---

# 5. Vercel Frontend Preview Failures

### Symptoms

- `frontend-url.txt` artifact missing  
- Vercel CLI errors in logs  
- Build fails during `vercel build`  
- Deployment fails during `vercel deploy`

### Likely Causes

- Missing or invalid Vercel token  
- Missing `.env.local` variables  
- Build errors in Next.js  
- Vercel rate limiting

### How to Fix

- Verify Vercel PAT in GitHub Secrets  
- Check `.env.local` generation step in CI logs  
- Run `npm run build` locally to reproduce  
- Re-run CI (Vercel outages are often transient)

---

# 6. Render API Preview Fails to Start

### Symptoms

- CI stuck waiting for readiness  
- `wait-for-endpoint` reports repeated failures  
- `/api/dogs` never returns 200/400/401  
- Render dashboard shows no preview instance

### Likely Causes

- `render-preview` label not added  
- Render PR Preview misconfigured  
- API container failed to build  
- Environment variables missing  
- Cold start exceeded timeout

### How to Fix

- Check PR labels — ensure `render-preview` is present  
- Check Render dashboard for build logs  
- Verify environment variables in Render service  
- Re-run CI (cold starts can take several minutes)

---

# 7. Render Teardown Fails

### Symptoms

- CI stuck waiting for teardown  
- `/api/health` never returns 404  
- Old preview instance still visible in Render dashboard

### Likely Causes

- Render delayed teardown  
- Label removal not processed yet  
- Render API propagation delay

### How to Fix

- Check Render dashboard — manually delete preview  
- Re-run CI  
- If persistent, check Render status page

---

# 8. API Integration Test Failures

### Symptoms

- CI logs show failures in:  
  `CampFitFurDogs.Api.IntegrationTests`  
- API preview is up, but tests fail  
- Responses differ from expected values

### Likely Causes

- API behavior changed  
- Tests assume specific seed data  
- Tests assume specific error codes  
- Tests not updated after domain changes

### How to Fix

- Run tests locally against the preview URL  
- Update tests to match new API behavior  
- Re-run CI

---

# 9. Missing Artifacts

### Symptoms

- `db-conn.txt` missing  
- `frontend-url.txt` missing  
- CI fails early

### Likely Causes

- Upstream job failed  
- Artifact upload step skipped  
- Workflow concurrency canceled a run

### How to Fix

- Inspect upstream job logs  
- Re-run CI  
- Ensure concurrency group is not blocking runs

---

# 10. When in Doubt: How to Debug Quickly

### Checklist

- Check **Neon branch** exists  
- Check **db-conn.txt** artifact  
- Check **frontend-url.txt** artifact  
- Check **Render preview** exists  
- Hit `${API_BASE_URL}/api/health`  
- Hit `${API_BASE_URL}/api/dogs`  
- Check Vercel preview URL manually  
- Re-run CI with “Re-run failed jobs”  
- Check Render + Neon status pages

### Fastest Manual Debug Path

1. Open Neon → confirm branch exists  
2. Open Render → confirm preview exists  
3. Open Vercel → confirm preview exists  
4. Hit the API preview URL directly  
5. Run migrations locally against the Neon branch  
6. Run integration tests locally against the preview URL

---

# Summary

This guide helps diagnose failures in:

- Neon branch creation  
- Connection string conversion  
- EF Core migrations  
- Infrastructure tests  
- Vercel frontend preview  
- Render API preview  
- API integration tests  
- Artifact generation  
- Teardown and readiness checks  

Use this document whenever a PR Preview fails to deploy or validate.

