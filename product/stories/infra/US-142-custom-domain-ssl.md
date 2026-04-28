---
id: US-142
title: "Custom Domain & SSL"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-139
  - US-140
---

# US-142: Custom Domain & SSL

## Intent

As an **owner**, I should be able to reach Camp Fit Fur Dogs at
campfitfurdogs.com with a secure HTTPS connection so that the site looks
professional and trustworthy.

## Value

A custom domain is the public identity of the business. Without it, owners see
a platform-generated URL (e.g., camp-fit-fur-dogs.vercel.app) that looks
temporary and unprofessional. HTTPS is table stakes for trust, SEO, and browser
security warnings.

## Acceptance Criteria

### DNS Configuration
- [ ] DNS records for campfitfurdogs.com are configured at the registrar (Spaceship)
- [ ] `campfitfurdogs.com` and `www.campfitfurdogs.com` resolve to the frontend hosting platform
- [ ] API subdomain (e.g., `api.campfitfurdogs.com`) resolves to the API hosting platform (if supported on free tier)
- [ ] DNS propagation is verified across multiple regions
- [ ] If the API platform does not support custom domains on its free tier, the frontend proxies API requests (Next.js rewrites)

### SSL/TLS
- [ ] Free SSL/TLS certificates are provisioned and auto-renewed for all custom domains
- [ ] HTTPS is enforced — HTTP requests redirect to HTTPS
- [ ] HSTS header is configured (ties into US-134 Security Headers)

### Verification
- [ ] `https://campfitfurdogs.com` loads the frontend with a valid certificate
- [ ] `https://www.campfitfurdogs.com` redirects to (or mirrors) the apex domain
- [ ] API requests from the frontend reach the hosted API successfully
- [ ] No mixed-content warnings in the browser console

## Emotional Guarantees

- **EG-01 No Surprises** — The URL owners see is clean and professional
- **EG-03 Calm Protection** — The padlock icon is always present; no security warnings

## Domain Details

- **Domain:** campfitfurdogs.com
- **Registrar:** Spaceship
- **Purchased:** April 22, 2026
- **Renewal:** April 22, 2027
- **DNS Management:** Spaceship (or transfer to Cloudflare for free DNS management)

## Notes

- Depends on US-139 (frontend hosting) and US-140 (API hosting) — domain can't point anywhere until hosting exists
- Most free hosting platforms (Vercel, Render, Fly.io, Cloudflare Pages) provide free SSL via Let's Encrypt
- If the API host does not support custom domains on its free tier, use Next.js API route rewrites to proxy requests through the frontend domain
- Consider transferring DNS management to Cloudflare (free) for faster propagation, DDoS protection, and caching
- **Demo:** Open a browser, navigate to https://campfitfurdogs.com — see the live site with a valid SSL certificate
