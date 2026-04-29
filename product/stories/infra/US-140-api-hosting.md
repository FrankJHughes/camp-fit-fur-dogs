---
id: US-140
title: "API Hosting & Deployment"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-056
  - US-015
---

# US-140: API Hosting & Deployment

## Intent

As a **system operator**, I need the ASP.NET Core API deployed to a free hosting
platform with automatic deployments from GitHub so that the frontend can reach
a live API without depending on localhost.

## Value

The API is the backend engine for every feature — registration, login, dog
management, and future services. Until it's hosted, the frontend is a shell
with no data. Free-tier hosting with CI/CD means every merge is live and
testable without manual intervention.

## Acceptance Criteria

- [ ] ASP.NET Core API is deployed to a free-tier hosting platform
- [ ] Every push to `main` triggers an automatic build and deploy
- [ ] API is accessible via HTTPS at the platform's default URL (custom domain is US-142)
- [ ] Environment variables and connection strings are configured securely (not in source)
- [ ] Health check endpoint (`/health`) responds correctly on the deployed instance
- [ ] API startup time is acceptable for the chosen platform (document cold-start behavior)
- [ ] Deployment uses a Docker container or platform-native .NET support
- [ ] Build failures surface clearly in GitHub
- [ ] Rollback to a previous deployment is possible
- [ ] CORS is configured to allow requests from the frontend hosting URL
- [ ] Connection string is stored securely in the API hosting platform's environment variables

## Emotional Guarantees

- **EG-01 No Surprises** — API availability is predictable; cold-start behavior is documented
- **EG-03 Calm Protection** — Secrets are never in source code; HTTPS is enforced

## Platform Options (free tier)

| Platform | Type | Custom Domain | Cold Start | .NET Support | Best For |
|----------|------|---------------|------------|--------------|----------|
| **Azure App Service F1** | Always-on (limited) | Paid plans only | None | Native | Best .NET support, no cold start |
| **Render** | Spin-down after idle | Yes + free SSL | ~30-60s | Docker | Best free custom domain |
| **Fly.io** | 3 free VMs | Yes + free SSL | None (always on) | Docker | Best always-on free tier |
| **Railway** | $5/mo credit | Yes + free SSL | None | Docker | Best DX, PostgreSQL add-on |

> **Recommendation:** Render or Fly.io. Render is simplest (Docker push and go,
> free custom domain, free SSL). Fly.io keeps the API warm (no cold start) with
> 3 free VMs. Azure App Service F1 is native .NET but lacks custom domain on the
> free tier — workable if custom domain points only to the frontend and the API
> is accessed via its platform URL.

## Notes

- Dependencies are shipped: US-056 (API project), US-015 (CI baseline)
- The existing Dockerfile (or a new one) must build the API for the target platform
- Cold-start behavior matters for login/registration UX — document and communicate
- Connection string to the hosted database (US-141) must be configured as an env var
- Platform decision should be recorded as an ADR
- **Demo:** Push a commit to main — watch the API deploy, hit `/health` and `/swagger` on the live URL
