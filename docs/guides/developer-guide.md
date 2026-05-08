# Developer Guide

> Onboarding guide and central reference for contributors.

## Quick Links

- [Actions Index](actions-index.md) — catalog of all custom GitHub Actions
  and conventions for adding new ones.
- [Action README Template](action-readme-template.md) — canonical format for
  documenting a custom action.
- [Workflow Conventions](../../conventions/workflow.md) — structure and rules
  for GitHub Actions workflows.

## Getting Started

1. **Clone the repository** and install dependencies according to the root
   `README.md`.
2. **Review the conventions** listed under Quick Links before making changes
   to workflows or actions.
3. **Follow the PR Preview workflow** documented in
   [Workflow Conventions](../../conventions/workflow.md) to understand how
   pull requests are built, deployed, and validated.

## Repository Layout

```text
.github/
  actions/          # Reusable composite actions (see Actions Index)
  workflows/        # CI/CD workflow definitions
docs/
  conventions/      # Project-wide conventions and standards
  guides/
    developer/      # This guide and related developer references
```

## Contributing

- All new custom actions must include a `README.md` created from the
  [Action README Template](action-readme-template.md).
- All new actions must be registered in the
  [Actions Index](actions-index.md).
- Workflow changes should be documented in
  [Workflow Conventions](../../conventions/workflow.md).
- Follow the Universal Patch Rule: regenerate entire files rather than
  applying partial patches to avoid merge conflicts and drift.
- All files must be saved as UTF-8 without BOM.
