---
id: US-218
title: "Migrate Exception‑to‑Problem Pipeline to Frank"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-176   # Test Harness Migration
  - US-183   # Observability
---

# US‑218 — Migrate Exception‑to‑Problem Pipeline to Frank

## Intent

As an **admin**, I must be able to rely on a unified, platform‑level
Exception‑to‑Problem translation pipeline so that all Frank‑based products
benefit from consistent error handling, safe problem shaping, and predictable
HTTP semantics without re‑implementing the logic in each API.

## Value

Today, the Exception‑to‑Problem pipeline lives entirely inside
CampFitFurDogs.Api. This creates:

- duplication across future products  
- inconsistent error semantics  
- inconsistent logging and observability  
- difficulty testing exception behavior in the Frank test harness  
- tight coupling between product code and error‑handling infrastructure  

Migrating this capability into Frank:

- centralizes exception classification  
- standardizes Problem shaping  
- unifies HTTP mapping rules  
- improves observability (US‑183)  
- enables deterministic exception testing (US‑176)  
- reduces product‑level boilerplate  
- ensures all Frank‑based APIs behave consistently  

This becomes a **core platform capability**, not a product‑specific one.

## Acceptance Criteria

- [ ] A new Frank module provides a unified Exception‑to‑Problem pipeline  
- [ ] Frank exposes a standard `IExceptionToProblemMapper` abstraction  
- [ ] Frank provides default mappings for:
      - validation exceptions  
      - authentication exceptions  
      - authorization exceptions  
      - domain rule exceptions  
      - not‑found exceptions  
      - infrastructure exceptions  
      - unexpected exceptions  
- [ ] Frank shapes Problems using a consistent, safe ProblemDetails model  
- [ ] Frank middleware catches all unhandled exceptions and delegates to the mapper  
- [ ] HTTP status codes follow platform rules (400/401/403/404/409/500)  
- [ ] No exception details (stack traces, messages, type names) leak to clients  
- [ ] Observability hooks emit structured exception events (US‑183)  
- [ ] Test harness (US‑176) can simulate exceptions and assert shaped Problems  
- [ ] CampFitFurDogs.Api removes its local exception‑handling middleware and registry  
- [ ] CampFitFurDogs.Api adopts the Frank capability with zero custom logic  

## Emotional Guarantees

- **EG‑01 No Surprises** — All Frank‑based APIs behave the same when failures occur  
- **EG‑03 Calm Protection** — Clients receive safe, predictable error responses  
- **EG‑05 Clear Boundaries** — Exceptions remain internal; Problems are the public contract  

## Notes

- This story does **not** redesign the Problem model; it migrates the existing capability into Frank.  
- Product‑specific exceptions may register additional mappings via extension points.  
- This capability becomes part of Frank’s HTTP boundary layer, not the dispatcher pipeline.  
- Consider adding an ADR documenting the platform‑level Problem model and mapping rules.  
