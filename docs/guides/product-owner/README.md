# Camp Fit Fur Dogs — Product Owner Workflow Guide  
Product‑specific backlog and refinement handbook

This guide documents **how to write stories, manage the backlog, apply emotional guarantees, and run refinement** for Camp Fit Fur Dogs (CFFD).

It describes **product‑specific workflow**, not framework behavior.  
All engineering behavior (DI, hosting, testing, observability, etc.) is governed by Frank and documented in the Frank guides.

---

# 1. How the Backlog Works

The backlog is **markdown files in Git**, not a tool.

Every story lives in:

```
product/stories/<domain>/
```

Domains:

```
customer/    # End‑user features (accounts, dogs, bookings)
docs/        # Documentation and contributor guides
infra/       # CI/CD, tooling, developer experience
```

### Key Principles

- The backlog is **versioned**, **reviewable**, and **searchable**.  
- No story is silently edited — every change is a commit.  
- **GitHub Issues are never stories.**  
- Issues represent **Tasks**, created only when a story enters a sprint.  

This ensures a clean backlog and a meaningful sprint board.

---

# 2. Story Authoring

## 2.1 File Naming Convention (ADR‑0009)

```
US-NNN-kebab-case-title.md
```

- `US` — always uppercase  
- `NNN` — zero‑padded, globally unique  
- `kebab-case-title` — matches the story heading  

Examples:

- `US-027-create-customer-account.md`
- `US-045-product-owner-workflow-guide.md`

---

## 2.2 Story Templates

CFFD uses two templates depending on domain.

### Customer Stories (features)

```markdown
# US-NNN — Title

## Intent
As a [role], I want [capability], so that [value].

## Value
Why this matters to the user and the product.

## Acceptance Criteria
- [ ] Observable, testable criterion.
- [ ] Observable, testable criterion.

## Emotional Guarantees
- **EG-NN Name** — How this story honors the guarantee.

## Edge Cases
- Boundary conditions and failure modes.

## Notes
Optional context or links.
```

### Docs / Infra Stories

```markdown
# US-NNN — Title

## User Story
As a [role], I want [capability], so that [value].

## Context
Background and motivation.

## Scope
What this story produces and where it lives.

## Acceptance Criteria
### Section Name
- [ ] Criterion grouped by deliverable.

## Dependencies
- Other stories this depends on.

## Open Questions
- Items to resolve during refinement.
```

---

# 3. Writing Good Acceptance Criteria

Acceptance criteria must be:

- **Observable** — visible to a reviewer  
- **Testable** — pass/fail without reading code  
- **Implementation‑free** — describe *what*, not *how*  

| Good | Bad |
|------|-----|
| Duplicate email returns a clear error. | Add a unique constraint on the email column. |
| Confirmation message appears after registration. | Use a toast notification component. |
| Guide includes a branch naming table. | Write the table in HTML. |

---

# 4. Emotional Guarantees

Emotional Guarantees (EGs) are **product‑level promises** that shape how features behave.

| ID | Name | Promise |
|----|------|---------|
| EG‑01 | Calm Confidence | No anxiety‑inducing interactions. |
| EG‑02 | Respected Identity | Personal info handled with care. |
| EG‑03 | Graceful Recovery | Clear errors with a path forward. |
| EG‑04 | Forgiveness | Work is never silently lost. |
| EG‑05 | Joyful Moments | Small moments of warmth. |
| EG‑06 | Transparent Progress | Users always know what comes next. |
| EG‑07 | Inclusive Access | Works for all abilities. |

### Tagging Stories with EGs

Every customer story must reference at least one EG:

```markdown
## Emotional Guarantees
- **EG‑01 Calm Confidence** — No countdown timers or urgency language.
- **EG‑03 Graceful Recovery** — Inline validation with corrective guidance.
```

EGs become **testable scope**.

---

# 5. Definition of Ready (DoR)

