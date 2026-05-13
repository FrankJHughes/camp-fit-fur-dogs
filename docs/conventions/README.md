# Conventions

Authoritative system‑wide rules for code, architecture, workflows, documentation, and operational guardrails.

Conventions define the **non‑negotiable standards** that keep the system consistent, maintainable, and safe.  
All contributors must follow these conventions when writing code, documentation, workflows, or tests.

---

## Convention Categories

### Architecture Conventions

System‑wide architectural rules and guardrails.

- **[Architecture Conventions](architecture.md)** — DDD layering, hosting model, preview architecture, operational guardrails

### Workflow Conventions

CI/CD, PR Preview lifecycle, and automation rules.

- **[Workflow Conventions](workflow.md)** — CI/CD architecture, PR Preview lifecycle, composite action usage, workflow structure

### Code Conventions

Coding standards and purity rules.

- **[Code Conventions](code.md)** — coding standards, endpoint rules, guardrail enforcement

### Documentation Conventions

Documentation structure, formatting, and patching rules.

- **[Documentation Conventions](docs.md)** — documentation rules, Universal Patch Rule, formatting standards

---

## Purpose of Conventions

Conventions ensure:

- Predictable architecture  
- Consistent code quality  
- Deterministic CI/CD behavior  
- Safe and maintainable documentation  
- Clear onboarding for new contributors  
- Reduced cognitive load across the team  

Conventions are **living documents** and should be reviewed at each sprint close (see the Scrum Master Guide).

---

## Updating Conventions

Changes to conventions must:

1. Be discussed in a PR  
2. Reference the relevant ADR (if architectural)  
3. Follow the Universal Patch Rule  
4. Be reflected in all affected guides  

Conventions are **source‑of‑truth documents**.  
Guides and runbooks must conform to them.

---

## Related Documentation

- **[Guides Index](../guides/README.md)** — role guides and developer sub‑guides  
- **[ADR Index](../adr/README.md)** — architecture decision records  
- **[Governance](../governance/governance.md)** — roles, ceremonies, ADR process, CI governance  
