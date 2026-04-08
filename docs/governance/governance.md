# Governance — Camp Fit Fur Dogs

**Purpose**  
This document is the single source of truth for project decisions, process, and ownership. It defines how we make architecture choices, run sprints, accept work, and keep the repository healthy and consistent. Treat this file as a living artifact: update it by PR whenever code or process changes.

---

## Scope and Audience

**Scope**  
Covers project roles, sprint cadence, branch strategy, review rules, documentation rules, ADR process, and enforcement mechanisms.

**Audience**  
Product Owner, Scrum Master, Developers, Reviewers, Contributors.

---

## Roles and Responsibilities

- **Product Owner**  
  Owns the product backlog, priorities, acceptance criteria, and final acceptance of increments.

- **Scrum Master**  
  Facilitates ceremonies, removes impediments, and ensures the team follows agreed process.

- **Docs Owner**  
  Maintains `docs/`, ADRs, governance content, and runs periodic audits.

- **Development Team**  
  Delivers increments, writes tests, updates docs, and follows the Definition of Done.

- **Code Owners**  
  Named in `CODEOWNERS`; required reviewers for changes to critical areas such as `docs/`, `src/`, and `infra/`.

---

## Cadence and Ceremonies

**Sprint length**  
Two weeks. Sprint 0 may be one week for initial setup.

**Ceremonies**  
- **Sprint Planning**: Day 0, 2 hours. Define sprint goal and commit stories.  
- **Daily Standup**: 15 minutes. What I did, what I will do, blockers.  
- **Backlog Refinement**: Mid‑sprint, 45–60 minutes. Clarify and split upcoming stories.  
- **Sprint Review and Demo**: Last day, 30–45 minutes. Demo increment and collect feedback.  
- **Sprint Retrospective**: Last day, 30–45 minutes. Inspect and adapt process.

---

## Definition of Done

Every story or PR must satisfy the Definition of Done before merge:

- **Build**: Code compiles and builds.  
- **Tests**: Unit tests added or updated and passing. Integration tests added or updated when applicable.  
- **Docs**: Public API changes documented; relevant `docs/` updated.  
- **CI**: All CI checks pass.  
- **Review**: At least one approving review from a Code Owner.  
- **Changelog**: User‑facing changes added to `CHANGELOG.md` under Unreleased.

---

## Definition of Ready

A story is Ready when:

- Acceptance criteria are clear and testable.  
- Dependencies are identified and either resolved or listed as tasks.  
- Story is estimated and small enough to complete in a sprint (split if > 5 points).  
- Any required design or ADR is referenced.

---

## Branch Strategy and Pull Request Rules

**Branching**  
- `trunk` is the main integration branch.  
- Feature branches: `feature/<short-name>` or `chore/<short-name>`.  
- Hotfix branches: `hotfix/<short-name>`.

**Pull Requests**  
- Use the PR template and link the story/issue.  
- PR must include DoD checklist items.  
- PRs touching `docs/` or ADRs require review by Docs Owner.  
- Merge only when CI is green and required reviews are complete.

### Branch Protection on `main`

The following rules are enforced via the GitHub branch-protection API
(applied by `scripts/configure-branch-protection.sh`):

| Setting | Value | Rationale |
|---|---|---|
| Require pull request | Yes | All changes flow through PRs — no direct pushes |
| Required approving reviews | 1 | CODEOWNERS approval required |
| Dismiss stale reviews | Yes | New pushes invalidate previous approvals |
| Require status checks (strict) | `Build & Test` | CI must pass on an up-to-date branch |
| Require CODEOWNERS review | Yes | Critical-path changes need owner sign-off |
| Allow force-push | No | Protect commit history |
| Allow deletion | No | Prevent accidental branch removal |
| Enforce for admins | No | Solo-dev bypass — see rationale below |

> **Solo-dev rationale:** `enforce_admins` is `false` so the sole
> maintainer can bypass protection in genuine emergencies (e.g., a
> broken CI pipeline that blocks all PRs).  Every bypass **must** be
> documented in the next sprint retrospective.