A story must pass **all 8 criteria** before entering a sprint:

| # | Criterion | What to Check |
|---|-----------|----------------|
| 1 | Clear intent | “As a / I want / So that” is unambiguous. |
| 2 | Single capability | If the title contains “and,” split it. |
| 3 | Emotional guarantees | At least one EG with explanation. |
| 4 | Observable AC | Reviewer can verify without reading code. |
| 5 | Edge cases | Boundary conditions listed. |
| 6 | Contained scope | Work stays within the domain directory. |
| 7 | Dependencies | Blocking stories identified. |
| 8 | No implementation | AC describe behavior, not technology. |

### Running a Readiness Check

If a story fails any criterion → **back to refinement**, not into the sprint.

---

# 6. Backlog Management

## 6.1 Adding a New Story

1. Choose domain directory.  
2. Find next available US number.  
3. Create file using correct template.  
4. Open a PR to `main`.

## 6.2 Retiring a Story

1. Delete the file.  
2. Commit: `stories: retire US-NNN (reason)`  
3. Update dependencies in referencing stories.

## 6.3 Absorbing a Story

When one story’s scope is fully covered by another:

1. Add note at top:  
   `> Absorbed into US-NNN. See [US-NNN](path).`
2. Move unique AC into the absorbing story.  
3. Do **not** delete — the file becomes a redirect.

**Caution:** Absorption is not free. Verify the absorbing story actually delivers the absorbed scope.

## 6.4 Splitting a Story

1. Create new story files.  
2. Add note to original:  
   `> Split into US-NNN and US-NNN.`  
3. Redistribute AC.  
4. Each new story must independently pass DoR.

## 6.5 Updating Scope After Refinement

Allowed changes:

- Clarifying AC wording  
- Adding edge cases  
- Updating EG explanations  

Requires a new story:

- New capability  
- Changed user role or intent  
- Doubling AC count  

---

# 7. Refinement & Prioritization

## 7.1 When to Refine

- Before sprint planning  
- Mid‑sprint when blockers reveal missing AC  
- After retro when action items identify gaps  

## 7.2 Identifying Stories Needing Grooming

Look for:

- Fewer than 3 AC  
- No EGs  
- Open questions  
- Changed dependencies  
- Older than 2 sprints without movement  

## 7.3 Milestone‑Driven Prioritization

Milestones define **capability goals**.  
Sprints define **timeboxed commitments**.

Prioritize by:

1. Which milestone is closest to completion?  
2. Which stories in that milestone pass DoR?  
3. Which ready stories unblock others?  

---

# 8. Milestones

Milestones are tracked in:

```
product/catalog.csv
```

### Creating a Milestone

A milestone is appropriate when:

- Multiple stories contribute to one capability  
- The capability is demonstrable  
- Completion is meaningful to stakeholders  

To propose:

1. Write a one‑sentence goal.  
2. List required stories.  
3. Ensure no story appears in two milestones.  

### Reorganizing Milestones

1. Edit `product/catalog.csv`.  
2. Run `scripts/frontmatter/Sync-FrontMatter.ps1`.  
3. Document the change in the next sprint review.

---

# 9. Conventions Maintenance

Project conventions live in:

```
.github/copilot-instructions.md
```

The Product Owner ensures:

- Retro decisions are captured  
- Conventions remain current  
- Governance stays consistent  

---

# 10. Getting Help

- Sprint ceremonies → **Scrum Master Guide**  
- Code & PR workflow → **Developer Guide**  
- Backlog & prioritization → Tag `@FrankJHughes`  

---

# Summary

This guide defines the **product‑specific workflow** for:

- writing high‑quality stories  
- applying emotional guarantees  
- maintaining a clean, versioned backlog  
- running refinement  
- managing milestones  
- ensuring stories are sprint‑ready  

Frank provides the deterministic engineering foundation.  
This guide provides the **product‑specific process** for defining value.
