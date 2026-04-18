# Scrum Master Workflow Guide

This guide documents how to run sprint ceremonies, manage the board,
create issues from stories, and maintain project artifacts for
Camp Fit Fur Dogs.

## Core Concept: The 2-Step Workflow

This project separates the **backlog** from **sprint commitments**:

| Artifact | Where | Purpose |
|----------|-------|---------|
| Story files | `product/stories/` | The backlog — all potential work |
| GitHub Issues | `Issues` tab | Sprint commitments — only created when pulled into a sprint |

**Stories are never pre-created as issues.** A GitHub Issue means "we
committed to doing this in sprint N." This keeps the issue tracker
clean and the board meaningful.

## Sprint Planning

### Step 1: Select stories

1. Review candidate stories with the Product Owner.
2. Verify each story passes all 8 Definition of Ready criteria
   (see the [Product Owner Guide](product-owner-guide.md#definition-of-ready)).
3. Agree on a sprint goal that connects to a milestone objective.
4. Confirm the team has capacity for the selected stories.

### Step 2: Create issues from stories

For each committed story, create a GitHub Issue:

```powershell
$repo = "frankjhughes/camp-fit-fur-dogs"

gh issue create --repo $repo `
  --title "US-NNN: Story Title" `
  --body "Story: [US-NNN](https://github.com/$repo/blob/main/product/stories/<domain>/US-NNN-kebab-title.md)" `
  --milestone "M1: First Customer Vertical" `
  --label "sprint:3"
```

Key rules:

- **`--title`** matches the story heading: `US-NNN: Title`.
- **`--body`** links back to the story file (the source of truth).
- **`--milestone`** uses the milestone **title** (not number).
- **`--label`** uses the `sprint:N` label for the current sprint.

### Step 3: Add issues to the project board

Issues are not automatically added to the project board. After
creation, add them using the GraphQL API:

```powershell
$projectId = "PVT_kwHOAnH4YM4BTJvQ"
$query = 'mutation($projectId: ID!, $contentId: ID!) { addProjectV2ItemById(input: {projectId: $projectId, contentId: $contentId}) { item { id } } }'

$issues = gh issue list --repo $repo --state open --json number,title,id | ConvertFrom-Json

foreach ($issue in $issues) {
    Write-Host "Adding #$($issue.number) ($($issue.title))..."
    gh api graphql -f query="$query" -f projectId="$projectId" -f contentId="$($issue.id)"
}
```

### Setting a sprint goal

A sprint goal connects the sprint's work to a milestone objective.
Format:

> **Sprint N Goal:** *[Verb] [what] ([milestone reference]).*

Examples:
- *Ship the first customer vertical (M1) and document all three
  contributor role workflows.*
- *Complete dog management CRUD (M2).*

The sprint goal goes in the sprint review document and optionally in
the project board description.

### Sprint capacity

Consider:

- Number of stories and total acceptance criteria count.
- Technical infrastructure work (not captured in stories but needed
  to deliver them — e.g., first-time database setup).
- Stories that are documentation-only vs code — they can run in
  parallel.
- Historical velocity: Sprint 0 shipped 3 stories, Sprint 1 shipped
  4, Sprint 2 shipped 11.

## Board Management

### Board structure

The GitHub Projects board uses three columns:

| Column | Meaning |
|--------|---------|
| **Todo** | Committed but not started |
| **In Progress** | Actively being worked |
| **Done** | Merged to main, AC verified |

### Moving issues

- Move to **In Progress** when a branch is created and work begins.
- Move to **Done** only after the PR is merged and AC are verified.
- Never move to Done based on PR creation alone.

### Handling blocked issues

When an issue is blocked:

1. Add a comment to the issue explaining the blocker.
2. If the blocker is another issue, link them with
   `Blocked by #<number>`.
3. Leave the issue in its current column — do not create a "Blocked"
   column.
4. Raise the blocker in standup or async check-in.

### Tracking progress

Sprint progress = issues in Done / total issues. Check the board
view's built-in progress bar, or run:

```powershell
$repo = "frankjhughes/camp-fit-fur-dogs"
$open = (gh issue list --repo $repo --label "sprint:3" --state open --json number | ConvertFrom-Json).Count
$closed = (gh issue list --repo $repo --label "sprint:3" --state closed --json number | ConvertFrom-Json).Count
$total = $open + $closed
Write-Host "Sprint 3: $closed/$total done ($([math]::Round($closed/$total*100))%)"
```

## Sprint Review

### Document format

After each sprint, create a review document at
`docs/sprint-reviews/sprint-N.md`:

```markdown
# Sprint N Review

## Sprint Goal

> [The sprint goal statement]

## Shipped Stories

| Issue | Story | Status |
|-------|-------|--------|
| #NN | US-NNN: Title | Done |

## Key Decisions

- [Decision made during the sprint and why]

## Metrics

| Metric | Value |
|--------|-------|
| Stories committed | N |
| Stories shipped | N |
| Carry-over | N |
| PRs merged | N |

## Demo Notes

[What was demonstrated and to whom]

## Retrospective Items

### What went well

- [Item]

### What could improve

- [Item]

### Action items

- [ ] [Concrete action for next sprint]
```

### Compiling the review

1. List all issues with the `sprint:N` label.
2. For each, note whether it shipped (closed) or carried over (open).
3. Summarize key decisions made during the sprint (ADRs, scope
   changes, process changes).
4. Record retro items while they're fresh.

### Updating CHANGELOG.md

After each merged PR, add an entry to CHANGELOG.md under the current
sprint heading:

```markdown
## Sprint N

### Added
- US-NNN: Short description (#PR)

### Changed
- US-NNN: Short description (#PR)

### Fixed
- Description of fix (#PR)
```

### Updating README milestone progress

After sprint review, update the milestone progress section in
README.md to reflect current status:

- Which milestones are complete, in progress, or not started.
- How many stories remain in each active milestone.

## Sprint Retrospective

### Format

Use a simple three-section retro:

1. **What went well** — practices, tools, or decisions to keep.
2. **What could improve** — friction, surprises, or mistakes.
3. **Action items** — concrete, assignable changes for next sprint.

### Rules

- Action items must be specific ("Add a PR template") not vague
  ("Improve code review").
- Each action item gets assigned to a person.
- Review previous retro action items at the start — were they done?
- Retro items are recorded in the sprint review document.

### Feeding retro into planning

Action items from retro can become:

- A new story (if the scope is large enough).
- A task added to the next sprint's technical work.
- A process change documented in governance.

## Milestone Management

### Milestones vs sprints

These are independent concepts:

| Concept | Question it answers | Lifecycle |
|---------|-------------------|-----------|
| **Milestone** | What capability goal are we building toward? | Spans multiple sprints |
| **Sprint** | What will we finish in this timebox? | Fixed duration (1-2 weeks) |

A sprint can contain stories from multiple milestones. A milestone
can span multiple sprints.

### Creating a GitHub Milestone

```powershell
gh api "repos/frankjhughes/camp-fit-fur-dogs/milestones" `
  --method POST `
  -f title="M4: Milestone Name" `
  -f state=open `
  -f description="One-sentence capability goal."
```

### Closing a milestone

Close a milestone when all its stories have shipped:

```powershell
# Find the milestone number
gh api "repos/frankjhughes/camp-fit-fur-dogs/milestones?state=open" | ConvertFrom-Json | Format-Table number, title

# Close it
gh api "repos/frankjhughes/camp-fit-fur-dogs/milestones/<number>" `
  --method PATCH -f state=closed
```

When closing a milestone:

1. Verify all assigned issues are closed.
2. Record the completion in the sprint review document.
3. Update README milestone progress.
4. Consider a demo or case study for portfolio value.

### Current milestones

See the [Product Owner Guide — Milestones](product-owner-guide.md#milestones) for the authoritative milestone table.

## Merge Governance

See [copilot-instructions.md — Source Control](../../.github/copilot-instructions.md) for the authoritative branch protection settings, branching model, and PR conventions.

See the [Pull Request Template](../../.github/PULL_REQUEST_TEMPLATE.md) for the enforced merge checklist.

### Handling stale branches

Branches that haven't been updated in 2+ weeks:

1. Check if the associated issue is still in the sprint.
2. If the work is abandoned, close the PR and move the issue back
   to Todo (or remove from the sprint).
3. Delete the stale branch.

### Handling failed PRs

When a PR can't be merged:

1. If CI fails — fix the code and push.
2. If review is rejected — address feedback and re-request review.
3. If the approach is wrong — close the PR, discuss in the issue,
   and open a new PR with a different approach.

## Sprint Label Convention

Each sprint gets a label: `sprint:N`.

```powershell
gh label create "sprint:4" --color "0E8A16" `
  --description "Sprint 4 timebox" --force `
  --repo frankjhughes/camp-fit-fur-dogs
```

Use consistent colors per sprint for visual distinction on the board.


## Conventions Review

At each sprint closing, review [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md) for staleness. See its §Maintaining This Document section for the full review checklist.
## Getting Help

- Check the [Product Owner Guide](product-owner-guide.md) for story
  and backlog questions.
- Check the [Developer Guide](developer-guide.md) for code and PR
  workflow.
- Tag `@FrankJHughes` for process or governance questions.

