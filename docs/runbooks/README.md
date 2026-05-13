# Runbooks

Runbooks are **operational, step-by-step procedures** for running, maintaining, and recovering the Camp Fit Fur Dogs system.

Runbooks are **not** conventions, guides, or architectural documents — they are **action playbooks** used during real operational tasks.

---

## Purpose

Runbooks exist to:

- Provide **repeatable operational procedures**
- Reduce cognitive load during incidents
- Ensure consistent execution across the team
- Capture institutional knowledge in a durable format
- Support onboarding for new contributors

Runbooks are **procedural**, not explanatory.

For rules and standards, see:

- **../conventions/architecture.md**
- **../conventions/workflow.md**
- **../conventions/code.md**

For explanations and conceptual material, see:

- **../guides/README.md**

---

## Template

Use the standard runbook template when creating new runbooks:

- **[runbook-template.md](runbook-template.md)**

---

## When to Add a New Runbook

Create a runbook when:

- A procedure is repeated often
- A task is operational, not architectural
- A failure mode requires a documented response
- A developer or operator would benefit from a checklist

Runbooks should be:

- Step-by-step
- Actionable
- Script-first
- Minimal in explanation
- High in clarity

---

## When *Not* to Add a Runbook

Do **not** create a runbook for:

- Conventions → belongs in docs/conventions/
- Architecture → belongs in docs/conventions/architecture.md
- Guides → belongs in docs/guides/
- Story writing → belongs in Product Owner Guide
- CI/CD rules → belongs in Workflow Conventions

Runbooks are for **operations**, not **rules** or **design**.