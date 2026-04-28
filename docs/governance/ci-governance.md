# CI Governance

This document defines how Continuous Integration (CI) behaves across the repository.  
It complements (but does not duplicate) the Workflow Conventions and the CI baseline established in US‑015.

CI governance ensures:

- Fast, predictable feedback  
- Correct test coverage  
- No silent regressions  
- No drift between code, stories, and workflows  
- Deterministic behavior across all contributors  

---

# 1. CI Principles

CI must be:

- **Deterministic** — same inputs produce the same outputs  
- **Fast** — avoid unnecessary work  
- **Selective** — run only what is relevant  
- **Comprehensive** — full coverage when required  
- **Transparent** — developers can see what ran and why  
- **Safe** — no skipped tests on `main`, no silent failures  

CI is a governance mechanism, not just automation.

---

# 2. CI Zones (Path-Based Classification)

Every file in the repository belongs to exactly one CI zone.

## Backend Zone
Paths:

- `src/CampFitFurDogs.*/**`
- `tests/CampFitFurDogs.*/**`

Triggers:

- Backend tests  
- SharedKernel tests (because backend depends on SharedKernel)

## Frontend Zone
Paths:

- `frontend/**`

Triggers:

- Frontend tests only

## SharedKernel Zone (Frank)
Paths:

- `src/SharedKernel/**`
- `tests/SharedKernel.*/**`

Triggers:

- SharedKernel tests  
- Backend tests (because backend depends on SharedKernel)

## Infra Zone
Paths:

- `.github/**`
- `Makefile`
- `docker-compose*`
- `*.sln`

Triggers:

- **All** test suites (infra changes affect the entire system)

## Docs-Only Zone
Paths:

- `*.md`
- `docs/**`
- `product/**`
- `LICENSE`
- `.editorconfig`

Triggers:

- **No** test suites

Docs-only changes must not trigger any test jobs.

---

# 3. CI Behavior Rules

## 3.1 Pull Requests
PRs use path-based filtering:

- Backend-only changes → backend + SharedKernel tests  
- Frontend-only changes → frontend tests  
- SharedKernel changes → SharedKernel + backend tests  
- Infra changes → all tests  
- Docs-only changes → no tests  

Skipped suites must be logged in the workflow summary.

## 3.2 Merges to `main`
Merges to `main` always run:

- Backend tests  
- Frontend tests  
- SharedKernel tests  

No skipping is allowed on the default branch.

## 3.3 Nightly Safety Net
A scheduled nightly run executes **all** suites, regardless of path filters.

This ensures:

- No long-term drift  
- No missed regressions  
- No dependency surprises  

---

# 4. Status Check Governance

Branch protection requires:

- All required checks must pass or be explicitly skipped  
- Skipped jobs must still appear as “skipped” (not omitted)  
- Jobs must use `if:` conditions, not dynamic job omission  

If a job is omitted entirely, GitHub treats it as missing and blocks the merge.

---

# 5. Test Suite Governance

Each suite must be:

- Fast  
- Deterministic  
- Isolated  
- Parallelizable where possible  
- Free of external dependencies except those defined in conventions  

Suites:

- **Backend** — domain, application, infrastructure, endpoint tests  
- **Frontend** — Vitest + React Testing Library  
- **SharedKernel** — primitives, dispatchers, validators, guardrails  

SharedKernel failures block backend merges.

---

# 6. CI and Story Governance Integration

CI must enforce:

- Changelog updates for user-facing changes  
- Story ID references in PRs  
- No drift between story metadata and code  
- No missing acceptance criteria in sprint stories  
- No untested behavior changes  

CI is a gatekeeper for story completeness.

---

# 7. CI and Repo Hygiene Integration

CI enforces:

- Correct encoding (`utf8NoBOM`)  
- Correct file paths  
- Correct catalog metadata  
- No duplicate story IDs  
- No missing story files  
- No drift between catalog and story frontmatter  

CI is the first line of defense against repository drift.

---

# 8. CI Workflow Hygiene

Workflows must:

- Use deterministic versions of actions  
- Avoid floating tags (e.g., `@v3` is allowed; `@latest` is not)  
- Use caching for Node and NuGet  
- Use path filters defined in governance  
- Avoid duplication across workflow files  
- Use consistent naming for jobs and steps  

Workflows must not:

- Contain business logic  
- Modify repository files  
- Generate artifacts that alter the working tree  

---

# 9. CI Failure Rules

A PR must fail if:

- A required suite fails  
- A required suite is missing  
- A changelog entry is required but missing  
- Story metadata is inconsistent  
- CI workflow logic is violated  
- A product boundary is crossed  
- A dependency direction is reversed  

CI failures are governance failures.

---

# 10. Governance Enforcement

- Reviewers enforce CI governance  
- CI enforces structural and behavioral rules  
- Product Owner enforces story and milestone alignment  
- Scripts enforce metadata correctness  

No PR may merge if CI governance is violated.

