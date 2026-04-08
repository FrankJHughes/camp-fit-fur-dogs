# Standardized Developer Commands

**Issue:** #57
**Layer:** L4 – Developer Workflow (Diamond Model, ADR-003)

## User Story

As a developer, I want a single, discoverable command interface at the repo
root so that build, test, and infrastructure tasks work identically on my
machine and in CI—without memorising raw CLI flags.

## Acceptance Criteria

- [ ] A `Makefile` at the repo root exposes standard targets: `restore`,
      `build`, `test`, `clean`, `infra-up`, `infra-down`, `all`.
- [ ] `make help` lists every target with a one-line description.
- [ ] CI (`Build & Test` workflow) invokes `make all` instead of raw
      `dotnet` commands.
- [ ] Targets produce identical results locally, in the dev container,
      and in CI.

## Dependencies

- **L1 – Shared infrastructure definitions** (Issue #56, ADR-0004):
  `infra-up` / `infra-down` targets wrap `docker compose`.

## Decision Record

See [ADR-0005](../../../docs/adr/0005-makefile-for-standardized-commands.md).