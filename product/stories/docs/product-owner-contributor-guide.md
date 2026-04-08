# Product Owner Contributor Guide

## User Story

As a Product Owner contributing to this project, I want a dedicated
guide that explains how to author stories, manage the backlog, define
acceptance criteria, and participate in sprint ceremonies — so that I
can drive product direction using the repo's established artifacts
and workflows without needing to understand the build system.

## Context

This project follows a story-first development model: every feature
begins as a product story (`product/stories/`) before becoming a
GitHub issue, sprint board item, or pull request. The story is the
source of truth for intent and acceptance.

Today, the story authoring workflow is undocumented. A Product Owner
would need to reverse-engineer the format from existing files or ask
a teammate. This story closes that gap with an accessible,
self-contained guide.

## Scope

- **docs/contributing/product-owner-guide.md** — the dedicated
  Product Owner reference.
- Linked from the CONTRIBUTING.md hub page.

## Acceptance Criteria

### Story Authoring

- [ ] Story file format documented: required sections (User Story,
      Context, Acceptance Criteria), optional sections (Dependencies,
      Open Questions, Decision Record link).
- [ ] Directory convention documented: `product/stories/<domain>/`
      with examples of existing domains (infra, docs).
- [ ] File naming convention documented: kebab-case matching the
      story title (e.g., `one-command-local-bootstrap.md`).
- [ ] Relationship between product story and GitHub issue explained:
      story is authored first, issue is created from the story, issue
      references the story file path.
- [ ] Story-to-ADR linking explained: when a story requires an
      architectural decision, the story references the ADR and the
      ADR references the story.

### Acceptance Criteria Patterns

- [ ] Guide explains how to write testable acceptance criteria:
      observable, specific, binary (pass/fail).
- [ ] Common anti-patterns documented with corrections:
      - Vague: "System is fast" → Specific: "API responds in < 200ms
        at p95 under load"
      - Implementation-prescriptive: "Use Redis" → Outcome-focused:
        "Cache reduces cold-start latency below target"
      - Untestable: "Easy to use" → Testable: "New developer completes
        onboarding in < 10 minutes"
- [ ] Explains the checkbox convention: criteria start unchecked in
      the story, checked during implementation or PR review.

### Backlog Management

- [ ] Backlog structure documented: epics → stories → tasks,
      with GitHub labels and milestones mapping.
- [ ] Prioritization guidance: how to sequence stories by dependency,
      value, and risk.
- [ ] Grooming workflow: how to refine stories (add context, sharpen
      criteria, resolve open questions) before sprint planning.
- [ ] Definition of Ready documented: what makes a story ready to
      enter a sprint (acceptance criteria defined, dependencies
      identified, sized by team).

### Sprint Participation

- [ ] Sprint planning role documented: presenting candidate stories,
      answering clarifying questions, confirming sprint goal.
- [ ] Sprint review role documented: verifying acceptance criteria
      against delivered work, providing feedback.
- [ ] How to request a spike or research story when the path forward
      is unclear.
- [ ] How to handle mid-sprint scope changes: when to add, when to
      defer, how to communicate.

### Linking Artifacts

- [ ] Flowchart or ordered list showing the full artifact lifecycle:
      Product Story → GitHub Issue → Sprint Board → Branch → PR →
      ADR (if applicable) → Merge → Issue Closed.
- [ ] Each artifact's purpose and ownership explained:
      - Product Story: owned by PO, defines intent
      - GitHub Issue: owned by PO, tracks execution
      - ADR: owned by developer, records architectural decisions
      - PR: owned by developer, delivers implementation.

## Dependencies

- Developer Contributor Guide story (for CONTRIBUTING.md hub
  structure).

## Open Questions

- Should the guide include a story template file that the PO can
  copy (`product/stories/_template.md`)?
- Should there be a validation check (CI or script) that ensures
  every GitHub issue references a product story file?