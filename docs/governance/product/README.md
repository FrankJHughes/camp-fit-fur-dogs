# Product Governance Guide  
Authoritative rules for defining and evolving the Camp Fit Fur Dogs product

This document defines the **product‑level governance system** for Camp Fit Fur Dogs (CFFD).  
It establishes the rules, boundaries, and decision‑making structures that ensure the product remains:

- coherent  
- consistent  
- maintainable  
- emotionally aligned  
- milestone‑driven  
- refinement‑ready  
- governed  

This guide governs **product definition**, not engineering execution.  
Engineering governance lives in Frank’s governance documents.

---

# 1. Purpose of Product Governance

Product governance ensures that:

- The product evolves intentionally  
- Stories are consistent and high‑quality  
- Emotional guarantees are honored  
- Capabilities are decomposed correctly  
- Milestones reflect real capability goals  
- Refinement produces sprint‑ready stories  
- Decisions are recorded and reversible  
- The backlog remains clean and navigable  

This guide defines **rules**, not workflows.  
Role‑specific workflows live in:

- Product Owner Guide  
- Scrum Master Guide  
- Developer Guide  
- Tester Guide  

---

# 2. Product Definition Governance

## 2.1 What a Story Represents

A story represents:

- a single user capability  
- expressed from a user’s perspective  
- with clear value  
- with observable acceptance criteria  
- with emotional guarantees  
- with contained scope  

A story is **not**:

- a task  
- a technical implementation  
- a multi‑capability bundle  
- a placeholder for future work  

## 2.2 Story File Requirements

Every story must:

- live in `product/stories/<domain>/`  
- follow naming convention `US-NNN-kebab-case-title.md`  
- use the correct template (customer vs docs/infra)  
- include Intent, Value, AC, EGs, Edge Cases  
- include dependencies when applicable  
- include milestone metadata in frontmatter  

## 2.3 Story Boundaries

A story must:

- deliver **one** capability  
- stay within **one** domain directory  
- not require cross‑domain work unless explicitly stated  
- not prescribe implementation details  
- not include UI mockups (those belong in design artifacts)  

---

# 3. Capability Governance

Capabilities define **what the product can do**, independent of sprints.

A capability:

- is larger than a story  
- is smaller than a milestone  
- is user‑visible  
- is demonstrable  
- is stable over time  

Examples:

- “Owners can register accounts”  
- “Owners can manage dog profiles”  
- “Staff can view daily schedule”  

## 3.1 Capability Decomposition Rules

A capability must be decomposed into stories such that:

- each story is independently valuable  
- each story is independently testable  
- no story depends on UI state from another story  
- no story contains multiple verbs (“create and edit”)  
- emotional guarantees apply consistently across all stories  

## 3.2 Capability Completion

A capability is complete when:

- all required stories are complete  
- all emotional guarantees are satisfied  
- all edge cases are covered  
- the capability is demonstrable end‑to‑end  

---

# 4. Milestone Governance

Milestones represent **capability goals**, not sprints.

Milestones answer:

> “What must be true for this capability to exist?”

## 4.1 Milestone Requirements

A milestone must:

- have a one‑sentence capability goal  
- list all required stories  
- contain no unrelated stories  
- be demonstrable when complete  
- be meaningful to stakeholders  

## 4.2 Milestone Structure

Milestones are tracked in:

```
product/catalog.csv
```

Each story includes milestone metadata in its frontmatter.

## 4.3 Milestone Reorganization

When priorities shift:

1. Update `product/catalog.csv`  
2. Run `scripts/frontmatter/Sync-FrontMatter.ps1`  
3. Document the change in the next sprint review  

Milestones must remain:

- stable  
- intentional  
- capability‑aligned  

---

# 5. Emotional Guarantee Governance

Emotional Guarantees (EGs) are **product‑level constraints** that shape all user‑facing behavior.

They are not optional.  
They are not “nice to have.”  
They are **governance**.

## 5.1 EG Requirements

Every customer story must:

- reference at least one EG  
- explain how the story honors it  
- include EG‑driven acceptance criteria  

## 5.2 EG Enforcement

An EG violation is a **story failure**, not a UX issue.

Examples:

- EG‑01 Calm Confidence → no urgency language  
- EG‑03 Graceful Recovery → clear, actionable errors  
- EG‑04 Forgiveness → no silent data loss  
- EG‑06 Transparent Progress → clear progress indicators  

