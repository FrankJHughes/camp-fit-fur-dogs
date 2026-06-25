# Camp Fit Fur Dogs — Governance — Technical — Operations

This document defines how the system is operated in hosted environments, how deployments behave, how reliability is maintained, how observability is surfaced, and how incidents are handled.  
It complements (but does not duplicate) Security Governance, CI Governance, Observability Governance, and the hosting stories (US‑139, US‑140, US‑141, US‑142).

Operations governance ensures:

- Predictable deployments  
- Stable hosting environments  
- Safe configuration management  
- Clear operational responsibilities  
- Reliable customer experience  
- Deterministic observability  
- Zero‑surprise behavior in production  

Operations is a product surface — not an afterthought.

---

# 1. Operational Principles

Operations must follow these principles:

- **Predictability** — deployments behave the same every time  
- **Observability** — failures are visible, structured, correlated, and diagnosable  
- **Safety** — no operational action may risk data loss  
- **Reversibility** — every deployment must be reversible  
- **Zero Surprises** — customers should never experience unexpected downtime  
- **Separation of Concerns** — hosting, deployment, and configuration are isolated  
- **Fail Fast** — misconfigured environments must prevent startup  
- **Determinism** — operational behavior must not depend on hidden state  
- **Traceability** — all operational actions must be observable and auditable  

Operations is governed by process, not improvisation.

---

# 2. Hosting Governance

Hosting is defined by the M2 infrastructure stories:

- US‑139 — Frontend Hosting  
- US‑140 — API Hosting  
- US‑141 — Database Hosting  
- US‑142 — Custom Domain & SSL  

Hosting governance rules:

- Hosting platforms must be free‑tier or zero‑cost unless approved otherwise  
- Hosting must support HTTPS  
- Hosting must support environment‑based configuration  
- Hosting must not require manual steps after initial setup  
- Hosting must support rollback  
- Hosting must expose logs for debugging  
- Hosting must expose structured, correlated observability output  
- Hosting must not expose internal ports or services  
- Hosting providers must be hardened and must abort startup if configuration cannot be applied  
- Hosting provider selection must follow first‑active‑wins semantics  
- Hosting provider infrastructure must be implemented in Frank  
- Hosting providers must use Frank’s injected abstractions:
  - `IEnvironment`  
  - `IRenderPrParser`  
  - `IGitHubArtifactClient`  
  - `IRenderConfigurationWriter`  
  - `IObservabilityContext`  

Frontend and API hosting must be independent but coordinated.  
Hosting provider behavior must be fully observable.

---

# 3. Deployment Governance

Deployments must:

- Be triggered automatically on merge to `main`  
- Be reproducible  
- Use deterministic build steps  
- Use environment variables for configuration  
- Never include secrets in source control  
- Never require manual intervention  
- Produce clear, structured, correlated logs on success or failure  
- Emit deployment‑level observability events  
- Fail fast if required configuration is missing  
- Validate hosting provider configuration before application startup  
- Validate DI auto‑registration and EF Core configuration scanning at startup  
- Validate security headers middleware is active  
- Validate StartupModule Engine ordering and startup behavior  
- Validate observability context propagation at startup  

Pull requests must generate preview deployments when supported by the platform.  
Rollback must be possible without code changes.

---

# 4. Configuration Governance

Configuration must:

- Use environment variables  
- Never be stored in `.env` files committed to source  
- Never be stored in frontend bundles  
- Never be logged  
- Never appear in observability payloads  
- Be validated at startup  
- Fail fast if required configuration is missing or invalid  

Configuration categories:

- API secrets  
- Database connection strings  
- Frontend environment variables  
- Feature flags  
- Hosting platform settings  
- OIDC authentication settings  
- Render PR preview artifact settings  

Configuration drift is prohibited.  
Configuration validation must emit structured configuration events.

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

Database configuration must be validated at startup.  
Missing or invalid connection strings must prevent the API from starting.

Database operations must emit:

- Migration events  
- Connection validation events  
- Failure events  
- Retry events  

All database observability must be structured and correlated.

---

# 6. Monitoring & Observability Governance

The system must provide:

- Application logs  
- Deployment logs  
- Health checks  
- Error visibility  
- Performance visibility (where supported)  
- Structured, correlated observability events  
- Metrics for latency, success/failure, and startup behavior  

Health checks:

- `/health` endpoint must return 200 when the API is operational  
- Frontend must expose a simple health indicator (static page load)  

Monitoring must not expose sensitive data.

Operational logs must include:

