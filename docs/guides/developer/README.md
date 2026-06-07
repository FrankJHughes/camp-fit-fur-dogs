# Developer Guide

> Onboarding guide and central reference for contributors.

## Quick Links

- [Actions Index](ca://s?q=Open_Actions_Index)
- [Action README Template](ca://s?q=Open_Action_README_Template)
- [Workflow Conventions](ca://s?q=Open_Workflow_Conventions)
- [Architecture Conventions](ca://s?q=Open_Architecture_Conventions)
- [Code Conventions](ca://s?q=Open_Code_Conventions)
- [Documentation Conventions](ca://s?q=Open_Documentation_Conventions)
- [Scrum Master Guide](ca://s?q=Open_Scrum_Master_Guide)
- [Product Owner Guide](ca://s?q=Open_Product_Owner_Guide)

---

# Developer Documentation Structure

The `developer` folder contains several focused sub‑areas:

### Architecture  
Core architectural explanations and subsystem architecture  
→ [Architecture Guides](ca://s?q=Open_Architecture_Guides)

### Authentication  
OIDC login, callback flow, session management  
→ [Authentication Guides](ca://s?q=Open_Authentication_Guides)

### Testing  
Testing strategy across API, Application, Domain, and frontend  
→ [Testing Guides](ca://s?q=Open_Testing_Guides)

### Operations  
Local development, hosting, troubleshooting, secrets  
→ [Operations Guides](ca://s?q=Open_Operations_Guides)

### CI/CD  
GitHub Actions, dependency graph, workflow documentation  
→ [CI/CD Guides](ca://s?q=Open_CI_CD_Guides)

### Forms  
Frontend form architecture and validation  
→ [Form Guides](ca://s?q=Open_Form_Guides)

### Feature Guides  
Vertical slice walkthroughs and slice‑level abstractions  
→ [Feature Guides](ca://s?q=Open_Feature_Guides)

---

# Getting Started

1. Clone the repository and install dependencies according to the root README.md.
2. Review the conventions listed under Quick Links before making changes.
3. Understand the PR Preview lifecycle via the Workflow Conventions.
4. Review the Architecture Conventions to understand the system’s DDD layering.
5. Understand the Story → Task → PR workflow described below.
6. Explore the Architecture Guides to understand vertical slices, CQRS, and Frank primitives.
7. Run the system locally using the Operations Guides.

---

# Repository Layout

```
.github/
  actions/          # Reusable composite actions
  workflows/        # CI/CD workflow definitions

docs/
  conventions/      # Project-wide conventions and standards
  guides/
    developer/      # Developer documentation (this folder)
      architecture/
      authentication/
      testing/
      operations/
      ci-cd/
      forms/
      feature-guides/

product/
  stories/          # Backlog stories (source of truth)

src/
  ...               # Backend, frontend, and Frank source code

integration-tests/
  ...               # Infrastructure and API integration test suites
```

---

# Stories, Tasks, and PRs

This project uses a three-artifact execution model:

```
Story (repo file)
    ↓ decomposed into
Tasks (GitHub Issues)
    ↓ implemented by
PRs (one per Task)
```

## Stories

- Live in the repo (`product/stories/`)
- Represent user outcomes
- Contain acceptance criteria
- Are durable product artifacts
- Are never created as GitHub Issues
- Must follow Story Grammar and Story Governance

## Tasks

- Live as GitHub Issues
- Represent vertical slices of a story
- Small enough to complete in 1–2 days
- Unit of sprint commitment
- Created using the Task Issue Template
- Must reference exactly one story
- Must include labels:
  - `task`
  - `sprint:N`
  - domain labels (`backend`, `frontend`, `infra`, etc.)
- Must be independently mergeable and testable

## Pull Requests

- Implement exactly one Task
- Close the Task
- Reference the Story
- List acceptance criteria covered
- List remaining acceptance criteria
- Follow the PR template
- Pass CI
- Follow architecture and code conventions
- Include only changes relevant to the Task

---

# Creating Tasks

Tasks are created during sprint planning or when new work is discovered.

A Task must:

- reference exactly one Story  
- cover one or more acceptance criteria  
- be mergeable independently  
- be testable independently  
- be small enough for a single PR  

Use the Task Issue Template in:

```
.github/ISSUE_TEMPLATE/task.md
```

---

# Branching & PR Workflow

## Branch Naming

```
task/<issue-number>-short-description
```

Example:

```
task/312-login-api-endpoint
```

## PR Requirements

Every PR must:

- Close the Task  
- Reference the Story  
- List acceptance criteria covered  
- List remaining acceptance criteria  
- Follow the PR template  
- Pass CI  
- Follow architecture and code conventions  

## PR Size

A PR should:

- be reviewable in under 20 minutes  
- touch only one vertical slice  
- avoid unrelated changes  
- avoid refactors unless explicitly part of the Task  

---

# Composite Actions

- All new custom actions must include a README created from the Action README Template.
- All new actions must be registered in the Actions Index.
- Composite actions must:
  - Use `using: composite`
  - Pin dependencies to full SHAs
  - Have minimal surface area
  - Include clear inputs, outputs, and failure modes
  - Be testable in isolation
  - Avoid inline shell logic
  - Follow script‑first patterns

---

# Workflows

All workflow changes must be documented in Workflow Conventions.

Workflows must follow:

- Universal Patch Rule  
- Deterministic behavior  
- Correct teardown/readiness rules  
- Correct use of composite actions  
- UTF‑8 without BOM  
- Script‑first execution  
- Minimal permissions  
- Stable artifact naming  

---

# Code & Architecture

Backend code must respect:

- Domain purity  
- CQRS pipelines  
- Frank primitives  
- Repository and reader boundaries  
- Handler purity  
- Endpoint purity  

Frontend code must follow:

- Layer + aggregate structure  
- Purity rules  
- Architecture conventions  
- Form architecture  
- Query architecture  
- API client conventions  

See the [Architecture Guides](ca://s?q=Open_Architecture_Guides) for deeper explanations.

---

# Documentation

All documentation must follow Documentation Conventions:

- UTF‑8 without BOM  
- Universal Patch Rule  
- No drift between guides and conventions  
- Clear separation between governance, conventions, and guides  
- Script‑safe fencing and quoting rules  

---

# Additional Developer References

- [Preview Troubleshooting](ca://s?q=Open_Preview_Troubleshooting)
- [CI Dependency Graph](ca://s?q=Open_CI_Dependency_Graph)
- [Frank Architecture](ca://s?q=Open_Frank_Architecture)
- [Vertical Slice Index](ca://s?q=Open_Vertical_Slice_Index)
- [Dispatcher Pipeline Guide](ca://s?q=Open_Dispatcher_Pipeline_Guide)

---

# Summary

This Developer Guide provides the entry point for contributors.

All development work follows:

- Stories → Tasks → PRs  
- System architecture rules  
- Workflow and automation conventions  
- Composite action standards  
- Documentation and patching rules  

The developer documentation, conventions, and governance together ensure a stable, predictable, and maintainable system.
