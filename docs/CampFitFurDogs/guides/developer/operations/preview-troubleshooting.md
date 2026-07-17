# Preview Troubleshooting Guide

> How to diagnose and resolve failures in PR Preview environments  
> (Neon branch → migrations → infrastructure tests → frontend preview → Render API preview → API tests)

This guide covers the most common failure modes in the PR Preview pipeline and how to resolve them quickly.  
It reflects the current CI/CD architecture, Render + Neon hosting model, and all updated governance rules.

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
- CI logs show: `Error: failed to create branch`
- Neon API returns 4xx or 5xx errors
- `db-conn.txt` artifact missing
- Migrations never run

### Likely Causes
- Neon API key missing or invalid  
- Neon rate limiting  
- Branch name collision  
- Neon service outage  

### How to Fix
- Verify Neon API key in GitHub Secrets  
- Re-run CI (Neon outages are often transient)  
- Check Neon dashboard for existing `pr-<number>` branch  
- Manually delete stuck branches and re-run CI  

---

# 2. Connection String Conversion Failures

### Symptoms
- CI logs show: `postgres-uri-to-npgsql-connection-string: failed`
- EF Core migrations fail immediately
- Connection string is empty or malformed

### Likely Causes
- Neon returned unexpected URI format  
- Missing `postgres://` prefix  
- Special characters not URL‑encoded  

### How to Fix
- Inspect Neon branch details  
- Ensure URI begins with `postgres://`  
- Re-run CI  

---

# 3. Migration Failures

### Symptoms
- EF Core migration errors in CI  
- `dotnet ef database update` fails  
- Infrastructure tests never run  
- API preview never deploys  

### Likely Causes
- Breaking schema change  
- Missing migration  
- Migration ordering issue  
- Model drift between branches  

### How to Fix
- Run migrations locally against a fresh Neon branch  
- Add missing migrations  
- Fix ordering issues  
- Re-run CI  

---

# 4. Infrastructure Integration Test Failures

### Symptoms
- Failures in `CampFitFurDogs.Infrastructure.IntegrationTests`
- DB connection artifact exists
- Frontend + API previews never deploy

### Likely Causes
- Tests assume existing data  
- Tests assume schema state  
- Tests not updated after domain changes  
- Tests rely on external services  

### How to Fix
- Ensure tests create their own data  
- Ensure tests do not apply migrations  
- Update tests to match domain rules  
- Re-run CI  

---

# 5. Vercel Frontend Preview Failures

### Symptoms
- `frontend-url.txt` artifact missing  
- Vercel CLI errors  
- Build fails during `vercel build`  
- Deployment fails during `vercel deploy`

### Likely Causes
- Missing or invalid Vercel token  
- Missing `.env.local` variables  
- Next.js build errors  
- Vercel rate limiting  

### How to Fix
- Verify Vercel PAT in GitHub Secrets  
- Check `.env.local` generation step  
- Run `npm run build` locally  
- Re-run CI  

---

# 6. Render API Preview Fails to Start

### Symptoms
- CI stuck waiting for readiness  
- `wait-for-endpoint` repeatedly fails  
- `/api/dogs` never returns 200/400/401  
- Render dashboard shows no preview instance  

### Likely Causes
- `render-preview` label missing  
- Render PR Preview misconfigured  
- API container failed to build  
- Missing environment variables  
- Cold start exceeded timeout  

### How to Fix
- Ensure `render-preview` label is present  
- Check Render build logs  
- Verify environment variables in Render  
- Re-run CI (cold starts can take several minutes)  

---

# 7. Render Teardown Fails

### Symptoms
- CI stuck waiting for teardown  
- `/health` never returns 404  
- Old preview still visible in Render dashboard  

### Likely Causes
- Render delayed teardown  
- Label removal not processed  
- Render API propagation delay  

### How to Fix
- Manually delete preview in Render  
- Re-run CI  
- Check Render status page  

---

# 8. API Integration Test Failures

### Symptoms
- Failures in `CampFitFurDogs.Api.IntegrationTests`
- API preview is running, but tests fail
- Responses differ from expected values

### Likely Causes
- API behavior changed  
- Tests assume specific seed data  
- Tests assume specific error codes  
- Tests not updated after domain changes  

### How to Fix
- Run tests locally against the preview URL  
- Update tests to match current API behavior  
- Re-run CI  

---

# 9. Missing Artifacts

### Symptoms
- `db-conn.txt` missing  
- `frontend-url.txt` missing  
- CI fails early  

### Likely Causes
- Upstream job failed  
- Artifact upload skipped  
- Workflow concurrency canceled a run  

### How to Fix
- Inspect upstream job logs  
- Re-run CI  
- Ensure concurrency group is not blocking  

---

# 10. When in Doubt: How to Debug Quickly

### Checklist
- Confirm **Neon branch** exists  
- Confirm **db-conn.txt** artifact  
- Confirm **frontend-url.txt** artifact  
- Confirm **Render preview** exists  
- Hit `${API_BASE_URL}/api/health`  
- Hit `${API_BASE_URL}/api/dogs`  
- Open Vercel preview URL  
- Re-run CI  
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
