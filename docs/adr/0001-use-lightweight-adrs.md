# ADR-001: Use lightweight ADRs for architecture decisions

| Field     | Value          |
|-----------|----------------|
| Status    | Accepted       |
| Date      | 2026-03-30     |
| Deciders  | Frank Hughes   |

## Context

Camp Fit Fur Dogs is a portfolio project that will evolve over multiple
sprints. Architectural decisions — framework choices, layer boundaries,
persistence strategies — are made incrementally and often revisited.
Without a written record, the reasoning behind past decisions is lost,
making it hard to onboard contributors or evaluate trade-offs later.

## Decision

We will record significant architecture decisions using Architecture
Decision Records (ADRs) stored in `docs/adr/`.

Each ADR follows the template in `docs/adr/TEMPLATE.md` and is numbered
sequentially (`0001`, `0002`, …). ADRs are versioned alongside the code
they describe and are reviewed as part of pull requests.

An ADR's status progresses through: **Proposed → Accepted → Deprecated**
(or **Superseded by ADR-NNN**).

## Consequences

### Positive

- Decisions are discoverable and searchable in the repo
- New contributors can understand *why* the system is shaped the way it is
- Trade-offs are documented at decision time, not reconstructed from memory
- Status field makes it clear which decisions are still in effect

### Negative

- Adds a lightweight writing step to the architecture workflow
- ADRs can drift if not updated when decisions are revisited

### Neutral

- ADRs are plain Markdown — no tooling required to read or write them