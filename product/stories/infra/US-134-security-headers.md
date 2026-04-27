---
id: US-134
title: "Security Headers"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies: []
---

# US-134: Security Headers

## Intent

As a **system operator**, I need the application to serve standard HTTP security
headers on every response so that common browser-based attacks are mitigated by
default.

## Value

Security headers are low-effort, high-impact hardening. They instruct browsers
to enforce HTTPS, prevent clickjacking, block MIME-type sniffing, and limit
script execution. Without them, the application is vulnerable to a class of
attacks that browsers can prevent automatically.

## Acceptance Criteria

- [ ] `Strict-Transport-Security` (HSTS) enforces HTTPS with a minimum max-age
- [ ] `Content-Security-Policy` (CSP) restricts script, style, and frame sources to trusted origins
- [ ] `X-Content-Type-Options: nosniff` prevents MIME-type sniffing
- [ ] `X-Frame-Options: DENY` prevents clickjacking via iframes
- [ ] `Referrer-Policy` limits referrer information leakage
- [ ] `Permissions-Policy` disables unused browser features (camera, microphone, geolocation)
- [ ] `X-XSS-Protection: 0` (deprecated but set explicitly to avoid legacy browser quirks)
- [ ] Headers are applied via middleware, not per-endpoint
- [ ] CSP policy does not break legitimate application functionality (Next.js inline scripts, styles)
- [ ] Integration test verifies all headers are present on responses
- [ ] Security scanner (e.g., securityheaders.com) scores A or higher

## Emotional Guarantees

- **EG-03 Calm Protection** — Owners are protected from browser-based attacks without needing to know or do anything

## Notes

- No dependencies — can ship in any sprint
- CSP requires careful tuning for Next.js — inline scripts and styles may need nonce-based exceptions
- Consider using a library like `NetEscapades.AspNetCore.SecurityHeaders` for configuration
- **Demo:** Run the deployed site through securityheaders.com — show the A+ rating
