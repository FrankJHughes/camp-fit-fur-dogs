# ADR 003 — EF Core

## Status
Accepted

## Context

Entity Framework Core (EF Core) is used as the primary ORM for persistence.  
A decision record is required to document why EF Core was selected, how it fits  
into the vertical slice architecture, and how it interacts with domain models,  
repositories, and infrastructure components.

## Decision

Adopt EF Core as the persistence layer for Camp Fit Fur Dogs.  
EF Core provides a mature, well‑supported ORM with strong .NET integration,  
LINQ support, migrations, and flexible mapping options suitable for both  
domain‑driven design and vertical slice architectures.

## Consequences

### Positive

- Strong integration with ASP.NET Core  
- Mature tooling (migrations, scaffolding, design‑time services)  
- Flexible mapping for aggregates and value objects  
- Supports PostgreSQL, SQL Server, and in‑memory providers  
- Works well with repository abstractions  
- Enables efficient query pipelines for CQRS read models  

### Negative

- Requires careful mapping for complex aggregates  
- Lazy loading must be avoided or controlled  
- Migrations require discipline to maintain consistency  
- EF Core abstractions can leak into domain if not isolated properly  

## Implementation

- EF Core lives in the `Infrastructure` layer of each vertical slice  
- Aggregates are mapped using `EntityTypeConfiguration<T>` classes  
- Value objects are mapped using owned types  
- Repositories wrap `DbContext` and expose aggregate‑focused operations  
- Queries use LINQ and projection to DTOs  
- Migrations are stored in the infrastructure project and applied at startup  

## Related

- ADR 001 — Vertical Slice Architecture  
- ADR 002 — CQRS Pattern  
- ADR 004 — PostgreSQL  
- ADR 005 — Unit of Work  

## Notes

Keep this document grounded in the actual EF Core implementation and update it  
as the source architecture evolves.
