---
id: US-194
title: "Authentication Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-110
  - US-111
  - US-197
  - US-198
---

# US‑194 — Authentication Observability

## Intent
As an **admin**, I must observe authentication behavior so that I can diagnose login failures, identity provider issues, and authentication‑related outages.

As a **developer**, I must have visibility into authentication flows so that I can debug OIDC interactions, token validation, and session establishment.

## Value
Authentication is a critical boundary. Failures here block all protected functionality.  
Without observability, diagnosing authentication issues requires guesswork, log scraping, or reproducing flows manually.

## Acceptance Criteria
- [ ] AC‑1: Emit `[auth.login.start]` when an authentication attempt begins
- [ ] AC‑2: Emit `[auth.login.redirect]` when redirecting to the identity provider
- [ ] AC‑3: Emit `[auth.login.callback.received]` when the OIDC callback is processed
- [ ] AC‑4: Emit `[auth.login.success]` with correlation ID and subject identifier
- [ ] AC‑5: Emit `[auth.login.failed]` with sanitized error reason
- [ ] AC‑6: Emit `[auth.session.established]` when a session token is issued
- [ ] AC‑7: Emit `[auth.session.revoked]` when a session is terminated
- [ ] AC‑8: All events include correlation ID and authentication method
- [ ] AC‑9: Test harness can assert authentication observability

## Emotional Guarantees
- **EG‑03 Calm Protection** — Authentication failures feel safe, predictable, and diagnosable.
- **EG‑05 Clear Boundaries** — Authentication behavior is transparent and consistent.

## Notes
- This story covers authentication flow observability, not authorization decisions.
- API boundary observability for authentication endpoints is covered in US‑195.
