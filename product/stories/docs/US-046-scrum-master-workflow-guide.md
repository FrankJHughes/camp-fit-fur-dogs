# US-046 — Scrum Master Workflow Guide

## User Story

As a scrum master joining the project, I want a single guide
that documents how to run sprint ceremonies, manage the board,
create issues from stories, and maintain project artifacts — so
that sprint execution follows a consistent, transparent process.

## Context

The project uses a 2-step workflow: story files are the backlog,
GitHub Issues are created only when stories are pulled into a
sprint. Sprint tracking uses labels (`sprint:N`) while capability
progress uses GitHub Milestones (M1, M2, M3). No documentation
currently explains this workflow or the ceremony cadence.

US-011 (SM Contributor Guide) was absorbed into US-022 during
Sprint 2, but US-022 focused on the planning runbook — the
standalone SM workflow guide was never produced.

## Scope

This guide lives at `docs/guides/scrum-master-guide.md` and is
linked from the CONTRIBUTING.md role-routing hub (created by US-009).

## Acceptance Criteria

### Sprint Planning

- [ ] How to select stories from the backlog for sprint commitment.
- [ ] How to create GitHub Issues from story files (2-step workflow
      documented with exact commands).
- [ ] How to assign milestone and sprint label to each issue.
- [ ] How to set a sprint goal that connects to milestone objectives.
- [ ] Sprint capacity considerations documented.

### Board Management

- [ ] GitHub Projects board setup and column structure explained.
- [ ] How to move issues through the board during a sprint.
- [ ] How to handle blocked issues and dependencies.
- [ ] How to track sprint progress against the sprint goal.

### Sprint Review

- [ ] Sprint review document format explained with reference to
      template (`docs/sprint-reviews/template.md`).
- [ ] How to compile shipped stories, key decisions, metrics,
      and retro items.
- [ ] How to update CHANGELOG.md after each merged PR.
- [ ] How to update README milestone progress after sprint review.

### Sprint Retrospective

- [ ] Retro format documented: what went well, what could improve,
      action items.
- [ ] How action items feed into next sprint planning.

### Milestone Management

- [ ] Difference between milestones (capability goals) and sprints
      (timeboxes) explained.
- [ ] How to create, close, and update GitHub Milestones.
- [ ] When to record demos or case studies (milestone completion
      triggers).
- [ ] How to update milestone progress in stakeholder artifacts.

### Merge Governance

- [ ] Branch protection rules explained (who can merge, required
      reviews).
- [ ] CODEOWNERS file explained.
- [ ] PR checklist walkthrough.
- [ ] How to handle stale branches and failed PRs.

## Dependencies

- US-009 (Developer Contributor Guide) creates the CONTRIBUTING.md
  hub that routes to this guide.

## Open Questions

- Should the guide include a sprint planning checklist as a
  separate artifact?
- Should CHANGELOG maintenance be automated via CI?
