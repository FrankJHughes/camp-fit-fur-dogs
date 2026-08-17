---
id: US-193
title: "OIDC Protocol Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-192
---

# US‑193 — OIDC Protocol Observability

## Intent
As an **admin**, I must be able to observe the OIDC protocol flow inside the authentication pipeline so that I can diagnose authentication issues without exposing sensitive information.

As a **developer**, I must have clear, structured logs for protocol‑level behavior so that debugging authentication failures is fast, safe, and deterministic.

## Value
Protocol‑level failures (token exchange, userinfo, claims normalization) are currently opaque.  
Structured, non‑PII logging enables safe debugging across environments and reduces time‑to‑resolution for authentication issues.

## Acceptance Criteria
- Structured logs emitted for:
  - Token exchange start/end  
  - Userinfo fetch start/end  
  - Claims normalization  
- All logs include correlation IDs  
- No PII is ever logged:
  - No authorization codes  
  - No access tokens  
  - No ID tokens  
  - No userinfo payloads  
  - No email addresses  
- Protocol failures logged with safe, structured context  
- Logs follow Frank logging conventions  
- Developer can trace protocol flow end‑to‑end using correlation ID  
- Unit tests validate logging behavior where applicable  

## Emotional Guarantees
- **EG‑03 Calm Protection** — Sensitive identity data is never exposed in logs.  
- **EG‑05 Clear Boundaries** — Protocol behavior is visible without leaking business logic.  
- **EG‑06 Developer Confidence** — Debugging protocol issues is predictable and fast.

## Example Log Sequence
```
{
  "event": "auth.callback.received",
  "layer": "api",
  "correlationId": "c-9f3a1d",
  "path": "/api/auth/callback",
  "hasCode": true
}

{
  "event": "oidc.token.exchange.start",
  "layer": "protocol",
  "correlationId": "c-9f3a1d",
  "provider": "auth0"
}

{
  "event": "oidc.token.exchange.end",
  "layer": "protocol",
  "correlationId": "c-9f3a1d",
  "status": "success"
}

{
  "event": "oidc.userinfo.fetch.start",
  "layer": "protocol",
  "correlationId": "c-9f3a1d"
}

{
  "event": "oidc.userinfo.fetch.end",
  "layer": "protocol",
  "correlationId": "c-9f3a1d",
  "status": "success"
}

{
  "event": "oidc.claims.normalized",
  "layer": "protocol",
  "correlationId": "c-9f3a1d",
  "externalIdPresent": true
}
```

## Actor Consumption
Admins consume these logs through the log aggregation system defined in US‑192.

Developers consume these logs through local console output, container logs, and integration test traces.

## Notes
- Logging occurs only in the protocol layer, not Application or API.  
