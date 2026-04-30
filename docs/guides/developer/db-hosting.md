# Neon Database Hosting & Backup Strategy

This guide documents how the production database is hosted on Neon, how access is controlled, and how backups and restores are handled. It supports the Database Hosting story (US‑141) and provides the operational guarantees required for production reliability.

---

## Overview

The application uses Neon as the hosted PostgreSQL provider for production. Neon provides:

- Free‑tier hosting suitable for early production use
- Point‑in‑time restore (PITR) for disaster recovery
- Zero‑cost branching for snapshots and testing
- Fine‑grained access control via IP allowlists and roles
- A GitHub integration for CI branch databases (used by integration tests)

Local development continues to use Docker SQL Server. Production uses Neon PostgreSQL.

---

## Application Database Users

Neon uses role‑based access control. The following application‑scoped users are created:

- `app_local` — used only for local development
- `app_production` — used by the deployed API
- Neon GitHub integration user — used only for CI branch databases and migrations

Each application user has:

- Permission to connect to the database
- Permission to use the `public` schema
- CRUD permissions on all tables
- Default privileges ensuring new tables are accessible

Application users do **not** have DDL permissions. Schema changes are applied only through migrations.

---

## Access Restrictions

Neon provides an IP allowlist to restrict access to the production database. The following rules apply:

- Only the API hosting provider’s outbound IPs are allowed to connect to the production branch.
- Local developer IPs may be added temporarily if needed.
- The default “Allow all IPs” rule is removed.
- Protected branches may be enabled to prevent accidental deletion.

This ensures the production database is not publicly accessible and satisfies the security requirements of US‑141.

---

## Backup Strategy

Neon provides built‑in backup and restore capabilities that require no manual maintenance on the free tier.

### Point‑in‑Time Restore (PITR)

- Neon retains WAL (write‑ahead log) data for 7 days.
- The database can be restored to any point in time within that window.
- PITR is available through the Neon console under “Branches → Restore”.

This allows recovery from accidental data loss, corruption, or destructive migrations.

### Branch‑Based Snapshots

Neon branches act as zero‑cost snapshots:

- Creating a branch from `main` produces a copy‑on‑write snapshot.
- Branches can be used for:
  - backup snapshots
  - migration testing
  - rollback points
  - staging or preview environments

Branches consume storage only when data diverges.

### No Manual Backups Required

- Neon automatically manages WAL retention and PITR.
- No cron jobs, dump files, or manual exports are required.
- Long‑term archival can be added later using `pg_dump` if needed.

### Disaster Recovery Workflow

In the event of data corruption or accidental deletion:

1. Create a new branch from a PITR timestamp.
2. Promote that branch to replace the production branch.
3. Update the API connection string (handled in US‑140).

This provides a fast, reliable recovery path.

---

## Responsibilities

### Owned by US‑141 (Database Hosting)

- Provision Neon database
- Create application‑scoped users
- Restrict access via IP allowlist
- Document backup strategy
- Ensure migrations run successfully against Neon

### Owned by US‑140 (API Hosting)

- Store production connection string in hosting platform
- Ensure deployed API uses `app_production`
- Validate connectivity from hosting provider to Neon

---

## Summary

Neon provides a secure, low‑maintenance, and developer‑friendly hosting environment for the production database. With PITR, branch‑based snapshots, and IP‑restricted access, the application meets the operational and reliability requirements defined in US‑141.

