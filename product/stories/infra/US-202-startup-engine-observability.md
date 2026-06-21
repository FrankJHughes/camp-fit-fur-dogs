---
id: US-202
title: "Startup Engine Observability"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-181
  - US-197
  - US-198
---

# US‑202 — Startup Engine Observability

## Intent
As an **admin**, I must observe startup engine behavior so that I can diagnose module load failures, configuration issues, and startup sequencing problems.

As a **developer**, I must have visibility into startup execution so that I can debug module initialization without relying on console output.

## Value
Startup is one of the most failure‑prone phases of the system.  
Without observability, diagnosing startup failures requires attaching debuggers or reading raw logs.

## Acceptance Criteria
- [ ] AC‑1: Emit `[startup.begin]` when the startup engine begins initialization
- [ ] AC‑2: Emit `[startup.module.start]` for each module with module name and order
- [ ] AC‑3: Emit `[startup.module.success]` when a module completes initialization
- [ ] AC‑4: Emit `[startup.module.failed]` with sanitized exception info
- [ ] AC‑5: Emit `[startup.end]` with total duration and module count
- [ ] AC‑6: All events include correlation ID and environment metadata
- [ ] AC‑7: Test harness can assert startup observability

## Emotional Guarantees
- **EG‑05 Clear Boundaries** — Startup behavior is predictable and transparent.
- **EG‑06 Developer Confidence** — Startup failures are diagnosable without guesswork.

## Notes
- Complements US‑201 (Hosting Engine Observability).
- Must integrate with the Immutable Context Builder (US‑203).
