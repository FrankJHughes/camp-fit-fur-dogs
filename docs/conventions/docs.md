# Documentation Conventions

## Documentation Purpose

Documentation captures architecture, workflow, code, and process conventions.  
It exists to:

- Make decisions explicit.
- Reduce ambiguity.
- Keep the team aligned.
- Onboard new contributors quickly.

Docs are part of the product and must be kept up to date.

## Structure

Conventions are organized into four main documents:

- **Architecture** — layers, boundaries, core building blocks.
- **Workflow** — how work is done (TDD, debugging, patching, script‑first).
- **Code** — language‑ and stack‑specific coding rules.
- **Docs** — how documentation itself is structured and maintained.

Other documents (for example, ADRs) may reference these conventions.

## Architecture Decision Records

Significant architectural decisions should be captured as ADRs.

Each ADR should:

- Describe the context.
- State the decision.
- Explain the consequences.
- Reference relevant conventions when they refine or extend them.

ADRs live alongside the code they affect or in a central `docs/adr` folder.

## Documentation Lifecycle

When behavior or architecture changes, documentation must be updated as part of the same work.

- PRs that change conventions must include doc updates.
- Outdated documentation should be corrected, not left in place.
- Breaking changes must be reflected in both code and docs.

Docs should be treated as a first‑class artifact, not an afterthought.

## Safe Examples And Fencing

Examples in documentation must respect fencing and quoting rules defined in Workflow Conventions.

- Avoid nested fences and complex quoting that can break scripts or PR bodies.
- When showing syntax that conflicts with script‑generation rules, describe it in words instead of using literal characters.
- Prefer simple, copy‑pasteable examples.

This keeps documentation compatible with script‑first workflows and automation.

## Navigation And Ownership

Conventions should be easy to find.

- `docs/conventions/` is the hub for architecture, workflow, code, and docs.
- Each file should have clear headings and stable anchors.

Ownership:

- Architecture and SharedKernel conventions: typically platform or architecture group.
- Workflow and tooling conventions: team maintaining build and automation.
- Code conventions: teams working in those stacks (backend, frontend, infra).
- Docs conventions: shared responsibility, with a clear maintainer.

## Updating Conventions

When updating conventions:

- Follow the Universal Patch Rule (full‑file regeneration).
- Keep changes cohesive and well‑scoped.
- Explain rationale in the PR description.
- Ensure examples remain valid and safe under fencing and script‑first rules.

Conventions are living documents; they should evolve deliberately, not drift accidentally.

## Relationship To SharedKernel

SharedKernel conventions are part of the architecture and code story.

Documentation should:

- Make clear when a rule is enforced by SharedKernel types or helpers.
- Encourage product code to lean on SharedKernel instead of duplicating patterns.
- Highlight SharedKernel as the canonical source for cross‑cutting behavior.

This keeps the system coherent and reduces divergence over time.
