# Governance — Camp Fit Fur Dogs

**Purpose**
This document is the single source of truth for project process, policy, and ownership. It defines how we make architecture choices, run sprints, accept work, and keep the repository healthy and consistent. Treat this file as a living artifact: update it by PR whenever code or process changes.

---

## Scope and Audience

**Scope**
Covers project roles, sprint cadence, ADR process, documentation rules, security, CI, and enforcement mechanisms.

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

See the [Pull Request Template](../../.github/PULL_REQUEST_TEMPLATE.md) for the enforced DoD checklist. Every story or PR must satisfy it before merge.

---

## Definition of Ready

See the [Product Owner Guide — Definition of Ready](../guides/product-owner-guide.md#definition-of-ready) for the full readiness criteria table.

---

## Source Control, Branching, and PR Rules

See [copilot-instructions.md — Source Control](../../.github/copilot-instructions.md) for the authoritative branch strategy, branch protection settings, PR conventions, and emergency override process.

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

## CI, Deployments, and Environments

- CI runs on PRs and merges to `main`. CI must run build, tests, and lint.
- Use preview environments for feature branches when possible. Provision ephemeral DB branches for integration tests if supported.
- Deployments to public demo environment require passing CI and an approved PR.

---

## Enforcement and Automation

- **CODEOWNERS** enforces required reviewers for critical paths.
- **CI checks** include build, tests, markdown lint, and optional doc coverage checks.
- **PR template** includes DoD checklist and changelog reminder.
- **Branch protection** on `main` blocks direct pushes, requires passing CI, requires CODEOWNERS approval, and dismisses stale reviews. See [copilot-instructions.md](../../.github/copilot-instructions.md) for full settings table.
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