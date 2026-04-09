# Product Owner Workflow Guide

This guide documents how to write stories, manage the backlog, apply
emotional guarantees, and run refinement for Camp Fit Fur Dogs.

## How the Backlog Works

The backlog is **markdown files in Git**, not a tool. Every story lives
in `product/stories/` under a domain directory. GitHub Issues are only
created when a story is pulled into a sprint — they are sprint
commitments, not backlog items.

```
product/stories/
├── customer/    # End-user-facing features (accounts, dogs, bookings)
├── docs/        # Documentation and contributor guides
└── infra/       # CI/CD, tooling, developer experience
```

This means the backlog is versioned, reviewable, and searchable with
standard Git tools. No story is lost, renamed, or silently edited
without a commit trail.

## Story Authoring

### File naming convention (ADR-0009)

```
US-NNN-kebab-case-title.md
```

- **US** — User Story prefix (always uppercase).
- **NNN** — Three-digit number, zero-padded, globally unique across all
  domains. Assign the next available number.
- **kebab-case-title** — Lowercase, hyphen-separated summary matching
  the story heading.

Examples:
- `US-027-create-customer-account.md`
- `US-045-product-owner-workflow-guide.md`

### Story format

Stories use one of two templates depending on the domain:

**Customer stories** (features):

```markdown
# US-NNN — Title

## Intent

As a [role], I want [capability], so that [value].

## Value

Why this matters to the user and the product.

## Acceptance Criteria

- [ ] Observable, testable criterion 1.
- [ ] Observable, testable criterion 2.

## Emotional Guarantees

- **EG-NN Name** — How this story honors the guarantee.

## Edge Cases

- What happens when [boundary condition]?

## Notes

Optional context, links, or prior art.
```

**Docs / infra stories**:

```markdown
# US-NNN — Title

## User Story

As a [role], I want [capability], so that [value].

## Context

Background, motivation, and prior decisions that inform this work.

## Scope

What this story produces and where it lives.

## Acceptance Criteria

### Section Name

- [ ] Criterion grouped by deliverable section.

## Dependencies

- Other stories this depends on.

## Open Questions

- Unresolved decisions to address during refinement.
```

### Writing good acceptance criteria

Each criterion must be:

- **Observable** — A reviewer can see whether it's done.
- **Testable** — You can write a pass/fail check.
- **Implementation-free** — Describe *what*, never *how*.

| Good | Bad |
|------|-----|
| Customer sees a confirmation message after registration. | Use a toast notification component. |
| Duplicate email returns a clear error. | Add a unique constraint on the email column. |
| Guide includes a branch naming table. | Write the table in HTML. |

## Emotional Guarantees

Emotional guarantees are promises the product makes to every user. They
are not features — they are constraints that shape how features behave.

| ID | Name | Promise |
|----|------|---------|
| EG-01 | Calm Confidence | The interface never creates anxiety or urgency. |
| EG-02 | Respected Identity | Personal information is handled with care and never exposed carelessly. |
| EG-03 | Graceful Recovery | Errors are explained clearly with a path forward. |
| EG-04 | Forgiveness | The system is forgiving of interruption — work is never silently lost. |
| EG-05 | Joyful Moments | Positive interactions include small moments of warmth. |
| EG-06 | Transparent Progress | The user always knows where they are and what comes next. |
| EG-07 | Inclusive Access | The experience works for all users regardless of ability. |

### Tagging stories with emotional guarantees

Every customer story must reference at least one emotional guarantee.
In the `## Emotional Guarantees` section, list each relevant guarantee
and explain how the story honors it:

```markdown
## Emotional Guarantees

- **EG-01 Calm Confidence** — Registration flow uses no countdown
  timers or "limited availability" language.
- **EG-03 Graceful Recovery** — Validation errors appear inline next
  to the field, with a suggestion for correction.
```

Emotional guarantees become acceptance criteria. If a story tags EG-03,
then graceful error handling is testable scope — not aspirational.

## Definition of Ready

Before a story can be pulled into a sprint, it must pass all 8 criteria:

| # | Criterion | What to check |
|---|-----------|---------------|
| 1 | Intent is clear | The "As a / I want / So that" is specific and unambiguous. |
| 2 | Single capability | The story delivers one thing. If you say "and" in the title, consider splitting. |
| 3 | Emotional guarantees tagged | At least one EG is referenced with a concrete explanation. |
| 4 | Acceptance criteria are observable | Every AC can be verified by a reviewer without reading code. |
| 5 | Edge cases acknowledged | Boundary conditions are listed, even if the AC says "handled gracefully." |
| 6 | Scope is contained | The story doesn't require work outside its domain directory. |
| 7 | Dependencies are identified | Blocking stories or technical prerequisites are listed. |
| 8 | No implementation prescribed | AC describe behavior, not technology choices. |

