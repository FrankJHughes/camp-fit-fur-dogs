# Neon Database Hosting & Backup Strategy

This guide documents how the production database is hosted on Neon, how access is controlled, and how backups and restores are handled. It supports the Database Hosting story (**US‑141**) and provides the operational guarantees required for production reliability.

---

# Overview

The application uses **Neon PostgreSQL** as the hosted database provider for production. Neon provides:

- Free‑tier hosting suitable for early production  
- Point‑in‑time restore (PITR) for disaster recovery  
- Zero‑cost branching for snapshots, previews, and testing  
- Fine‑grained access control via IP allowlists and roles  
- GitHub integration for CI‑managed preview branches  

Local development continues to use **Docker PostgreSQL**.  
Production uses **Neon PostgreSQL**.

---

# Application Database Users

Neon uses role‑based access control. The following application‑scoped users exist:

- `app_local` — used only for local development  
- `app_production` — used by the deployed API  
- **Neon GitHub integration user** — used only for CI preview branches and migrations  

Each application user has:

- Permission to connect  
- Permission to use the `public` schema  
- CRUD permissions on all tables  
- Default privileges ensuring new tables are accessible  

Application users do **not** have DDL permissions.  
Schema changes are applied **only** through EF Core migrations.

This enforces **Security Governance** and **Least Privilege**.

---

# Access Restrictions

Neon provides an IP allowlist to restrict access to the production database.

Rules:

- Only the API hosting provider’s outbound IPs may connect to the production branch  
- Local developer IPs may be added temporarily  
- The default “Allow all IPs” rule is removed  
- Protected branches may be enabled to prevent accidental deletion  

This ensures the production database is **not publicly accessible** and satisfies **US‑141 security requirements**.

---

# Backup Strategy

Neon provides built‑in backup and restore capabilities that require **no manual maintenance** on the free tier.

## Point‑in‑Time Restore (PITR)

- Neon retains WAL (write‑ahead log) data for **7 days**  
- The database can be restored to **any point in time** within that window  
- PITR is available via the Neon console under:  
  **Branches → Restore**

This protects against:

- Accidental data loss  
- Corruption  
- Destructive migrations  

---

## Branch‑Based Snapshots

Neon branches act as **zero‑cost snapshots**:

- Creating a branch from `main` produces a copy‑on‑write snapshot  
- Branches can be used for:  
  - Backup snapshots  
  - Migration testing  
  - Rollback points  
  - Staging or preview environments  

Branches consume storage **only when data diverges**.

---

## No Manual Backups Required

- Neon automatically manages WAL retention and PITR  
- No cron jobs, dump files, or manual exports are required  
- Long‑term archival can be added later using `pg_dump` if needed  

This satisfies **Operations Governance** for early‑stage production.

---

# Disaster Recovery Workflow

If data corruption or accidental deletion occurs:

1. Create a new branch from a PITR timestamp  
2. Promote that branch to replace the production branch  
3. Update the API connection string (handled in **US‑140**)  

This provides a **fast, reliable, low‑risk** recovery path.

---

# Responsibilities

## Owned by US‑141 (Database Hosting)

- Provision Neon database  
- Create application‑scoped users  
- Restrict access via IP allowlist  
- Document backup strategy  
- Ensure migrations run successfully against Neon  

## Owned by US‑140 (API Hosting)

- Store production connection string in hosting platform  
- Ensure deployed API uses `app_production`  
- Validate connectivity from hosting provider to Neon  

---

# Summary

Neon provides a secure, low‑maintenance, developer‑friendly hosting environment for the production database.  
With:

- PITR  
- Branch‑based snapshots  
- IP‑restricted access  
- CI‑managed preview branches  

…the application meets the operational and reliability requirements defined in **US‑141**.

This guide reflects the **current**, **fully aligned**, **governance‑compliant** database hosting model.