## 5.3 EG Lifecycle

EGs may only be:

- added  
- clarified  
- refined  

EGs may **not** be removed without a governance decision.

---

# 6. Story Lifecycle Governance

Stories evolve through a governed lifecycle:

```
Draft → Refined → Ready → In Sprint → Completed → Retired
```

## 6.1 Draft

A draft story:

- may be incomplete  
- may contain open questions  
- may lack EGs  
- may lack AC  

Drafts are allowed only in early ideation.

## 6.2 Refined

A refined story:

- uses the correct template  
- has clear intent  
- has initial AC  
- has initial EGs  
- has initial edge cases  

## 6.3 Ready

A story is Ready when it passes all **8 Definition of Ready** criteria.

Ready stories may enter a sprint.

## 6.4 In Sprint

A story in sprint:

- is decomposed into Tasks  
- must not change intent  
- must not add new capabilities  
- may clarify AC wording  
- may add edge cases  

## 6.5 Completed

A story is completed when:

- all Tasks are merged  
- all AC are satisfied  
- all EGs are honored  
- the capability is demonstrable  

## 6.6 Retired

A story is retired when:

- it is no longer relevant  
- it has been superseded  
- it has been absorbed  

Retired stories remain in Git for traceability.

---

# 7. Story Evolution Governance

## 7.1 Absorbing a Story

A story may be absorbed when:

- its scope is fully covered by another story  
- its capability is no longer distinct  

Rules:

1. Add redirect note  
2. Move unique AC  
3. Do not delete the file  

## 7.2 Splitting a Story

A story must be split when:

- it contains multiple capabilities  
- it contains multiple verbs  
- its AC exceed reasonable scope  

Rules:

1. Create new stories  
2. Add split note  
3. Redistribute AC  
4. Each new story must pass DoR  

## 7.3 Updating a Story

Allowed:

- clarifying AC  
- adding edge cases  
- refining EG explanations  

Requires new story:

- new capability  
- changed user role  
- changed intent  
- doubled AC count  

---

# 8. Backlog Governance

The backlog must remain:

- clean  
- intentional  
- prioritized  
- milestone‑aligned  

## 8.1 Backlog Rules

- No story enters a sprint without passing DoR  
- No story may be silently edited  
- No story may be deleted without retirement  
- No story may exist without a domain  
- No story may exist without a milestone  

## 8.2 Backlog Hygiene

Stories must be reviewed when:

- dependencies change  
- milestones shift  
- EGs evolve  
- refinement reveals missing AC  
- stories become stale  

---

# 9. Refinement Governance

Refinement ensures stories are sprint‑ready.

## 9.1 Refinement Inputs

- candidate stories  
- milestone priorities  
- retro action items  
- dependency changes  
- new insights from ongoing work  

## 9.2 Refinement Outputs

- clarified AC  
- clarified EGs  
- updated edge cases  
- updated dependencies  
- updated milestone assignments  
- stories moved to Ready  

## 9.3 Refinement Rules

- Refinement is not optional  
- Refinement must occur before sprint planning  
- Refinement must be documented in story files  
- Refinement must not change story intent  

---

# 10. Product Decision Governance

Decisions must be:

- explicit  
- documented  
- reversible  
- traceable  

## 10.1 Decision Recording

Decisions are recorded in:

- story files  
- milestone definitions  
- sprint reviews  
- `.github/copilot-instructions.md` (for process changes)  

## 10.2 Decision Reversal

A decision may be reversed only when:

- new information invalidates it  
- it conflicts with EGs  
- it conflicts with capability goals  
- it conflicts with milestone structure  

Reversals must be documented.

---

# 11. Governance Change Process

Changes to product governance require:

1. A PR modifying this file  
2. Clear rationale  
3. Review by Product Owner  
4. Review by Scrum Master  
5. Approval by Product Owner  
6. Updates to role guides if required  

Governance must remain:

- stable  
- intentional  
- minimal  
- enforceable  

---

# Summary

This Product Governance Guide defines the **rules that govern the product**, including:

- story structure  
- capability decomposition  
- milestone definition  
- emotional guarantees  
- story lifecycle  
- backlog hygiene  
- refinement structure  
- decision governance  

Frank provides the deterministic engineering foundation.  
This guide provides the **product‑level rule system** that ensures Camp Fit Fur Dogs evolves intentionally and coherently.
