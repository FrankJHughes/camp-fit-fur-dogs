# Containerized Development Environment

**Issue:** #58
**Layer:** L2 – Containerized Environment (Diamond Model, ADR-003)

## User Story

As a developer who prefers containerized environments, I want to open the
repository in a dev container and land in a fully working setup — SDK,
tools, infrastructure, and passing tests — so that I can start coding
without installing anything beyond Docker.

## Acceptance Criteria

- [ ] Dev container configuration exists and produces a ready-to-code
      environment
- [ ] SDK, CLI tools, and extensions are present without manual
      installation
- [ ] Infrastructure starts automatically using shared definitions
- [ ] Restore, build, and test pass without manual intervention
- [ ] README documents the launch path

## Dependencies

- **L1 – Shared infrastructure definitions** (Issue #56, ADR-0004):
  PostgreSQL, Redis, and RabbitMQ services from `compose.yml`.
- **L4 – Standardized developer commands** (Issue #57, ADR-0005):
  `make all` runs restore, build, and test in the `postCreateCommand`.

## Decision Record

See [ADR-0006](../../../docs/adr/0006-dev-container-for-development.md).