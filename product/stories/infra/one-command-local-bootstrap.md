# One-Command Local Bootstrap

## Intent
A developer who prefers working locally runs a single command that validates
their environment, starts infrastructure services via container orchestration,
builds the solution, runs the test suite, and prints a readiness report —
without needing to know the specific toolchain commands.

## Value
Local development is the primary onboarding path. The bootstrap command
catches mismatches early (wrong SDK version, Docker not running, ports
unavailable) instead of letting them surface as cryptic errors mid-setup.
Infrastructure runs in containers using the same declarative definitions
as the containerized environment and CI — one specification, zero divergence.

## Acceptance Criteria
- [ ] A single entry-point command exists for each supported OS
- [ ] The command checks for required prerequisites — including a container runtime — and prints actionable messages for anything missing
- [ ] Infrastructure services start using the shared declarative definitions
- [ ] The command calls the standardized task runner for restore, build, and test
- [ ] Final output is a human-readable readiness summary (pass/fail per check, elapsed time)
- [ ] The command is idempotent — safe to re-run at any time
- [ ] README documents the bootstrap command as the primary getting-started path

## Out of Scope
- Automatic installation of system-level prerequisites (e.g., SDKs, container runtimes)
- Prescribing scripting language or specific tooling
- Containerized code execution (covered by Containerized Development Environment)

## Emotional Guarantees: N/A
