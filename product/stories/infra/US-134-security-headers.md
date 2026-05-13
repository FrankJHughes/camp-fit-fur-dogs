---
id: US-134
title: "Security Headers"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
urgency: high
importance: high
covey_quadrant: Q1
emotional_guarantees:
legal_guarantees:
---

# US‑134: Security Headers

## Intent
As a **system operator**, I need the application to serve standard HTTP security headers on every response so that common browser‑based attacks are mitigated by default.

## Value
Security headers are low‑effort, high‑impact hardening. They instruct browsers to enforce HTTPS, prevent clickjacking, block MIME‑type sniffing, and limit script execution. Without them, the application is vulnerable to attacks that modern browsers can prevent automatically.

## Acceptance Criteria

### Required Headers
- `Strict-Transport-Security` (HSTS) enforces HTTPS with a minimum `max-age`.
- `Content-Security-Policy` (CSP) restricts script, style, and frame sources to trusted origins.
- `X-Content-Type-Options: nosniff` prevents MIME‑type sniffing.
- `X-Frame-Options: DENY` prevents clickjacking via iframes.
- `Referrer-Policy` limits referrer information leakage.
- `Permissions-Policy` disables unused browser features (camera, microphone, geolocation).
- `X-XSS-Protection: 0` is set explicitly to avoid legacy browser quirks.

### Behavior & Constraints
- Headers are applied via middleware, not per‑endpoint.
- CSP policy must not break legitimate application functionality (e.g., Next.js inline scripts, styles).
- All headers must be present on **every** response, including errors and redirects.
- Policies are environment‑specific and configurable.

### Observability & Testing
- Integration test verifies all headers are present on responses.
- Security scanner (e.g., securityheaders.com) scores **A or higher**.
- Logs do not include sensitive CSP or browser‑specific details.

## Emotional Guarantees
- **EG‑03 — Calm Protection**  
  Owners are protected from browser‑based attacks without needing to know or do anything.

## Notes
- No dependencies — can ship in any sprint.
- CSP requires careful tuning for Next.js; inline scripts may require nonce‑based exceptions.
- Consider using `NetEscapades.AspNetCore.SecurityHeaders` for configuration.
- **Demo:** Run the deployed site through securityheaders.com and show the A+ rating.
