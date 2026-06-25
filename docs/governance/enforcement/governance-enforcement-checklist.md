# Camp Fit Fur Dogs — Governance — Enforcement — Checklist

A unified checklist for reviewers, CI, Product Owner, and automation to ensure all governance rules are upheld across the repository.  
This checklist is the **operational enforcement layer** for the entire Governance Hub.

Governance defines **process, responsibilities, boundaries, and enforcement**.  
This checklist defines **how governance is verified**.

---

# 1. Story Governance Enforcement

- Story uses correct grammar:  
  **As a <role>, I must/should be able to <verb>… so that <value>…**
- Role is valid (Owner, Staff, Admin)  
- Story file lives under correct category folder  
- Story includes acceptance criteria  
- Story includes EG/LG alignment  
- Story includes Covey quadrant  
- Story references dependencies correctly  
- Story status transitions follow lifecycle rules  
- Story changes update catalog and changelog  
- Story is referenced by tasks and PRs  
- Story is not merged without Product Owner approval  
- Story does not violate product boundaries  
- Story does not duplicate existing functionality  
- Story is testable and vertical‑slice compatible  
- Story is linked in PR via **Story Reference**  
- Story includes observability acceptance criteria when applicable (NEW)

---

# 2. Changelog Governance Enforcement

- Every user‑facing change updates the changelog  
- Changelog entries follow required format  
- Multi‑product changes are separated by product  
- Changelog entries reference stories and tasks  
- No PR merges without changelog coverage  
- Release grouping rules are followed  
- No drift between changelog and catalog  
- Changelog is updated in the same PR as the change  
- Changelog entries are written in user‑facing language  
- Changelog is validated by CI and reviewers  
- Observability‑related changes include event/metric naming updates (NEW)

---

# 3. Repository Hygiene Enforcement

- No untracked files or directories  
- No drift between stories, catalog, changelog, CI, or docs  
- No stale or orphaned stories  
- No missing frontmatter  
- No broken links in docs  
- No unused scripts  
- No cross‑product contamination  
- No forbidden directory structures  
- No missing or invalid metadata  
- Hygiene scripts pass before merge  
- CI validates hygiene via repo‑hygiene rules  
- Observability conventions remain consistent across code, docs, and events (NEW)

---

# 4. Multi‑Product Governance Enforcement

- Camp Fit Fur Dogs must not depend on Frank internals  
- Frank must not depend on product code  
- Product boundaries are respected in code, stories, and CI  
- Milestones remain independent  
- Releases remain independent  
- No cross‑product coupling in migrations, workflows, or scripts  
- CI enforces dependency direction  
- Reviewers enforce product boundaries  
- Frank changes require architectural review  
- Product code must use Frank primitives, not reimplement them  
- Observability primitives must come from Frank only (NEW)

---

# 5. CI Governance Enforcement

- Required checks must pass  
- Path‑based test skipping is correct  
- Dependency graph is valid  
- Composite actions are used correctly  
- No inline complex shell logic  
- Nightly runs execute full suite  
- CI does not leak secrets  
- CI uses pinned versions  
- CI enforces deterministic behavior  
- CI enforces preview‑safe rules  
- CI enforces governance via ci‑governance.md  
- CI validates observability propagation, event emission, and metric emission (NEW)  
- CI blocks merges on forbidden observability patterns (NEW)

---

# 6. Security Governance Enforcement

- No plaintext secrets in repo  
- No secrets in logs  
- No secrets in commit messages  
- OIDC configuration validated  
- Session cookies follow security rules  
- Authorization is explicit  
- Endpoints declare required roles/permissions  
- No bypassing validation pipeline  
- No insecure defaults  
- No sensitive data in error messages  
- Dependency scanning passes  
- Security headers are present  
- CORS policy is enforced  
- Hosting provider hardening rules enforced  
- Incident response rules followed  
- Security governance overrides conventions when needed  
- PRs touching auth/session require security review  
- Security‑relevant observability events are emitted and validated (NEW)

