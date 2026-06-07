# Operations Guides

This folder contains documentation related to **running, hosting, configuring, and troubleshooting** the CampFitFurDogs system across all environments.

These guides are operational, not architectural.  
They help developers and operators understand how to run the system locally, in preview, and in production.

---

# Contents

- **[API Hosting](../api-hosting.md)**  
  How the API is hosted locally, in preview, and in production.

- **[Database Hosting](../db-hosting.md)**  
  How the database is provisioned, migrated, and managed across environments.

- **[Authentication Operations](../authentication-operations.md)**  
  How Auth0, callback URLs, environment variables, and session cookies are configured and operated.

- **[Local Development](../local-development.md)**  
  How to run the system locally, including environment setup and developer workflows.

- **[Preview Troubleshooting](../preview-troubleshooting.md)**  
  How to diagnose and fix issues in preview environments.

- **[Secrets Setup](../secrets-setup.md)**  
  How to configure environment variables and manage secrets safely.

- **[Environment Variables Reference](../environment-variables.md)**  
  Complete reference for all environment variables across local, preview, and production.

- **[Hosting Provider Operations](../hosting-provider-operations.md)**  
  How hosting providers (Render, Neon, Vercel) behave operationally and how to debug them.

- **[PR Preview Operations](../preview-operations.md)**  
  How PR Previews are created, destroyed, validated, and tested.

- **[Migrations & Database Operations](../migration-operations.md)**  
  How to run, validate, and troubleshoot EF Core migrations in all environments.

---

# Purpose

These guides help developers:

- Run the system locally  
- Understand hosting architecture  
- Operate authentication and session flows  
- Troubleshoot preview deployments  
- Configure secrets and environment variables  
- Understand operational constraints across environments  
- Debug hosting provider behavior  
- Reproduce CI/preview issues locally  
- Manage migrations safely and deterministically  

---

# Operational Scope

Operations Guides cover:

- Local development workflows  
- Environment variable setup  
- Hosting provider behavior  
- PR Preview lifecycle  
- Database provisioning and migrations  
- Authentication configuration  
- Troubleshooting patterns  
- Secrets management  
- Operational safety rules  

Operations Guides do **not** define:

- Architecture rules  
- Code conventions  
- Workflow conventions  
- Governance rules  

They describe **how to operate the system**, not how to design or implement it.

---

# Operational Principles

Operations across all environments follow these principles:

- **Deterministic** — reproducible locally and in CI  
- **Script‑first** — no manual steps when automation is possible  
- **Environment‑safe** — no environment‑specific branching in product code  
- **Preview‑safe** — PR Previews must be isolated and ephemeral  
- **Secret‑safe** — no secrets in logs, artifacts, or source control  
- **Provider‑agnostic** — hosting providers are interchangeable via Frank abstractions  

---

# Related Documentation

- [Authentication Configuration](ca://s?q=Show_authentication_configuration_doc)  
- [CI/CD Guides](ca://s?q=Show_ci_cd_docs)  
- [Session Management](ca://s?q=Generate_Session_Management_Guide)  
- [Identity Mapping](ca://s?q=Generate_Identity_Mapping_Guide)  
- [Workflow Conventions](ca://s?q=Open_Workflow_Conventions)  
- [Architecture Conventions](ca://s?q=Open_Architecture_Conventions)
