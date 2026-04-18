# US-103 — Frontend Testing Setup

## User Story

As a **frontend developer**, I want Vitest and React Testing Library configured with a first passing component test wired into CI, so that TDD is possible for all frontend work.

## Context

The proving slice (US-084) requires TDD at the frontend layer. You cannot red-green-refactor without test infrastructure. This story creates the test runner configuration, installs React Testing Library, writes the first passing component test, and wires it into CI. The testing guide produced here becomes the reference for all future frontend TDD.

## Scope

- Vitest configured as the frontend test runner.
- React Testing Library installed and configured.
- First component test (e.g., landing page heading renders).
- CI pipeline updated to run frontend tests.
- Testing guide documenting how to write and run a component test.

## Acceptance Criteria

- [x] Vitest configured as the test runner.
- [x] React Testing Library installed and configured.
- [x] First component test written and passing.
- [x] `npm test` runs all frontend tests.
- [x] CI pipeline updated — frontend tests run alongside backend tests.
- [x] Testing guide documents how to write and run a component test.

## Dependencies

- US-056 (Next.js Project Scaffold).

## Open Questions

- None.
