# Scrum Master Workflow Guide

This guide documents how to run sprint ceremonies, manage the board,
create tasks from stories, and maintain project artifacts for
Camp Fit Fur Dogs.

## Core Concept: The 3‑Artifact Workflow

This project separates **product definition** from **execution**:

| Artifact | Where | Purpose |
|----------|-------|---------|
| **Story files** | `product/stories/` | The backlog — durable product definition |
| **Tasks (GitHub Issues)** | Issues tab | Sprint commitments — vertical slices of work |
| **Pull Requests** | PRs | Implementation of a single task |

**Stories are never created as GitHub Issues.**  
A GitHub Issue represents a **Task**, meaning “a vertical slice of work committed in sprint N.”

This keeps the issue tracker clean and the board meaningful.

---

## Sprint Planning

### Step 1: Select stories

1. Review candidate stories with the Product Owner.
2. Verify each story passes all 8 Definition of Ready criteria  
   (see the Product Owner Guide).
3. Agree on a sprint goal that connects to a milestone objective.
4. Confirm the team has capacity for the selected stories.

### Step 2: Break stories into Tasks (GitHub Issues)

Each story is decomposed into **Tasks** — small, vertical, PR‑sized units.

A Task:

- implements part of a story  
- is mergeable in 1–2 days  
- has its own acceptance criteria subset  
- closes via a single PR  

Create a Task using the Task Issue Template:

```powershell
$repo = "frankjhughes/camp-fit-fur-dogs"

gh issue create --repo $repo `
  --title "US-NNN: <Task Name>" `
  --body "Task for story: \`product/stories/<domain>/US-NNN-title.md\`" `
  --label "task" `
  --label "sprint:8"
```

Key rules:

- **Title** begins with the story ID: `US-NNN: <Task Name>`
- **Body** links to the story file (source of truth)
- **Label** includes `task` and `sprint:N`
- **Milestones** are optional for Tasks (stories carry milestone metadata)

### Step 3: Add Tasks to the project board

Tasks are not automatically added to the board. Add them using the GraphQL API:

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

## Setting a Sprint Goal

A sprint goal connects the sprint’s Tasks to a milestone objective.

Format:

> **Sprint N Goal:** *[Verb] [what] ([milestone reference]).*

Examples:

- *Deliver the authentication foundation (M1).*
- *Complete the first customer vertical (M1).*

The sprint goal goes in the sprint review document and optionally in the project board description.

---

## Sprint Capacity

Consider:

- Number of Tasks and total AC coverage
- Infrastructure or setup work required to enable Tasks
- Parallelizable work (frontend vs backend)
- Historical velocity (measured in Tasks completed)

---

## Board Management

### Board Structure

| Column | Meaning |
|--------|---------|
| **Todo** | Task committed but not started |
| **In Progress** | Actively being worked |
| **Done** | PR merged and AC verified |

### Moving Tasks

- Move to **In Progress** when a branch is created and work begins.
- Move to **Done** only after the PR is merged and AC are verified.
- Never move to Done based on PR creation alone.

### Handling Blocked Tasks

1. Add a comment explaining the blocker.
2. Link the blocking Task: `Blocked by #<number>`.
3. Leave the Task in its current column.
4. Raise the blocker in standup or async check‑in.

### Tracking Progress

Sprint progress = Tasks in Done / total Tasks.

```powershell
$repo = "frankjhughes/camp-fit-fur-dogs"
$open = (gh issue list --repo $repo --label "sprint:8" --state open --json number | ConvertFrom-Json).Count
$closed = (gh issue list --repo $repo --label "sprint:8" --state closed --json number | ConvertFrom-Json).Count
$total = $open + $closed
Write-Host "Sprint 8: $closed/$total done ($([math]::Round($closed/$total*100))%)"
```

---

## Sprint Review

### Document Format

Create `docs/sprint-reviews/sprint-N.md`:

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

### Compiling the Review

1. List all Tasks with the `sprint:N` label.
2. Mark shipped (closed) vs carried over (open).
3. Summarize key decisions.
4. Record retro items.

### Updating CHANGELOG.md

Each merged PR adds an entry under the current sprint:

```markdown
## Sprint N

### Added
- US-NNN: Short description (#PR)

### Changed
- US-NNN: Short description (#PR)

### Fixed
- Description of fix (#PR)
```

### Updating README Milestone Progress

After the sprint:

- Update milestone completion percentages.
- Note which stories advanced or completed.

---

## Sprint Retrospective

### Format

1. **What went well**
2. **What could improve**
3. **Action items**

### Rules

- Action items must be specific and assigned.
- Review previous sprint’s action items first.
- Retro items are recorded in the sprint review document.

### Feeding Retro into Planning

Action items may become:

- A new story (if large)
- A Task in the next sprint
- A process change documented in governance

---

## Milestone Management

### Milestones vs Sprints

| Concept | Question | Lifecycle |
|---------|----------|-----------|
| **Milestone** | What capability are we building? | Spans multiple sprints |
| **Sprint** | What will we finish this timebox? | Fixed duration |

### Creating a Milestone

```powershell
gh api "repos/frankjhughes/camp-fit-fur-dogs/milestones" `
  --method POST `
  -f title="M4: Milestone Name" `
  -f state=open `
  -f description="One-sentence capability goal."
```

### Closing a Milestone

Close when all its **stories** are complete.

---

## Merge Governance

See:

- `copilot-instructions.md` — branch protection, merge rules  
- `PULL_REQUEST_TEMPLATE.md` — PR conventions  

### Handling Stale Branches

1. Check if the Task is still active.
2. If abandoned, close the PR and move the Task back to Todo.
3. Delete the stale branch.

### Handling Failed PRs

1. Fix CI failures.
2. Address review feedback.
3. If approach is wrong, close PR and open a new Task/PR.

---

## Sprint Label Convention

Each sprint gets a label: `sprint:N`.

```powershell
gh label create "sprint:9" --color "0E8A16" `
  --description "Sprint 9 timebox" --force `
  --repo frankjhughes/camp-fit-fur-dogs
```

---

## Getting Help

- Product questions → Product Owner Guide  
- Code/PR workflow → Developer Guide  
- Governance → `@FrankJHughes`
