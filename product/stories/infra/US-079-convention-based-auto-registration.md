# US-079 — Convention-Based Auto-Registration

> Repurposed from "React Project Skeleton." Original concept absorbed into US-056 (Next.js Project Scaffold).

## User Story

As a **contributor**, I want handlers, validators, and repositories discovered and registered automatically by convention, so that adding a new slice requires zero edits to shared configuration files.

## Context

Sprint 4 Phase 2. After the proving slice (US-084) ships, we know what a real slice looks like. This story refactors the DI registration so that new slices are automatically discovered — no manual wiring in `Program.cs`. The proving slice (US-084) serves as both the dependency and the regression baseline: if auto-registration breaks anything, the existing RegisterDog flow catches it.

## Scope

- Assembly scanning for handlers, validators, and repositories.
- Zero manual registration for slice-specific types.
- Convention rules documented (file/folder naming contracts, discovery rules).

## Acceptance Criteria

- [x] All handlers discovered and registered by assembly scanning convention.
- [x] All validators discovered and registered by assembly scanning convention.
- [x] All repositories discovered and registered by convention.
- [x] Zero manual registration in `Program.cs` or startup for slice-specific types.
- [x] Existing RegisterDog slice (US-084) still works after refactor.
- [x] New slice addition requires only adding files in the slice folder — no shared file edits.
- [x] Convention rules documented: file/folder naming contracts and discovery rules.

## Dependencies

- US-084 (Register Dog Page — proving slice must ship first).

## Open Questions

- None.
