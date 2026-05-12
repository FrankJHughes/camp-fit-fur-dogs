---
id: US-135
title: "CORS Policy"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-056
  - US-103
urgency: high
importance: high
covey_quadrant: Q1
emotional_guarantees:
legal_guarantees:
---

# US‑135: CORS Policy

## Intent
As a **system operator**, I need the API to enforce a **strict Cross‑Origin Resource Sharing (CORS) policy** so that only trusted frontend applications can call the API from a browser environment.

## Value
CORS is a browser‑enforced security boundary. Without a strict policy, any website could attempt to call the API using a logged‑in owner’s session, enabling CSRF‑style attacks or data exfiltration. A correct CORS configuration ensures only approved origins can interact with the API, while still supporting local development and preview deployments.

## Acceptance Criteria

### Allowed Origins
- Only approved origins are permitted to call the API.
- Allowed origins are environment‑specific:
  - **Local development:** `http://localhost:*`
  - **Staging:** staging frontend URL(s)
  - **Production:** production frontend URL(s)
- Wildcards (`*`) are **not** allowed in production.

### Allowed Methods & Headers
- Allowed methods: `GET`, `POST`, `PUT`, `DELETE`, `OPTIONS`.
- Allowed headers include:
  - `Content-Type`
  - `Authorization`
  - `Accept`
  - Any additional headers required by the frontend build system.
- `Access-Control-Allow-Credentials` is enabled only when necessary.

### Preflight Behavior
- Preflight (`OPTIONS`) requests return:
  - `200 OK`
  - Correct `Access-Control-Allow-*` headers
  - No body
- Preflight caching (`Access-Control-Max-Age`) is configured to reduce browser overhead.

### Security Constraints
- No wildcard origins in production.
- No wildcard headers in production.
- No wildcard methods in production.
- CORS configuration is centralized in middleware, not per‑endpoint.
- CORS must not leak internal environment details.

### Observability & Testing
- Integration tests verify:
  - Allowed origins succeed.
  - Disallowed origins fail with correct CORS errors.
  - Preflight requests return correct headers.
- Logs include CORS failures with origin and endpoint (no sensitive data).

## Emotional Guarantees
- **EG‑03 — Calm Protection**  
  Owners are protected from malicious websites attempting to access their data.

## Notes
- Depends on US‑056 (API Endpoint Purity) and US‑103 (Environment Configuration).
- Must support Vercel preview deployments (dynamic origin whitelisting).
- **Demo:** Attempt API calls from an unapproved origin and show the browser‑blocked CORS error.
