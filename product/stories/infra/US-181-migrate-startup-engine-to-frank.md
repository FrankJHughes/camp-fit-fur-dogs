---
id: US-181
title: "Migrate Startup Engine to Frank"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-108
  - US-180
---

# US‑181 — Migrate Startup Engine to Frank

## Intent

As an **admin**, I must have a unified startup engine that configures DI, modules, and validation using Frank conventions so that the system boots consistently and safely across all environments.

## Acceptance Criteria

- [ ] AC‑1: Startup engine is extracted into a Frank module with a single public entry point  
- [ ] AC‑2: Startup engine registers all DI modules using the Frank module loader  
- [ ] AC‑3: Startup engine validates handler registrations (no missing handlers, no duplicates)  
- [ ] AC‑4: Startup engine validates configuration (required keys present, no missing secrets)  
- [ ] AC‑5: Startup engine exposes a unified `ConfigureServices()` method  
- [ ] AC‑6: No legacy startup code remains in the API project  

## Notes

This story completes the migration of application startup into the Frank architecture.
