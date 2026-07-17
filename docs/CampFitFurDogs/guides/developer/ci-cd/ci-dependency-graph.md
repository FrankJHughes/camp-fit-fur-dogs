# CI Dependency Graph Guide

> How the CI dependency graph works, why it exists, and how to maintain it.

The CI Dependency Graph ensures that **test execution order** in `ci.yaml` matches the **architectural dependency order** of the codebase.  
It prevents accidental drift between:

- The **actual architecture**  
- The **intended test sequencing**  
- The **workflow logic**  

This guide explains how the graph works, how the validator enforces it, and how to update it safely.

---

# Purpose of the CI Dependency Graph

The dependency graph defines **which test suites depend on which others**.

Example:

```json
{
  "shared-kernel": [],
  "backend": ["shared-kernel"],
  "frontend": ["backend"]
}
```

This ensures:

- Frank tests run first  
- Backend tests run only after Frank  
- Frontend tests run only after Backend  

The graph is the **single source of truth** for CI test ordering.

---

# Where the Graph Lives

The dependency graph is stored in:

```
.github/ci/ci-deps.json
```

Example structure:

```json
{
  "shared-kernel": [],
  "backend": ["shared-kernel"],
  "frontend": ["backend"]
}
```

Each key represents a **test job** in CI.

---

# How the Validator Works

The validator script:

```
scripts/ci/validate-ci-dependencies.mjs
```

runs automatically in `ci.yaml` before any tests execute.

It performs three checks:

---

## 1. **Graph Completeness**

Every test job in CI must appear in `ci-deps.json`.

If a job exists in CI but not in the graph:

```
Error: Job 'backend' missing from ci-deps.json
```

---

## 2. **No Extra Dependencies**

If a CI job declares dependencies not listed in the graph:

```
Error: Job 'backend' has extra dependencies not in ci-deps.json: determine-changes
```

This prevents accidental coupling between jobs.

---

## 3. **Graph Consistency**

If the graph says:

```
frontend depends on backend
```

then CI must contain:

```yaml
needs: [backend]
```

If not:

```
Error: Workflow missing dependency: frontend depends on backend
```

---

# How to Update the Graph

You must update the graph when:

- A new test suite is added  
- A test suite is removed  
- A dependency between layers changes  
- A new product layer is introduced  

### Steps

1. Open `.github/ci/ci-deps.json`
2. Add or update the dependency entry
3. Ensure the CI workflow (`ci.yaml`) reflects the same dependency
4. Run CI to confirm the validator passes

---

# Example: Adding a New Test Suite

Suppose you add:

```
integration-tests
```

and it depends on backend.

Update `ci-deps.json`:

```json
{
  "shared-kernel": [],
  "backend": ["shared-kernel"],
  "integration-tests": ["backend"],
  "frontend": ["backend"]
}
```

Update `ci.yaml`:

```yaml
integration-tests:
  needs: [backend]
```

Commit both changes together.

---

# Common Failure Modes

## 1. CI job depends on orchestration jobs

Example:

```
Error: Job 'backend' has extra dependencies not in ci-deps.json: determine-changes
```

**Fix:**  
Remove orchestration jobs (`determine-changes`, `validate-ci-deps`) from `needs:`.

Only **test jobs** belong in the graph.

---

## 2. Graph updated but CI not updated

```
Error: Workflow missing dependency: frontend depends on backend
```

**Fix:**  
Add the missing `needs:` entry in `ci.yaml`.

---

## 3. CI updated but graph not updated

```
Error: Job 'integration-tests' missing from ci-deps.json
```

**Fix:**  
Add the job to the graph.

---

## 4. Wrong dependency direction

If you accidentally reverse a dependency:

```
Error: Workflow missing dependency: backend depends on frontend
```

**Fix:**  
Correct the graph to match architectural direction.

---

# Best Practices

- Keep the graph **minimal** — only real architectural dependencies.  
- Never include orchestration jobs (`determine-changes`, `validate-ci-deps`).  
- Update the graph **whenever** test suites change.  
- Commit graph + workflow changes together.  
- Use the Universal Patch Rule: regenerate entire files.

---

# Summary

The CI Dependency Graph ensures:

- Correct test ordering  
- Architectural consistency  
- No accidental coupling  
- Deterministic CI behavior  

Maintaining the graph is essential for keeping CI aligned with the system architecture.

