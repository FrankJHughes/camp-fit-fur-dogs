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
---

# US-135: CORS Policy

## Intent

As a **system operator**, I need the API to enforce a Cross-Origin Resource
Sharing policy so that only the Camp Fit Fur Dogs frontend can make requests
to the API.

## Value

Without a CORS policy, any website can make API requests on behalf of an
authenticated user. A properly configured CORS policy restricts API access to
the authorized frontend origin, preventing cross-site request forgery and
unauthorized API consumption.

## Acceptance Criteria

- [ ] CORS middleware allows requests only from the Camp Fit Fur Dogs frontend origin
- [ ] Allowed origins are stored in configuration, not hard-coded (supports dev, staging, production)
- [ ] Only required HTTP methods are allowed (GET, POST, PUT, DELETE)
- [ ] Only required headers are allowed (Authorization, Content-Type)
- [ ] Credentials (cookies, authorization headers) are permitted for the allowed origin
- [ ] Preflight (`OPTIONS`) requests are handled correctly and cached
- [ ] Requests from unauthorized origins receive no `Access-Control-Allow-Origin` header (browser blocks the response)
- [ ] Unit tests verify allowed and denied origins
- [ ] Integration test confirms cross-origin requests from unauthorized origins are rejected

## Emotional Guarantees

- **EG-03 Calm Protection** — The API protects itself from unauthorized cross-origin access without owner awareness

## Notes

- Dependencies are shipped: US-056 (API project), US-103 (Next.js frontend)
- CORS is likely partially configured already — this story formalizes and hardens it
- Wildcard (`*`) origins must never be used in production
- **Demo:** Make an API request from the allowed frontend origin (succeeds), then from an unauthorized origin via curl with an `Origin` header (blocked)
