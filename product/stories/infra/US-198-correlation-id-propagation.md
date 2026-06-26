---
id: US-198
title: "Correlation-ID Propagation"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-197
---

# US‑198 — Correlation-ID Propagation

## Intent

As an **admin**, I must be able to trace a request end‑to‑end so that I can diagnose failures across API, protocol, application, and vertical slices.

As a **developer**, I must have correlation IDs automatically propagated so that I can debug complex flows without manually wiring identifiers.

## Value

Correlation IDs unify logs, metrics, and events across all layers, enabling deterministic request tracing and dramatically reducing debugging time.

## Acceptance Criteria

- [ ] **AC‑1 — Correlation ID created at API boundary**  
      - If inbound request contains a correlation ID → use it  
      - Otherwise → generate a new one  
      - Stored in `IObservabilityContext` and `ICorrelationContext`

- [ ] **AC‑2 — Propagated through all architectural layers**  
      - Protocol layer  
      - Application layer  
      - Handler pipeline  
      - Vertical slices  
      - Domain events  
      - Outbound HTTP calls  
      - Background tasks (if applicable)

- [ ] **AC‑3 — Included in all structured events**  
      - All `ITraceEvents` emissions include correlation ID  
      - Event schema validated in tests

- [ ] **AC‑4 — Included in all error boundary events**  
      - Error observer attaches correlation ID  
      - Error flow tests assert presence

- [ ] **AC‑5 — Included in all metrics (within cardinality rules)**  
      - Counters, timers, gauges include correlation ID only when cardinality is safe  
      - Metrics tests validate inclusion/exclusion rules

- [ ] **AC‑6 — Test harness can assert correlation propagation**  
      - WebApplicationFactory test verifies correlation ID flows end‑to‑end  
      - Outbound call test verifies propagation to external systems

## Emotional Guarantees

- **EG‑01 — No Surprises**  
  Every log, event, and error is traceable to a single request.

- **EG‑06 — Developer Confidence**  
  Debugging becomes deterministic and repeatable.

## Notes

Correlation IDs are the backbone of cohesive observability and are required for all future observability stories (metrics, events, error boundaries, distributed tracing).
