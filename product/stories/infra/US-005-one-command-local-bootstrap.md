# One-Command Local Bootstrap

**Issue:** #55
**Layer:** L5 – One-Command Onboarding (Diamond Model, ADR-003)

## User Story

As a developer cloning the repo for the first time, I want a single
command that validates my environment, starts infrastructure, builds
the solution, runs all tests, and prints a readiness report — so that
I can go from clone to coding without reading a setup guide.

## Acceptance Criteria

- [x] A platform-native bootstrap script exists for each supported OS
      (`bootstrap.sh` for Linux/macOS/WSL, `bootstrap.ps1` for Windows).
- [x] The script validates prerequisites and exits early with clear
      install instructions if anything is missing.
- [x] Infrastructure starts automatically using shared definitions
      from `compose.yml`.
- [x] Restore, build, and test run without manual intervention.
- [x] A readiness report prints service endpoints, pipeline status,
      and elapsed time.
- [x] The command is idempotent — running it again produces the same
      result without side effects.
- [x] README documents the bootstrap command.

## Dependencies

- **L1 – Shared infrastructure definitions** (Issue #56, ADR-0004):
  `docker compose up -d --wait` starts services from `compose.yml`.
- **L4 – Standardized developer commands** (Issue #57, ADR-0005):
  `bootstrap.sh` calls `make all` for the build/test pipeline.

## Decision Record

See [ADR-0007](../../../docs/adr/0007-US-005-one-command-local-bootstrap.md).