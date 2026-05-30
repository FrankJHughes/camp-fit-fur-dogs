# Conventions

Authoritative system‑wide rules for code, architecture, workflows, documentation, and operational guardrails.

Conventions define the **non‑negotiable standards** that keep the system consistent, maintainable, and safe.  
All contributors must follow these conventions when writing code, documentation, workflows, or tests.

Conventions describe **how the system is implemented**, not governance or policy.  
Governance lives under `docs/governance/` and defines **what must be true**.

---

# Convention Categories

The repository maintains four canonical convention documents.  
These are the single source of truth for implementation‑level rules and must remain synchronized.

---

## Architecture Conventions

System‑wide architectural implementation rules and guardrails.

- **[Architecture Conventions](architecture.md)** — DDD layering, hosting provider architecture, authentication/session architecture, endpoint architecture, test seams, SharedKernel boundaries

---

## Workflow Conventions

CI/CD, PR Preview lifecycle, and automation rules.

- **[Workflow Conventions](workflow.md)** — CI/CD architecture, PR Preview lifecycle, composite actions, dependency graph, script‑first workflow structure

---

## Code Conventions

Coding standards and purity rules for backend and frontend.

- **[Code Conventions](code.md)** — backend and frontend coding rules, CQRS usage, endpoint rules, EF Core mapping, form architecture, test seams, preview‑safe behavior

---

## Documentation Conventions

Documentation structure, formatting, and patching rules.

- **[Documentation Conventions](docs.md)** — documentation lifecycle, Universal Patch Rule, fencing/quoting rules, ADR integration, authorship standards

---

# Purpose of Conventions

Conventions ensure:

- Predictable architecture  
- Consistent code quality  
- Deterministic CI/CD behavior  
- Safe and maintainable documentation  
- Clear onboarding for new contributors  
- Reduced cognitive load across the team  
- Alignment with SharedKernel and governance  
- Preview‑safe behavior across all environments  

Conventions are **living documents** and should be reviewed at each sprint close.

---

# Updating Conventions

Changes to conventions must:

1. Be proposed and discussed in a PR  
2. Reference the relevant ADR (if architectural)  
3. Follow the **Universal Patch Rule**  
4. Update all affected convention files  
5. Update any impacted guides or runbooks  
6. Ensure guardrail tests remain aligned  

Conventions are **source‑of‑truth documents**.  
Guides and runbooks must conform to them.

---

# Related Documentation

- **[Guides Index](../guides/README.md)** — role guides and developer sub‑guides  
- **[ADR Index](../adr/README.md)** — architecture decision records  
- **[Governance](../governance/governance.md)** — roles, ceremonies, ADR process, CI governance, security governance, operations governance

