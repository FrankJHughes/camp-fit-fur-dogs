# Scrum Master Contributor Guide

## User Story

As a Scrum Master facilitating this project, I want a dedicated guide
that documents sprint board management, ceremony facilitation,
governance artifacts, and the Definition of Ready / Definition of
Done — so that I can protect the team's process and remove impediments
using the repo's established workflows.

## Context

This project uses GitHub Projects as its sprint board, GitHub Issues
as work items, and a living governance model built on ADRs, merge
checklists, and product stories. The Scrum Master role spans process
facilitation and governance enforcement — but none of this is
documented in a way that a new SM could pick up and run.

## Scope

- **docs/guides/scrum-master-guide.md** — the dedicated
  Scrum Master reference.
- Linked from the CONTRIBUTING.md hub page.

## Acceptance Criteria

### Sprint Board Management

- [ ] GitHub Projects board documented: board number, columns/views,
      how items flow through statuses (Todo → In Progress → Done).
- [ ] How to add issues to the board, assign sprints, and set
      priority.
- [ ] How to read the board for standup: what to look at, what
      signals a blocked or stale item.
- [ ] Sprint milestone conventions: naming pattern, start/end dates,
      how to create and close milestones.

### Ceremony Facilitation

- [ ] Sprint Planning documented: inputs (refined backlog, velocity),
      outputs (sprint goal, committed items), facilitation checklist.
- [ ] Daily Standup documented: async or sync format, what each
      participant shares (yesterday, today, blockers), how to surface
      impediments.
- [ ] Sprint Review documented: who presents (developer or PO),
      how acceptance criteria are verified live, how feedback is
      captured.
- [ ] Sprint Retrospective documented: format options (Start/Stop/
      Continue, 4Ls, sailboat), how action items are tracked to
      completion.
- [ ] Backlog Refinement documented: cadence, who attends, how
      stories are sized and acceptance criteria sharpened.

### Governance Artifacts

- [ ] ADR lifecycle documented: when to create one (architectural
      decisions with trade-offs), status flow (Proposed → Accepted →
      Superseded → Deprecated), who authors (developer), who approves
      (team consensus).
- [ ] ADR index maintenance explained: `docs/adr/README.md` as the
      registry, numbering convention, how to update.
- [ ] Merge checklist governance explained: what each checklist item
      means, who is responsible, what happens when items are skipped.
- [ ] CODEOWNERS explained: who reviews what, how to update when
      team composition changes.

### Definition of Ready

- [ ] Definition of Ready documented as a checklist:
      - [ ] User story follows the standard format
      - [ ] Acceptance criteria are specific and testable
      - [ ] Dependencies are identified and resolvable
      - [ ] Story is sized by the team
      - [ ] Open questions are resolved or explicitly deferred
- [ ] Explains when the SM should push back on pulling an unready
      story into a sprint.

### Definition of Done

- [ ] Definition of Done documented as a checklist:
      - [ ] All acceptance criteria pass
      - [ ] CI pipeline passes (build + test)
      - [ ] PR meets merge checklist requirements
      - [ ] ADR written (if architectural decision was made)
      - [ ] Product story file updated (criteria checked off)
      - [ ] Documentation updated (if applicable)
      - [ ] Code reviewed and approved
- [ ] Explains the difference between "PR merged" and "Done" —
      Done means acceptance criteria are verified, not just code
      shipped.

### Impediment Tracking

- [ ] How to surface and track impediments: GitHub Issues with a
      `blocked` label, or a dedicated impediment log.
- [ ] Escalation path: when to involve stakeholders, when to defer,
      when to cut scope.
- [ ] Common impediments in this project and their resolutions:
      CI failures, Docker issues, dependency conflicts, merge
      conflicts across feature branches.

## Dependencies

- Developer Contributor Guide story (for CONTRIBUTING.md hub
  structure).
- Product Owner Contributor Guide story (for artifact lifecycle
  alignment).

## Open Questions

- Should the Scrum Master guide include a sprint checklist template
  (pre-planning → planning → daily → review → retro → close)?
- Should governance checks (merge checklist, Definition of Done)
  be enforced by CI, or remain human-verified?
- Should there be a `docs/governance/` directory for living process
  documents that evolve independently of ADRs?