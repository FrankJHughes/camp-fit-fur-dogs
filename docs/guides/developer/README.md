# Developer Guide

> Onboarding guide and central reference for contributors.

## Quick Links

- [Actions Index](../../actions-index.md)
- [Action README Template](../../action-readme-template.md)
- [Workflow Conventions](../../conventions/workflow.md)
- [Architecture Conventions](../../conventions/architecture.md)
- [Code Conventions](../../conventions/code.md)
- [Documentation Conventions](../../conventions/docs.md)
- [Scrum Master Guide](../scrum-master-guide.md)
- [Product Owner Guide](../product-owner-guide.md)

---

## Getting Started

1. Clone the repository and install dependencies according to the root README.md.
2. Review the conventions listed under Quick Links before making changes.
3. Understand the PR Preview lifecycle by reading the Workflow Conventions.
4. Review the Architecture Conventions to understand the system’s DDD layering.
5. Understand the Story → Task → PR workflow described below.

---

## Repository Layout

.github/
  actions/          # Reusable composite actions
  workflows/        # CI/CD workflow definitions
docs/
  conventions/      # Project-wide conventions and standards
  guides/
    developer/      # This guide and related developer references
product/
  stories/          # Backlog stories (source of truth)
src/
  ...               # Backend, frontend, and SharedKernel source code
integration-tests/
  ...               # Infrastructure and API integration test suites

---

# Stories, Tasks, and PRs

This project uses a three-artifact execution model:

Story (repo file)
    ↓ decomposed into
Tasks (GitHub Issues)
    ↓ implemented by
PRs (one per Task)

## Stories

- Live in the repo (product/stories/)
- Represent user outcomes
- Contain acceptance criteria
- Are durable product artifacts
- Are never created as GitHub Issues

## Tasks

- Live as GitHub Issues
- Represent vertical slices of a story
- Are small enough to complete in 1–2 days
- Are the unit of sprint commitment
- Are created using the Task Issue Template
- Must reference exactly one story
- Must be labeled with:
  - task
  - sprint:N
  - domain labels (backend, frontend, infra, etc.)

## Pull Requests

- Implement exactly one Task
- Close the Task
- Reference the Story
- Cover a subset of acceptance criteria
- Follow the updated PR template
- Must pass CI
- Must follow architecture and code conventions

---

# Creating Tasks

Tasks are created during sprint planning or when new work is discovered.

A Task must:

- reference exactly one Story
- cover one or more acceptance criteria
- be mergeable independently
- be testable independently
- be small enough for a single PR

Use the Task Issue Template in .github/ISSUE_TEMPLATE/task.md.

---

# Branching & PR Workflow

## Branch Naming

task/<issue-number>-short-description

Example:

task/312-login-api-endpoint

## PR Requirements

Every PR must:

- Close the Task
- Reference the Story
- List acceptance criteria covered
- List remaining acceptance criteria
- Follow the updated PR template
- Pass CI
- Follow architecture and code conventions

## PR Size

A PR should:

- be reviewable in under 20 minutes
- touch only one vertical slice
- avoid unrelated changes

---

# Composite Actions

- All new custom actions must include a README created from the Action README Template.
- All new actions must be registered in the Actions Index.
- Composite actions must:
  - Use using: composite
  - Pin dependencies to full SHAs
  - Have minimal surface area
  - Include clear inputs, outputs, and failure modes
  - Be testable in isolation

---

# Workflows

All workflow changes must be documented in Workflow Conventions.

Workflows must follow:

- Universal Patch Rule
- Deterministic behavior
- Correct teardown/readiness rules
- Correct use of composite actions
- UTF-8 without BOM

---

# Code & Architecture

Follow the DDD layering rules defined in Architecture Conventions.

Backend code must respect:

- Domain purity
- CQRS pipelines
- SharedKernel primitives

Frontend code must follow:

- Layer + aggregate structure
- Purity rules
- Architecture conventions

---

# Documentation

All documentation must follow Documentation Conventions.

- UTF-8 without BOM
- Universal Patch Rule
- No drift between guides and conventions

---

# Additional Developer References

- Preview Troubleshooting Guide (if present)
- CI Dependency Graph Guide (if present)

---

# Summary

This Developer Guide provides the entry point for contributors.

All development work follows:

- Stories → Tasks → PRs
- System architecture rules
- Workflow and automation conventions
- Composite action standards
- Documentation and patching rules
