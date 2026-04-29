# Operations Governance

This document defines how the system is operated in hosted environments, how deployments behave, how reliability is maintained, and how incidents are handled.  
It complements (but does not duplicate) Security Governance, CI Governance, and the hosting stories (US‑139, US‑140, US‑141, US‑142).

Operations governance ensures:

- Predictable deployments  
- Stable hosting environments  
- Safe configuration management  
- Clear operational responsibilities  
- Reliable customer experience  
- Zero‑surprise behavior in production  

Operations is a product surface — not an afterthought.

---

# 1. Operational Principles

Operations must follow these principles:

- **Predictability** — deployments behave the same every time  
- **Observability** — failures are visible and diagnosable  
- **Safety** — no operational action may risk data loss  
- **Reversibility** — every deployment must be reversible  
- **Zero Surprises** — customers should never experience unexpected downtime  
- **Separation of Concerns** — hosting, deployment, and configuration are isolated  

Operations is governed by process, not improvisation.

---

# 2. Hosting Governance

Hosting is defined by the M2 infrastructure stories:

- **US‑139** — Frontend Hosting  
- **US‑140** — API Hosting  
- **US‑141** — Database Hosting  
- **US‑142** — Custom Domain & SSL  

Governance rules:

- Hosting platforms must be free‑tier or zero‑cost unless approved otherwise  
- Hosting must support HTTPS  
- Hosting must support environment‑based configuration  
- Hosting must not require manual steps after initial setup  
- Hosting must support rollback  
- Hosting must expose logs for debugging  
- Hosting must not expose internal ports or services  

Frontend and API hosting must be independent but coordinated.

---

# 3. Deployment Governance

Deployments must:

- Be triggered automatically on merge to `main`  
- Be reproducible  
- Use deterministic build steps  
- Use environment variables for configuration  
- Never include secrets in source control  
- Never require manual intervention  
- Produce clear logs on success or failure  

Pull requests must generate preview deployments when supported by the platform.

Rollback must be possible without code changes.

---

# 4. Configuration Governance

Configuration must:

- Use environment variables  
- Never be stored in `.env` files committed to source  
- Never be stored in frontend bundles  
- Never be logged  
- Be documented in a single configuration reference file (future)  

Configuration categories:

- API secrets  
- Database connection strings  
- Frontend environment variables  
- Feature flags  
- Hosting platform settings  

Configuration drift is prohibited.

---

# 5. Database Operations Governance

Database hosting (US‑141) must follow:

- Free‑tier hosting  
- Secure connection strings  
- EF Core migrations run automatically or via CI/CD  
- No destructive migrations without explicit approval  
- No manual schema edits  
- No direct production database access except through approved tools  

Backups:

- Must be enabled if the platform supports them  
- Must be restorable  
- Must not require manual export/import workflows  

---

# 6. Monitoring & Observability Governance

The system must provide:

- Application logs  
- Deployment logs  
- Health checks  
- Error visibility  
- Performance visibility (where supported)  

Health checks:

- `/health` endpoint must return 200 when the API is operational  
- Frontend must expose a simple health indicator (static page load)  

Monitoring must not expose sensitive data.

---

# 7. Reliability Governance

Reliability expectations:

- Deployments must not break existing functionality  
- Cold‑start behavior must be documented  
- Downtime must be minimized  
- Rollbacks must be fast  
- Hosted environments must remain within free‑tier limits  

Operational risks must be documented and mitigated.

---

# 8. Incident Response Governance

When an incident occurs:

## 8.1 Immediate Actions
- Stop the bleeding  
- Disable affected features if necessary  
- Revoke compromised tokens  
- Block deployments if needed  

## 8.2 Diagnosis
- Identify root cause  
- Identify affected users  
- Identify affected systems  
- Determine severity  

## 8.3 Remediation
- Patch the issue  
- Add tests to prevent regression  
- Update documentation  
- Update changelog  

## 8.4 Communication
- Notify stakeholders  
- Document the incident  
- Record follow‑up actions  

Incidents must be treated as top‑priority work.

---

# 9. Operational Boundaries

Operations must not:

- Modify code directly in production  
- Apply manual hotfixes  
- Change configuration without version control  
- Deploy from local machines  
- Use untracked scripts  
- Introduce undocumented infrastructure changes  

All operational changes must be:

- Scripted  
- Documented  
- Reproducible  
- Reviewable  

---

# 10. Governance Enforcement

- CI enforces deployment safety  
- Reviewers enforce operational rules  
- Product Owner enforces EG/LG alignment  
- Scripts enforce deterministic behavior  
- Hosting platforms enforce environment isolation  

No PR may merge if it violates operational governance.

