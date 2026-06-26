# Camp Fit Fur Dogs — Scrum Master Workflow Guide  
Operational handbook for sprint execution

This guide documents **how to run sprint ceremonies, manage the board, create Tasks from Stories, and maintain project artifacts** for Camp Fit Fur Dogs (CFFD).

It describes **product‑specific workflow**, not framework behavior.  
All engineering behavior (DI, hosting, testing, observability, etc.) is governed by Frank and documented in the Frank guides.

---

# 1. Core Concept: The 3‑Artifact Workflow

Camp Fit Fur Dogs uses a **clean separation between product definition and execution**:

| Artifact | Location | Purpose |
|----------|----------|---------|
| **Story files** | `product/stories/` | Durable product definition (backlog) |
| **Tasks (GitHub Issues)** | GitHub Issues | Sprint commitments (vertical slices) |
| **Pull Requests** | GitHub PRs | Implementation of a single Task |

### Rules

- **Stories are never GitHub Issues.**  
- **Tasks are the only Issues.**  
- **Each Task is implemented by exactly one PR.**  
- **Stories define the product; Tasks define the sprint.**

This keeps the backlog clean and the board meaningful.

---

# 2. Sprint Planning

Sprint Planning is a **three‑step workflow**.

## Step 1 — Select Stories

1. Review candidate stories with the Product Owner.  
2. Verify each story meets the **Definition of Ready** (see Product Owner Guide).  
3. Align selected stories with the milestone objective.  
4. Confirm team capacity.  
5. Establish a clear sprint goal.

Stories remain in `product/stories/` — they are **not** added to the board.

---

## Step 2 — Break Stories into Tasks (GitHub Issues)

Each story is decomposed into **Tasks** — small, vertical, PR‑sized units.

A Task:

- implements part of a story  
- is mergeable in 1–2 days  
- has its own acceptance criteria subset  
- closes via a single PR  
- is labeled with `task` and `sprint:N`  

### Task Creation Command

```powershell
$repo = "frankjhughes/camp-fit-fur-dogs"

gh issue create --repo $repo `
  --title "US-NNN: <Task Name>" `
  --body "Task for story: \`product/stories/<domain>/US-NNN-title.md\`" `
  --label "task" `
  --label "sprint:8"
```

### Task Rules

- **Title**: `US-NNN: <Task Name>`  
- **Body**: link to the story file  
- **Labels**: `task`, `sprint:N`  
- **Milestones**: optional (stories carry milestone metadata)

---

## Step 3 — Add Tasks to the Project Board

Tasks are not automatically added to the board.  
Use the GitHub GraphQL API:

```powershell
$projectId = "PVT_kwHOAnH4YM4BTJvQ"
$query = 'mutation($projectId: ID!, $contentId: ID!) { addProjectV2ItemById(input: {projectId: $projectId, contentId: $contentId}) { item { id } } }'

$tasks = gh issue list --repo $repo --label "sprint:8" --json number,title,id | ConvertFrom-Json

foreach ($task in $tasks) {
    Write-Host "Adding #$($task.number) ($($task.title))..."
    gh api graphql -f query="$query" -f projectId="$projectId" -f contentId="$($task.id)"
}
```

---

# 3. Sprint Goal

A sprint goal connects the sprint’s Tasks to a milestone objective.

**Format:**

> **Sprint N Goal:** *[Verb] [what] ([milestone reference]).*

Examples:

- *Deliver the authentication foundation (M1).*  
- *Complete the first customer vertical (M1).*  

The sprint goal appears in:

- the sprint review document  
- optionally the project board description  

---

# 4. Sprint Capacity

Consider:

- Number of Tasks  
- Total acceptance criteria coverage  
- Infrastructure/setup work  
- Parallelizable work (frontend vs backend)  
- Historical velocity (measured in Tasks completed)  

Capacity is measured in **Tasks**, not points.

---

# 5. Board Management

## Board Columns

| Column | Meaning |
|--------|---------|
| **Todo** | Committed Task, not started |
| **In Progress** | Actively being worked |
| **Done** | PR merged and AC verified |

## Moving Tasks

- Move to **In Progress** when work begins (branch created).  
- Move to **Done** only after PR merge **and** AC verification.  
- Never move to Done based on PR creation alone.

