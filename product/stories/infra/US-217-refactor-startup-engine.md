---
id: US-217
title: "Refactor StartupEngine to use ImmutabilityContext"
domain: infra
status: backlog
vertical_slice: false
dependencies:
  - US-183
  - US-185
---

# US‑217 — Refactor StartupEngine to use ImmutabilityContext

## Intent
As an **admin**, I must be able to rely on the StartupEngine using an immutable context model so that startup modules execute deterministically and cannot introduce hidden state or ordering‑dependent bugs.

## Value
Refactoring StartupEngine to use `ImmutabilityContext` brings startup behavior in line with the immutable patterns already used for authentication callbacks.  
This improves testability, observability, and safety when multiple startup modules contribute DI registrations, configuration bindings, and initialization steps.

## Acceptance Criteria
- [ ] StartupEngine is refactored to use an `ImmutabilityContext`-based startup context
- [ ] Startup modules receive an immutable context and return a new context instance
- [ ] No startup module mutates shared state or previously created context instances
- [ ] Guardrail tests verify deterministic startup given the same configuration and modules
- [ ] Observability (US‑183) can inspect and log the startup context safely
- [ ] Test harness can construct, run, and compare startup context snapshots
- [ ] Documentation is updated to describe StartupEngine’s use of `ImmutabilityContext`

## Notes
- Existing `ImmutabilityContext` usage in auth callback contexts is the canonical pattern
- This story is a refactor only; no new startup features are introduced
