---
id: US-195
title: "API Authentication Boundary Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-194
  - US-197
  - US-198
---

# US‑195 — API Authentication Boundary Observability

## Intent
As an **admin**, I must observe authentication behavior at the API boundary so that I can diagnose unauthorized requests, expired sessions, and missing credentials.

As a **developer**, I must have visibility into authentication boundary failures so that I can debug 401/403 responses without relying on ad‑hoc logging.

## Value
The authentication boundary is the first line of defense for protected endpoints.  
Without observability, 401/403 responses are opaque and difficult to diagnose.

## Acceptance Criteria
- [ ] AC‑1: Emit `[auth.boundary.start]` when authentication middleware begins evaluation
- [ ] AC‑2: Emit `[auth.boundary.no_credentials]` when no credentials are provided
- [ ] AC‑3: Emit `[auth.boundary.invalid_token]` when token validation fails
- [ ] AC‑4: Emit `[auth.boundary.expired_token]` when token is expired
- [ ] AC‑5: Emit `[auth.boundary.success]` when authentication succeeds
- [ ] AC‑6: Emit `[auth.boundary.end]` with duration and outcome
- [ ] AC‑7: All events include correlation ID and request metadata
- [ ] AC‑8: Test harness can assert boundary observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Authentication failures are visible and understandable.
- **EG‑03 Calm Protection** — Owners feel protected, not blamed.

## Notes
- This story covers the *middleware boundary*, not the OIDC flow (US‑194).
- Authorization decisions are covered separately.
