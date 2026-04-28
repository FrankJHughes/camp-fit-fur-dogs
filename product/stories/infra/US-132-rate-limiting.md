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
---

# US-132: Rate Limiting

## Intent

As a **system operator**, I need the API to throttle excessive requests per client
so that brute-force attacks, credential stuffing, and denial-of-service attempts
are mitigated before they reach application logic.

## Value

Login and registration endpoints are prime targets. Without rate limiting, an
attacker can attempt thousands of logins per second or flood registration with
bot accounts. Rate limiting is the first line of defense — cheap to implement,
high impact, and protects every endpoint uniformly.

## Acceptance Criteria

- [ ] Rate limiting middleware is registered in the ASP.NET Core pipeline
- [ ] Global default policy limits requests per IP per time window (configurable)
- [ ] Stricter policy for authentication endpoints (login, registration, password reset)
- [ ] Stricter policy for write operations (POST, PUT, DELETE) vs. read operations (GET)
- [ ] Rate-limited requests receive a `429 Too Many Requests` response with a `Retry-After` header
- [ ] Rate limit thresholds are stored in configuration, not hard-coded
- [ ] Legitimate users under normal usage patterns are never throttled
- [ ] Unit tests verify policy enforcement and 429 responses
- [ ] Integration test simulates burst traffic and confirms throttling activates

## Emotional Guarantees

- **EG-01 No Surprises** — Normal usage is never affected; only abusive patterns trigger throttling
- **EG-03 Calm Protection** — The system protects itself and its users without requiring owner awareness

## Notes

- ASP.NET Core 7+ has built-in rate limiting middleware (`Microsoft.AspNetCore.RateLimiting`)
- Consider sliding window or token bucket algorithms for smoother throttling
- Pair with US-133 (Account Lockout) for layered brute-force defense
- **Demo:** Use a load testing tool to exceed the threshold — show 429 responses after the limit is hit, then normal responses after the window resets
