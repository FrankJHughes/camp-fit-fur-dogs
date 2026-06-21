---
id: US-183
title: "Observability Exploration"
epic: Infrastructure
milestone: M1+
status: ready
domain: infra
vertical_slice: false
dependencies:
  - US-180
  - US-181
---

# US‑183 — Observability Exploration

## Intent
As an **admin**, I must understand what observability capabilities are required across the Frank hosting and startup engines so that future stories can deliver unified, structured, and diagnosable system behavior.

As a **developer**, I must understand what information is missing from the current logging, metrics, and diagnostics surfaces so that I can design an observability model that supports debugging, tracing, and operational insight.

## Value
This exploration defines the *scope*, *gaps*, and *requirements* for observability across the Frank platform.  
It ensures that subsequent implementation stories are grounded in real needs, not assumptions.

## Acceptance Criteria
- [ ] AC‑1: Current observability gaps are documented across:
      - Hosting lifecycle  
      - Startup engine lifecycle  
      - Module discovery and loading  
      - Handler execution  
      - Error boundaries  
      - Authentication flows  
      - Vertical slices (see AC‑6)  
- [ ] AC‑2: Required observability surfaces for Admins and Developers are identified  
- [ ] AC‑3: Required log structure, event boundaries, and correlation‑ID propagation rules are defined  
- [ ] AC‑4: Required metrics and dimensions are defined  
- [ ] AC‑5: Risks and constraints are documented (e.g., no existing log aggregation system)  
- [ ] AC‑6: Observability requirements are mapped across **all vertical slices**, not just authentication  
- [ ] AC‑7: A complete set of implementation stories is produced based on the findings  

## Notes
This story produces *knowledge and planning*, not code.  
It ensures the Frank platform becomes diagnosable and production‑ready by defining the observability model before implementation begins.
