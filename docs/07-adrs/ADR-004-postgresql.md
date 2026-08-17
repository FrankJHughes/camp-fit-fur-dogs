# ADR 004 — PostgreSQL

## Status
Accepted

## Context

The application requires a reliable, scalable, and well‑supported relational database.  
PostgreSQL offers strong SQL compliance, robust indexing, JSON support, extensions,  
and excellent compatibility with EF Core. It is widely supported across hosting  
providers such as Render, AWS RDS, Azure Database for PostgreSQL, and Docker.

## Decision

Adopt PostgreSQL as the primary database engine for Camp Fit Fur Dogs.

PostgreSQL is used for all production environments and recommended for development  
via Docker or local installations. EF Core’s Npgsql provider integrates cleanly  
with aggregates, value objects, and vertical slice infrastructure.

## Consequences

### Positive

- Fully open‑source and widely supported  
- Strong EF Core integration via Npgsql  
- Rich feature set (JSONB, indexing, constraints, extensions)  
- High reliability and ACID compliance  
- Easy to containerize for development  
- Supported by major cloud providers  

### Negative

- Requires operational knowledge for tuning and maintenance  
- Migrations must be carefully managed across environments  
- Some advanced features differ from SQL Server  
- Local setup may require Docker for consistency  

## Implementation

- Use Npgsql EF Core provider  
- Configure connection strings via `appsettings*.json`  
- Apply migrations at startup in development environments  
- Store migrations in the infrastructure project  
- Use Docker Compose for local PostgreSQL + PgAdmin  
- Use managed PostgreSQL services in production (Render, AWS RDS, Azure)  

## Related

- ADR 003 — EF Core  
- ADR 005 — Unit of Work  
- ADR 006 — Immutable Contexts  

## Notes

Keep this document grounded in the actual PostgreSQL implementation and update it  
as the source architecture evolves.