- Hosting provider selection  
- Hosting provider configuration success/failure  
- Authentication flow errors  
- Startup failures  
- DI auto‑registration validation failures  
- EF Core configuration scanning failures  
- Security header validation failures  
- StartupModule Engine ordering and execution logs  
- Observability context creation and propagation  
- External call failures  
- Outbox processing failures  

Observability output must follow Frank’s conventions:

- Events: `slice.module.action`  
- Metrics: `slice.module.metric_name`  
- Correlation: `IObservabilityContext`  

No operational log may contain:

- Secrets  
- Tokens  
- PII  
- Provider error messages  
- Raw exceptions  

---

# 7. Reliability Governance

Reliability expectations:

- Deployments must not break existing functionality  
- Cold‑start behavior must be documented  
- Downtime must be minimized  
- Rollbacks must be fast  
- Hosted environments must remain within free‑tier limits  
- Hosting provider failures must be surfaced immediately  
- Misconfigured preview environments must fail fast rather than run partially configured  
- Observability must surface failures deterministically  

Operational risks must be documented and mitigated.

---

# 8. Hosting Provider Hardening Governance

Hosting providers must:

- Validate all required environment variables  
- Validate all required external configuration sources  
- Fail fast if configuration cannot be applied  
- Never silently skip required configuration  
- Never allow the API to start in an insecure or incomplete state  
- Use Frank hosting provider infrastructure  
- Use Frank’s injected abstractions exclusively  
- Never perform HTTP, JSON, or ZIP operations directly  
- Emit structured hosting provider events  
- Emit configuration validation events  

## 8.1 Render Hosting Provider Requirements

Render PR preview environments must provide:

- `IS_PULL_REQUEST`  
- `RENDER_GIT_REPO_SLUG`  
- `RENDER_SERVICE_NAME`  
- `GITHUB_PAT`  

Render must also provide GitHub Actions artifacts:

- `pr-XXX-db/db-conn.txt`  
- `pr-XXX-frontend/frontend-url.txt`  

If any required artifact or variable is missing:

- Startup must abort  
- A clear, actionable error must be thrown  
- A structured failure event must be emitted  
- No partial configuration is allowed  

Render hosting provider behavior must be fully deterministic and observable.

---

# 9. PR Preview Environment Governance

PR preview environments must:

- Use Neon branch provisioning  
- Apply EF Core migrations deterministically  
- Run infrastructure integration tests  
- Deploy API to Render  
- Deploy frontend to Vercel  
- Validate readiness via health probes  
- Fail fast on misconfiguration  
- Publish deterministic artifacts (`db-conn.txt`, `frontend-url.txt`)  
- Use Frank hosting provider abstractions for configuration  
- Use Frank security headers and error boundary middleware  
- Use StartupModule Engine for startup ordering and validation  
- Emit structured preview environment events  

Preview environments must be fully reproducible and observable.

---

# 10. Incident Response Governance

When an incident occurs:

## 10.1 Immediate Actions
- Stop the bleeding  
- Disable affected features if necessary  
- Revoke compromised tokens  
- Block deployments if needed  
- Emit an incident‑start event  

## 10.2 Diagnosis
- Identify root cause  
- Identify affected users  
- Identify affected systems  
- Determine severity  
- Emit structured diagnostic events  

## 10.3 Remediation
- Patch the issue  
- Add tests to prevent regression  
- Update documentation  
- Update changelog  
- Emit incident‑resolved event  

## 10.4 Communication
- Notify stakeholders  
- Document the incident  
- Record follow‑up actions  

Incidents must be treated as top‑priority work.

---

# 11. Operational Boundaries

Operations must not:

- Modify code directly in production  
- Apply manual hotfixes  
- Change configuration without version control  
- Deploy from local machines  
- Use untracked scripts  
- Introduce undocumented infrastructure changes  
- Allow hosting providers to run without validation  
- Allow the API to start with missing or invalid configuration  
- Bypass Frank startup validation  
- Bypass Frank security headers or error boundary middleware  
- Emit unstructured logs  
- Emit logs without correlation IDs  
- Use vendor‑specific logging or metrics APIs  

All operational changes must be:

- Scripted  
- Documented  
- Reproducible  
- Reviewable  
- Observable  

---

# 12. Governance Enforcement

- CI enforces deployment safety  
- Reviewers enforce operational rules  
- Product Owner enforces EG/LG alignment  
- Scripts enforce deterministic behavior  
- Hosting platforms enforce environment isolation  
- Frank enforces startup validation  
- Frank guardrails enforce architectural boundaries  
- Security header enforcement tests validate API safety  
- Observability tests validate correlation, event emission, and metric emission  

No PR may merge if it violates operational governance.

Operations governance is non‑negotiable.