---

# 7. Contributor Governance Enforcement

- PR includes story reference  
- PR includes task reference  
- PR includes changelog entry  
- PR updates docs when behavior changes  
- PR follows Universal Patch Rule for docs  
- PR follows code conventions  
- PR follows architecture conventions  
- PR follows workflow conventions  
- PR follows documentation conventions  
- PR does not violate product boundaries  
- PR is reviewed by appropriate maintainers  
- PR is approved by Product Owner when required  
- PR includes rationale for governance‑level changes  
- PR includes observability updates when behavior changes (NEW)

---

# 8. Operations Governance Enforcement

- Hosting provider selection follows first‑active‑wins  
- Hosting provider configuration validated  
- No partial configuration allowed  
- Startup fails fast on misconfiguration  
- Deployment is reproducible  
- No manual hotfixes  
- No local‑machine deployments  
- No untracked scripts  
- No configuration drift  
- Monitoring and health checks implemented  
- Preview environments follow lifecycle rules  
- Neon branch provisioning validated  
- Render preview deployment validated  
- Vercel preview deployment validated  
- Required artifacts (`db-conn.txt`, `frontend-url.txt`) present  
- Operational changes documented and reviewed  
- Operations governance overrides workflow conventions when needed  
- Hosting and deployment observability events are emitted and validated (NEW)

---

# 9. Architecture & DI Enforcement

- No manual DI registration of slice services  
- All slice services use `[AutoRegister]`  
- No Scrutor or suffix‑based scanning  
- Frank DI auto‑registration must pass  
- EF Core configuration scanning must pass  
- Endpoint discovery must pass  
- No API → Infrastructure dependencies  
- No handler invocation from endpoints  
- No domain mutation outside Application layer  
- Architecture tests must pass  
- Dependency direction must be preserved  
- Observability context must propagate through all layers (NEW)  
- Observability events/metrics must follow naming conventions (NEW)

---

# 10. Observability Governance Enforcement (NEW)

- All layers propagate `IObservabilityContext`  
- No manual correlation ID creation  
- No ad‑hoc logging  
- No vendor‑specific logging/metrics APIs  
- All events follow `slice.module.action` naming  
- All metrics follow `slice.module.metric_name` naming  
- No secrets, tokens, or PII in observability payloads  
- Callback endpoints emit required observability events  
- Infrastructure emits external call events/metrics  
- Application emits use‑case boundary events/metrics  
- API emits request boundary events/metrics  
- CI validates observability determinism  
- Observability tests must pass before merge  
- Observability conventions remain consistent across code and docs  
- Observability governance overrides workflow conventions when needed  

---

# 11. Governance Process Enforcement

- Governance changes require Product Owner approval  
- Governance changes include rationale and consequences  
- Governance changes update conventions if needed  
- Governance changes update scripts if needed  
- Governance changes follow Universal Patch Rule  
- Governance remains minimal and intentional  
- Governance documents remain internally consistent  
- Governance changes are recorded in ADRs when architectural  
- Observability governance changes require Frank review (NEW)

---

# 12. Enforcement Roles

- **Product Owner** — approves governance changes, enforces story/changelog rules  
- **Reviewers** — enforce hygiene, CI, contributor governance, product boundaries  
- **CI** — enforces structural rules, metadata validation, dependency direction, observability correctness  
- **Scripts** — enforce deterministic behavior, prevent drift  
- **Frank** — enforces architectural guardrails (DI, EF Core, hosting providers, observability primitives)  
- **Preview Pipeline** — enforces hosting, configuration, operational safety, and observability correctness  

---

# Summary

This checklist ensures:

- Governance is consistently enforced  
- Conventions remain aligned with governance  
- CI and scripts uphold structural and observability rules  
- Reviewers enforce boundaries and responsibilities  
- Product Owner maintains strategic control  
- The system remains stable, predictable, observable, and maintainable  

Governance is the backbone of the repository.  
This checklist is how it stays enforced.
