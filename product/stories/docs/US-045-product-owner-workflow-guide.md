---
id: US-045
title: "Product Owner Workflow Guide"
epic: ""
milestone: ""
status: shipped
domain: docs
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US-045 — Product Owner Workflow Guide

## User Story

As a product owner joining the project, I want a single guide
that documents how to write stories, manage the backlog, apply
emotional guarantees, and run refinement — so that product
decisions follow a consistent, repeatable process without
tribal knowledge.

## Context

The project uses a story-first approach where markdown files in
`product/stories/` are the single source of truth for the backlog.
Stories follow naming conventions (ADR-0009), tag emotional
guarantees, and must pass the definition of ready before sprint
commitment. No documentation currently explains this workflow
end-to-end.

US-010 (PO Contributor Guide) was absorbed into US-022 during
Sprint 2, but US-022 focused on the planning runbook — the
standalone PO workflow guide was never produced.

## Scope

This guide lives at `docs/guides/product-owner-guide.md` and is
linked from the CONTRIBUTING.md role-routing hub (created by US-009).

## Acceptance Criteria

### Story Authoring

- [x] Story file format documented: heading, user story,
      value/context, acceptance criteria, emotional guarantees,
      dependencies, open questions.
- [x] Story naming convention explained with examples
      (ADR-0009: `US-NNN-kebab-title.md`).
- [x] Domain directory structure explained
      (`product/stories/customer/`, `docs/`, `infra/`).
- [x] Acceptance criteria writing guidelines: observable, testable,
      no implementation details.

### Backlog Management

- [x] How to add a new story to the backlog (create file, choose
      domain directory, next available US number).
- [x] How to retire, absorb, or split stories — with examples
      from project history.
- [x] How to update story scope after refinement — what changes
      are acceptable vs requiring a new story.

### Emotional Guarantees

- [x] Full list of emotional guarantees (EG-01 through EG-07)
      with descriptions and examples.
- [x] How to tag stories with relevant guarantees.
- [x] How emotional guarantees become acceptance criteria on
      feature stories (DoD integration).

### Definition of Ready

- [x] All 8 DoR criteria listed and explained with pass/fail
      examples.
- [x] How to run a readiness check before sprint planning.

### Refinement and Prioritization

- [x] When and how to run backlog refinement.
- [x] How to identify stories that need grooming (scope unclear,
      missing AC, stale dependencies).
- [x] How milestones inform prioritization — capability goals
      vs sprint timeboxes.

### Milestones

- [x] Difference between milestones and sprints explained.
- [x] How to assign stories to milestones.
- [x] How to propose new milestones or reorganize existing ones.

## Dependencies

- US-009 (Developer Contributor Guide) creates the CONTRIBUTING.md
  hub that routes to this guide.

## Open Questions

- Should the guide include a decision flowchart for story splitting?
- Should refinement checklists be a separate runbook or inline?