#### Emergency Override Process

1. Confirm the override is necessary and CI cannot be fixed first.
2. Push directly to `main` with a commit message prefixed `[EMERGENCY]`.
3. Open a follow-up issue describing the override and root cause.
4. Discuss in the next sprint retrospective.

---

## Architecture Decision Records

**Location**  
`docs/adr/`

**ADR Template**  
- **Title**: short descriptive title.  
- **Status**: Proposed | Accepted | Deprecated.  
- **Context**: problem statement.  
- **Decision**: what we decided.  
- **Consequences**: tradeoffs and migration steps.  
- **Date** and **Authors**.

**When to create an ADR**  
Create an ADR for any nontrivial architectural or process decision that affects multiple parts of the system or is costly to reverse.

---

## Documentation Rules

- All behavior changes must update `docs/` in the same PR.  
- Keep `docs/governance/governance.md` and ADRs up to date.  
- Use `CHANGELOG.md` for user‑visible changes. Follow Keep a Changelog style.  
- Lint markdown in CI. Docs changes require the same review process as code.

---

## Security and Secrets

- Never commit secrets to the repo. Use GitHub Secrets for CI and hosting credentials.  
- Follow least privilege for cloud resources. Document required roles and permissions in `docs/runbooks/runbook.md`.

---

## CI Deployments and Environments

- CI runs on PRs and merges to `trunk`. CI must run build, tests, and lint.  
- Use preview environments for feature branches when possible. Provision ephemeral DB branches for integration tests if supported.  
- Deployments to public demo environment require passing CI and an approved PR.

---

## Enforcement and Automation

- **CODEOWNERS** enforces required reviewers for critical paths.  
- **CI checks** include build, tests, markdown lint, and optional doc coverage checks.  
- **PR template** includes DoD checklist and changelog reminder.
- **Branch protection** on `main` blocks direct pushes, requires passing CI (`Build & Test`), requires CODEOWNERS approval, and dismisses stale reviews.  See *Branch Protection on `main`* above.  
- Periodic audit: Docs Owner runs an audit every two sprints and opens issues for stale ADRs or missing docs.

---

## Change Process for Governance

- Propose a change by opening a PR that updates `docs/governance/governance.md` or adds an ADR.  
- PR must include rationale and link to any related code changes.  
- Changes require one approving review from Docs Owner or Product Owner.  
- After merge, announce the change in the sprint review and update any affected templates.

---

## Runbook and Incident Basics

Keep a short runbook at `docs/runbooks/runbook.md` with steps to:

- Reproduce common local issues.  
- Run migrations and seed demo data.  
- Roll back a deployment.  
- Rotate secrets.

---

## Ownership and Contacts

- **Product Owner**: **Frank**  
- **Scrum Master**: **Frank (temporary)**  
- **Docs Owner**: **Frank (temporary)**  
- **Code Owners**: see `CODEOWNERS` file (initially set to Frank for `docs/`, `src/`, `infra/`)

> **Action:** Replace temporary assignments with named team members as soon as hires/volunteers are confirmed. Update `docs/governance/governance.md` and `CODEOWNERS` in the same PR.

---

## Where to Find Things

- **Governance**: `docs/governance/governance.md`  
- **ADRs**: `docs/adr/`  
- **DoD**: `(see Definition of Done above)`  
- **Changelog**: `CHANGELOG.md`  
- **Runbook**: `docs/runbooks/runbook.md`

---

## Quick Start Checklist for Contributors

- Read `docs/governance/governance.md` and relevant ADRs before making changes.  
- Create a small, focused PR that updates code and docs together.  
- Fill the PR template checklist and link the story.  
- Request review from the appropriate Code Owner.

---

## How to Update This Document

- Edit via PR. Include rationale and link to any code changes.  
- Require one approving review from Docs Owner or Product Owner.  
- After merge, mention the change in the next sprint review.
