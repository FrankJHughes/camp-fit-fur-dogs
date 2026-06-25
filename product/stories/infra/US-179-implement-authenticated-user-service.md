---
id: US-179
title: "Implement Authenticated User Service"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-110
  - US-175
---

# US‑179 — Implement Authenticated User Service

## Intent

As an **owner**, I must have my authenticated identity available to all handlers so that the system can enforce authorization, personalization, and ownership rules consistently across the application.

## Acceptance Criteria

- [ ] AC‑1: AuthenticatedUser exposes a single method to retrieve the authenticated user’s identity  
- [ ] AC‑2: Service returns a strongly‑typed user identity (OwnerId, Email, Claims)  
- [ ] AC‑3: Service returns `Unauthenticated` state when no identity is present  
- [ ] AC‑4: Service integrates with the callback engine and OIDC middleware  
- [ ] AC‑5: Service is registered once in DI and available to all handlers  
- [ ] AC‑6: No handler reads claims directly from HttpContext  

## Emotional Guarantees

- **EG‑01 — No Surprises:** The owner must always see the correct authenticated identity reflected in the system.
- **EG‑03 — Calm Protection:** Identity information must be handled safely, predictably, and without exposing sensitive data.
- **EG‑05 — Clear Ownership:** The owner must always understand which identity is being used when performing actions.

## Notes

This story unifies identity access across the Frank architecture and removes direct HttpContext dependencies.
