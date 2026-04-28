---
id: US-139
title: "Frontend Hosting & Deployment"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-056
  - US-103
  - US-015
---

# US-139: Frontend Hosting & Deployment

## Intent

As a **system operator**, I need the Next.js frontend deployed to a free hosting
platform with automatic deployments from GitHub so that every merge to main
produces a live, publicly accessible site.

## Value

The frontend exists only on localhost until this story ships. Hosting is the
bridge between development and real users. Automatic deployment from GitHub
ensures every merge is live within minutes — no manual publish step, no drift
between code and production.

## Acceptance Criteria

- [ ] Next.js frontend is deployed to a free-tier hosting platform
- [ ] Every push to `main` triggers an automatic build and deploy (CI/CD)
- [ ] Pull requests generate preview deployments for review before merge
- [ ] Environment variables (API URL, feature flags) are configured per environment
- [ ] Build succeeds with the current Next.js App Router configuration
- [ ] Deployed site is accessible via the platform's default URL (custom domain is US-142)
- [ ] Free SSL/TLS certificate is provisioned automatically
- [ ] Build failures surface clearly in GitHub (status checks or notifications)
- [ ] Rollback to a previous deployment is possible without code changes
- [ ] Deployment completes in under 5 minutes for a typical build

## Emotional Guarantees

- **EG-01 No Surprises** — Every merge produces a predictable, inspectable deployment
- **EG-03 Calm Protection** — HTTPS is enforced automatically; no manual certificate management

## Platform Options (free tier)

| Platform | Bandwidth | Builds | Custom Domain | SSR Support | Best For |
|----------|-----------|--------|---------------|-------------|----------|
| **Vercel** | 100 GB/mo | Unlimited | Yes + free SSL | Native Next.js | Best DX, native Next.js support |
| **Cloudflare Pages** | Unlimited | 500/mo | Yes + free SSL | Via Workers | Best bandwidth, edge performance |
| **Netlify** | ~30 GB (credit) | 300 min/mo | Yes + free SSL | With adapter | Good Git workflow integration |

> **Recommendation:** Vercel — it's built by the Next.js team, zero-config
> deployment, native SSR/ISR support, and the free Hobby plan covers a portfolio
> project comfortably.

## Notes

- Dependencies are shipped: US-056 (Next.js scaffold), US-103 (frontend testing), US-015 (CI baseline)
- Vercel Hobby plan: 1 seat, 100 GB bandwidth, free custom domain + SSL
- If Vercel is selected, connect the GitHub repo and Vercel auto-detects Next.js — literally one click
- Platform decision should be recorded as an ADR
- **Demo:** Push a commit to main — watch the deployment go live within minutes, visit the URL
