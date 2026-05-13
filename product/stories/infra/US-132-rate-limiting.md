---
id: US-132
title: "Rate Limiting"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-056
urgency: high
importance: high
covey_quadrant: Q1
emotional_guarantees:
legal_guarantees:
---

# US‑132: Rate Limiting

## Intent
As a **system operator**, I need the API to **throttle excessive requests per client** so that brute‑force attacks, credential stuffing, and denial‑of‑service attempts are mitigated before they reach application logic.

## Value
Login and registration endpoints are prime targets. Without rate limiting, an attacker can attempt thousands of logins per second or flood registration with bot accounts. Rate limiting is the first line of defense — inexpensive to implement, high impact, and protects every endpoint uniformly.

## Acceptance Criteria

### Global Policy
- A global default rate‑limit policy is applied to all endpoints.
- Default policy limits requests **per IP per time window** (configurable).
- Thresholds are stored in configuration, not hard‑coded.
- Legitimate users under normal usage patterns are never throttled.

### High‑Risk Endpoints
- Authentication endpoints (login, registration, password reset) use a **stricter policy**.
- Write operations (POST, PUT, DELETE) use stricter thresholds than read operations (GET).

### Behavior Under Throttling
- Rate‑limited requests return `429 Too Many Requests`.
- Responses include a `Retry‑After` header with the correct wait time.
- Throttling is deterministic and does not leak internal state.

### Observability & Testing
- Unit tests verify policy enforcement and 429 responses.
- Integration test simulates burst traffic and confirms throttling activates.
- Logs include: timestamp, IP, endpoint, and policy triggered (no sensitive data).

## Emotional Guarantees
- **EG‑01 — No Surprises**  
  Normal usage is never affected; only abusive patterns trigger throttling.
- **EG‑03 — Calm Protection**  
  The system protects itself and its users without requiring owner awareness.

## Notes
- ASP.NET Core 7+ includes built‑in rate limiting middleware (`Microsoft.AspNetCore.RateLimiting`).
- Consider sliding‑window or token‑bucket algorithms for smoother throttling.
- Pair with US‑133 (Account Lockout) for layered brute‑force defense.
- **Demo:** Use a load‑testing tool to exceed the threshold — show 429 responses after the limit is hit, then normal responses after the window resets.
