# Sprint Reviews

Sprint reviews are **historical summaries** of each sprint.  
They document what shipped, what was learned, and what the team will focus on next.

Sprint reviews are **not** conventions, guides, or runbooks — they are lightweight records of progress.

---

## Purpose

Sprint reviews provide:

- A snapshot of what the team delivered  
- A record of completed and incomplete stories  
- A summary of key decisions and learnings  
- A reference for future planning  
- A consistent narrative of product evolution  

They are intentionally short and easy to maintain.

---

## Template

Use the standard template for each sprint:

- [`sprint-review-template.md`](sprint-review-template.md)

This template reflects the current workflow:

- Stories in `product/stories/`  
- Tasks as GitHub Issues  
- PRs referencing tasks  
- Milestones representing sprints  
- No YAML, no manifests, no planning automation  

---

## Naming Convention

Sprint review files follow this pattern:

```
sprint-01.md
sprint-02.md
sprint-03.md
...
```

Use zero‑padding for consistent sorting.

---

## How to Create a New Sprint Review

1. Copy `sprint-review-template.md`  
2. Rename it to `sprint-NN.md`  
3. Fill in:
   - Sprint goal  
   - Completed tasks  
   - Completed stories  
   - Incomplete stories  
   - Key decisions  
   - What went well  
   - What could improve  
   - Next sprint focus  
4. Add milestone and board links  
5. Commit the file to `main`

Sprint reviews should be added **at the end of each sprint**.

---

## What Not to Include

Sprint reviews should **not** contain:

- Story YAML  
- Sprint manifests  
- Planning automation outputs  
- Velocity calculations  
- Retrospective notes  
- Conventions or rules  

Those belong in:

- `docs/guides/`  
- `docs/conventions/`  
- `docs/governance/`

---

## Past Reviews

Add links here as new reviews are created:

- _None yet — first sprint review will appear here._