### Running a readiness check

Before sprint planning, review each candidate story against the 8
criteria. If a story fails any criterion, it goes back to refinement —
not into the sprint.

**Pass example** — US-027 (Create Customer Account):
- Intent: clear (customer creates account).
- Single capability: yes (account creation only).
- EGs: EG-01, EG-02, EG-03 all explained.
- AC: 5 observable criteria.
- Edge cases: duplicate account documented.
- Scope: contained to customer domain.
- Dependencies: none.
- Implementation: no tech prescribed.

**Fail example** — A story titled "Build the dog management system":
- Fails criterion 2 (multiple capabilities bundled).
- Fails criterion 6 (scope spans multiple aggregates).
- Action: split into register, edit, view, delete stories.

## Backlog Management

### Adding a new story

1. Determine the domain directory (`customer/`, `docs/`, or `infra/`).
2. Find the next available US number by checking all directories.
3. Create the file using the appropriate template.
4. Open a PR to `main` with the story file.

### Retiring a story

When a story is no longer relevant:

1. Delete the story file.
2. Commit with message: `stories: retire US-NNN (reason)`.
3. If the story was referenced by other stories, update their
   dependency sections.

### Absorbing a story

When one story's scope is fully covered by another:

1. Add a note at the top of the absorbed story:
   `> Absorbed into US-NNN. See [US-NNN](path).`
2. Move any unique AC into the absorbing story.
3. Do not delete — the absorbed file serves as a redirect.

Project example: US-010 and US-011 were absorbed into US-022 during
Sprint 2. The scope they covered (PO and SM guides) was later restored
as US-045 and US-046 when the absorbing story didn't deliver the full
scope.

**Lesson:** Absorbing is not free. Track what scope the absorbed story
carried and verify the absorbing story actually delivers it.

### Splitting a story

When a story is too large or covers multiple capabilities:

1. Create new story files for each split.
2. Add a note to the original:
   `> Split into US-NNN and US-NNN.`
3. Redistribute AC across the new stories.
4. Each split story must independently pass the Definition of Ready.

### Updating scope after refinement

Acceptable changes to a committed story:

- Clarifying AC wording (same intent, better precision).
- Adding edge cases discovered during refinement.
- Adding or updating EG explanations.

Changes that require a new story:

- Adding a new capability ("also add edit functionality").
- Changing the user role or intent.
- Doubling the AC count.

## Refinement and Prioritization

### When to refine

- **Before sprint planning** — candidate stories for the next sprint
  must pass the Definition of Ready.
- **Mid-sprint** — when blocked work reveals missing AC or edge cases
  on a future story.
- **After retro** — when action items identify backlog gaps.

### Identifying stories that need grooming

Look for:

- Stories with fewer than 3 AC (likely underspecified).
- Stories with no emotional guarantees tagged.
- Stories with open questions that haven't been answered.
- Stories whose dependencies have changed since they were written.
- Stories older than 2 sprints without being pulled (may be stale).

### Milestone-driven prioritization

Milestones define capability goals. Sprints are timeboxes. They are
independent:

- **Milestones** answer: *What must be true for this capability to
  exist?*
- **Sprints** answer: *What will we finish in the next timebox?*

Prioritize by milestone:

1. Which milestone is closest to completion?
2. Which stories in that milestone are ready (pass DoR)?
3. Do any ready stories unblock other milestone stories?

Current milestones:

| Milestone | Goal | Key Stories |
|-----------|------|-------------|
| M1: First Customer Vertical | Account + dog registration + profile view | US-027, US-028, US-029 |
| M2: Complete Dog Management | Full CRUD + edge cases for dog records | US-030-034, US-037, US-044 |
| M3: Portfolio Showcase | Project documented, tooled, and presentable | US-006, US-007, US-009, US-022 |

## Milestones

### Creating a new milestone

A milestone is appropriate when:

- Multiple stories contribute to a single capability goal.
- The goal is demonstrable ("a customer can register and view a dog").
- Completion is meaningful to stakeholders.

To propose a milestone:

1. Write a one-sentence goal.
2. List the stories required to achieve it.
3. Verify no story appears in two milestones.
4. Create the GitHub Milestone with the goal as the description.

### Reorganizing milestones

When priorities shift:

1. Move stories between milestones via `gh issue edit --milestone`.
2. Update the milestone description if the goal changed.
3. Document the change in the next sprint review.

## Getting Help

- Check the [Scrum Master Guide](scrum-master-guide.md) for sprint
  ceremonies and board management.
- Check the [Developer Guide](developer-guide.md) for code and PR
  workflow.
- Tag `@FrankJHughes` for backlog or prioritization questions.