## Blocked Tasks

1. Add a comment explaining the blocker.  
2. Link the blocking Task: `Blocked by #<number>`.  
3. Leave the Task in its current column.  
4. Raise the blocker in standup or async check‑in.

## Tracking Progress

```powershell
$repo = "frankjhughes/camp-fit-fur-dogs"
$open = (gh issue list --repo $repo --label "sprint:8" --state open --json number | ConvertFrom-Json).Count
$closed = (gh issue list --repo $repo --label "sprint:8" --state closed --json number | ConvertFrom-Json).Count
$total = $open + $closed
Write-Host "Sprint 8: $closed/$total done ($([math]::Round($closed/$total*100))%)"
```

---

# 6. Sprint Review

## Document Format

Create:  
`docs/sprint-reviews/sprint-N.md`

```markdown
# Sprint N Review

## Sprint Goal
> [The sprint goal statement]

## Shipped Tasks
| Task | Story | Status |
|------|--------|--------|
| #NN | US-NNN: Story Title | Done |

## Key Decisions
- [Decision and rationale]

## Metrics
| Metric | Value |
|--------|--------|
| Tasks committed | N |
| Tasks shipped | N |
| Carry-over | N |
| PRs merged | N |

## Demo Notes
[What was demonstrated]

## Retrospective Items

### What went well
- [Item]

### What could improve
- [Item]

### Action items
- [ ] [Concrete next-step]
```

## Compiling the Review

1. List all Tasks with the `sprint:N` label.  
2. Mark shipped (closed) vs carried over (open).  
3. Summarize key decisions.  
4. Record retro items.

## Updating CHANGELOG.md

Each merged PR adds an entry:

```markdown
## Sprint N

### Added
- US-NNN: Short description (#PR)
```

## Updating README Milestone Progress

After the sprint:

- Update milestone completion percentages.  
- Note which stories advanced or completed.  

---

# 7. Sprint Retrospective

## Format

1. **What went well**  
2. **What could improve**  
3. **Action items**  

## Rules

- Action items must be specific and assigned.  
- Review previous sprint’s action items first.  
- Retro items live in the sprint review document.  

## Feeding Retro Into Planning

Action items may become:

- a new story  
- a Task in the next sprint  
- a governance/process update  

---

# 8. Milestone Management

## Milestones vs Sprints

| Concept | Question | Lifecycle |
|---------|----------|-----------|
| **Milestone** | What capability are we building? | Spans multiple sprints |
| **Sprint** | What will we finish this timebox? | Fixed duration |

## Creating a Milestone

```powershell
gh api "repos/frankjhughes/camp-fit-fur-dogs/milestones" `
  --method POST `
  -f title="M4: Milestone Name" `
  -f state=open `
  -f description="One-sentence capability goal."
```

## Closing a Milestone

Close when all its **stories** are complete.

---

# 9. Merge Governance

See:

- `copilot-instructions.md` — branch protection, merge rules  
- `PULL_REQUEST_TEMPLATE.md` — PR conventions  

## Handling Stale Branches

1. Check if the Task is still active.  
2. If abandoned, close the PR and move the Task back to Todo.  
3. Delete the stale branch.

## Handling Failed PRs

1. Fix CI failures.  
2. Address review feedback.  
3. If approach is wrong, close PR and open a new Task/PR.

---

# 10. Sprint Label Convention

Each sprint gets a label: `sprint:N`.

```powershell
gh label create "sprint:9" --color "0E8A16" `
  --description "Sprint 9 timebox" --force `
  --repo frankjhughes/camp-fit-fur-dogs
```

---

# 11. Getting Help

- Product questions → Product Owner Guide  
- Code/PR workflow → Developer Guide  
- Governance → `@FrankJHughes`  

---

# Summary

This guide defines the **operational workflow** for running sprints in Camp Fit Fur Dogs:

- Stories define the product  
- Tasks define the sprint  
- PRs implement Tasks  
- The board reflects sprint execution  
- Reviews and retros maintain continuous improvement  

Frank provides the deterministic engineering foundation.  
This guide provides the **product‑specific process** for delivering value each sprint.
