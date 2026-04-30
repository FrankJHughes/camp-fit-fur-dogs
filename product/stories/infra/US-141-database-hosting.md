---
id: US-141
title: "Database Hosting"
epic: Infrastructure
milestone: M2
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-050
  - US-107
---

# US-141: Database Hosting

## Intent

As a **system operator**, I need the application database hosted on a free cloud
database service so that the deployed API can persist data without relying on a
local SQL Server container.

## Value

The local Docker SQL Server container works for development but cannot serve a
deployed application. A hosted database is the final piece of the deployment
puzzle — without it, the hosted API has nowhere to write. Choosing a free tier
with enough headroom ensures the project stays zero-cost through development
and early production.

## Acceptance Criteria

- [ ] Database is provisioned on a free-tier cloud database service
- [ ] EF Core migrations run successfully against the hosted database
- [ ] Existing seed data (if any) populates correctly
- [ ] Database is accessible only from the API hosting platform (no public endpoint, or IP-restricted)
- [ ] Backup strategy is documented (platform-provided or manual)
- [ ] Performance is acceptable for the current entity count and query patterns
- [ ] Local development continues to use the Docker SQL Server container (no disruption)
- [ ] Connection string switching between local and hosted is handled via environment/configuration

## Emotional Guarantees

- **EG-03 Calm Protection** — Data is persisted reliably; connection credentials are never exposed

## Platform Options (free tier)

| Platform | Engine | Free Storage | Expiration | Best For |
|----------|--------|-------------|------------|----------|
| **Azure SQL Database** | SQL Server | 32 GB | Lifetime (per subscription) | Best SQL Server compat, no migration needed |
| **Neon** | PostgreSQL (serverless) | 512 MB | No expiration | Best serverless PostgreSQL, scales to zero |
| **Supabase** | PostgreSQL | 500 MB | No expiration (2 free projects) | Best full-stack PostgreSQL with extras |
| **CockroachDB** | PostgreSQL-compat | 10 GB | No expiration | Best free storage amount |

> **Recommendation:** Azure SQL Database free offer — 32 GB lifetime free, native
> SQL Server compatibility means zero EF Core provider changes. The app already
> uses SQL Server locally via Docker, so the migration is a connection string
> swap. If the team prefers PostgreSQL, Neon is the strongest serverless option
> but requires switching the EF Core provider (a non-trivial but bounded change).

## Design Seam: Provider Portability

> If a future ADR decides to switch from SQL Server to PostgreSQL, the EF Core
> abstraction layer means the migration is isolated to the Infrastructure project
> — domain and application layers are unaffected. This story should not force a
> provider decision; it should document the choice and its rationale.

## Notes

- Dependencies are shipped: US-050 (Unit of Work), US-107 (EF Entity Auto-Discovery)
- Azure SQL free offer: 100,000 vCore seconds + 32 GB per month, auto-pause when idle
- If Azure SQL is selected, provision via Azure Portal or Azure CLI
- Connection string must use managed identity or username/password stored as env var
- **Demo:** Deploy the API with the hosted database connection, create a customer account via the API, query the database and show the row
