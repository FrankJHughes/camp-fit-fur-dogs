# ADR-0021: Query-Side Reader Isolation

| Field     | Value       |
|-----------|-------------|
| Status    | Accepted    |
| Date      | 2026-04-18  |
| Deciders  | Frank Hughes|

## Context

Query handlers depended on aggregate repository interfaces (e.g., `IDogRepository`)
that bundle read and write methods. This couples the query side to command-side
contracts, prevents independent optimization of reads (projections, raw SQL,
caching), and blurs the CQRS boundary established in ADR-0011.

## Decision

Query handlers depend on slice-scoped reader interfaces instead of repositories.
Each reader interface:

- Lives in `Application.Abstractions` alongside its query/response types.
- Is named after its slice: `IGetDogProfileReader` (verb + noun + Reader).
- Returns the query's response DTO directly — no domain aggregates cross the boundary.
- Has its Infrastructure implementation registered via Scrutor's `Reader` suffix scan.

Repositories (`IDogRepository`, `ICustomerRepository`) remain exclusively for
command handlers that need aggregate persistence.

An architecture guardrail test enforces the boundary: no `IQueryHandler`
implementation may depend on any interface ending in `Repository`.

## Why Not Command-Side Writers?

The asymmetry is intentional. Command handlers *need* the aggregate — they load it,
enforce invariants through domain methods, and persist it. The repository is the
correct DDD abstraction because it deals in aggregates. A slice-scoped "writer"
(`IRegisterDogWriter`) would just be a thin wrapper around `AddAsync` —
indirection without value.

Query handlers never need the aggregate. They need a DTO. Injecting
`IDogRepository` forces the handler to know about `Dog`, `DogId`, `OwnerId` —
domain concepts it shouldn't touch. The reader returns the response DTO directly.

| Side    | Abstraction                  | Returns          | Why                                              |
|---------|------------------------------|------------------|--------------------------------------------------|
| Command | Repository (`IDogRepository`)| Domain aggregate | Handler enforces invariants via domain methods    |
| Query   | Reader (`IGetDogProfileReader`)| Response DTO   | Handler has no domain logic — just data retrieval |

This is textbook CQRS: writes go through the domain model, reads bypass it.

## Consequences

### Positive

- Clean CQRS boundary — reads never touch aggregate contracts.
- Readers can be independently optimized (projections, raw SQL, caching).
- Architecture guardrail enforces the rule automatically.

### Negative

- Two Infrastructure file patterns per feature (repository + reader) instead of one.
- Contributors must learn when to use which abstraction.

### Neutral

- Repositories remain unchanged — this is additive.
- Scrutor scan picks up `Reader` suffix automatically (ADR-0015).
