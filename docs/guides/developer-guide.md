# Developer Guide

> Onboarding guide and central reference for contributors.

## Quick Links

- [Actions Index](actions-index.md) — catalog of all custom GitHub Actions and their responsibilities.
- [Action README Template](action-readme-template.md) — canonical format for documenting composite actions.
- [Workflow Conventions](../../conventions/workflow.md) — CI/CD architecture, PR Preview lifecycle, composite action usage, and workflow structure.
- [Architecture Conventions](../../conventions/architecture.md) — system architecture, hosting model, preview architecture, and operational guardrails.
- [Code Conventions](../../conventions/code.md) — coding standards, endpoint rules, and guardrail enforcement.
- [Documentation Conventions](../../conventions/docs.md) — documentation rules, Universal Patch Rule, and formatting standards.

## Getting Started

1. **Clone the repository** and install dependencies according to the root `README.md`.
2. **Review the conventions** listed under Quick Links before making changes to code, workflows, or actions.
3. **Understand the PR Preview lifecycle** by reading  
   [Workflow Conventions](../../conventions/workflow.md).  
   This explains how Neon branches, Render previews, and Vercel previews are created, validated, and destroyed.
4. **Review the Architecture Conventions** to understand the system’s DDD layering, hosting model, and operational guardrails.

## Repository Layout

```text
.github/
  actions/          # Reusable composite actions (see Actions Index)
  workflows/        # CI/CD workflow definitions (ci.yaml, preview.yaml)
docs/
  conventions/      # Project-wide conventions and standards
  guides/
    developer/      # This guide and related developer references
src/
  ...               # Backend, frontend, and SharedKernel source code
integration-tests/
  ...               # Infrastructure and API integration test suites
```

## Contributing

### Composite Actions

- All new custom actions must include a `README.md` created from the  
  [Action README Template](action-readme-template.md).
- All new actions must be registered in the  
  [Actions Index](actions-index.md).
- Composite actions must:
  - Use `using: composite`
  - Have minimal surface area
  - Pin dependencies to full SHAs
  - Include clear inputs, outputs, failure modes, and usage examples
  - Be testable in isolation

### Workflows

- All workflow changes must be documented in  
  [Workflow Conventions](../../conventions/workflow.md).
- Workflows must follow:
  - The **Universal Patch Rule** (regenerate entire files)
  - UTF‑8 without BOM
  - Deterministic behavior (no nondeterministic sleeps, no implicit retries)
  - Correct use of composite actions
  - Correct teardown/readiness rules for PR previews

### Code & Architecture

- Follow the DDD layering rules defined in  
  [Architecture Conventions](../../conventions/architecture.md).
- All backend code must respect:
  - Domain purity
  - CQRS pipelines
  - SharedKernel primitives and guardrails
- All frontend code must follow the layer + aggregate structure defined in the architecture guide.

### Documentation

- All documentation must follow  
  [Documentation Conventions](../../conventions/docs.md).
- All files must be saved as **UTF‑8 without BOM**.
- All updates must follow the **Universal Patch Rule** to avoid drift.

---

## Additional Developer References

- **Preview Troubleshooting Guide** (if present) — diagnosing issues with Neon branches, Render previews, or Vercel deployments.
- **CI Dependency Graph Guide** (if present) — understanding and maintaining the CI dependency validator.

---

## Summary

This Developer Guide provides the entry point for contributors.  
All development work should follow:

- System architecture rules  
- Workflow and automation conventions  
- Composite action standards  
- Documentation and patching rules  

Use the Quick Links above to navigate the canonical references for each area of the system.
