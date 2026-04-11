# US-051 — ADR-0011: CQRS with Command and Query Pipelines

## Intent

As a developer, I want an ADR that records the decision to use CQRS with explicit command and query pipelines so that I understand the architectural rationale and know how to structure new vertical slices.

## Value

The codebase now routes every write through `ICommand` / `ICommandHandler` / `ICommandDispatcher` and every read through `IQuery<TResponse>` / `IQueryHandler` / `IQueryDispatcher`. ADR-0002 still describes "application services" as the orchestration mechanism — a new contributor reading the ADRs would expect fat service classes, not thin handler pipelines. Recording this decision closes the gap between the documentation and the code, and gives future contributors a clear rationale for why CQRS was chosen over traditional service-layer orchestration.

## Acceptance Criteria

- `docs/adr/0011-cqrs-command-query-pipelines.md` exists and follows TEMPLATE.md
- ADR explains the decision to separate commands (writes) from queries (reads) at the application layer
- ADR documents the key abstractions: `ICommand`, `ICommandHandler`, `ICommandDispatcher`, `IQuery<TResponse>`, `IQueryHandler`, `IQueryDispatcher`
- ADR references ADR-0002 as context (DDD layered architecture) and explains how CQRS refines the application layer
- ADR index (`docs/adr/README.md`) includes the new entry
- Status is **Accepted**

## Emotional Guarantees

- EG-01 No surprises — the architecture docs match what the code actually does
- EG-04 Always know where you stand — a new contributor can trace the full request pipeline from ADR alone
