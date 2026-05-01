# ADR-0022 — PR Preview Pipeline with Neon, Render, and Layered Integration Tests

## Status
Accepted

## Context

The project requires a zero‑cost, fully automated PR Preview environment that validates changes across backend layers before merging into `main`. The existing CI baseline (US‑015) provides build and test automation, but does not provide:

- A hosted database for integration testing  
- A hosted API instance for end‑to‑end verification  
- A mechanism to run Infrastructure and API integration tests against real cloud resources  
- Automatic cleanup of preview environments  
- A unified workflow that ties Neon (database) and Render (API hosting) together  

Infrastructure stories US‑139 (Frontend Hosting), US‑140 (API Hosting), and US‑141 (Database Hosting) all require a hosted environment.  
US‑140 explicitly states:

> “Platform decision should be recorded as an ADR.”

Documentation conventions require ADRs to capture significant architectural decisions.  
Workflow conventions require full‑file regeneration and script‑first automation.

The team also recently split integration tests into:

- **Infrastructure Integration Tests** — persistence, EF Core, handlers  
- **API Integration Tests** — routing, middleware, DI, HTTP behavior  

This ADR formalizes the pipeline that supports both.

## Decision

We adopt a **PR Preview Pipeline** that provisions a temporary Neon database branch and a temporary Render API instance for every open PR. The pipeline runs in three phases:

### 1. Neon Preview Database Creation
- A Neon branch is created per PR using the naming convention:  
  `preview/pr-<PR_NUMBER>-<branch_name>`
- The branch expires automatically after 14 days.
- The connection string is converted into a .NET‑compatible format and injected into the CI environment.

### 2. Infrastructure Integration Tests (CI)
- EF Core migrations run against the Neon preview database.
- Infrastructure integration tests run against the same database.
- If these tests fail, the pipeline stops and the API is not deployed.

### 3. Render PR Preview Deployment
- The API is deployed to Render as a PR Preview instance.
- The connection string is patched into the Render service using the Render API.
- A deploy is triggered automatically.
- The preview URL follows the deterministic pattern:  
  `https://campfitfurdogs-api-pr-<PR_NUMBER>.onrender.com`

### 4. API Integration Tests (Live HTTP Tests)
- A separate job runs after the Render deploy.
- Tests use the environment variable:  
  `API_BASE_URL=https://campfitfurdogs-api-pr-<PR_NUMBER>.onrender.com`
- Tests validate:
  - Routing
  - Middleware
  - Authorization
  - Endpoint behavior
  - Health checks
  - DI configuration

### 5. Automatic Cleanup on PR Close
- The Neon preview branch is deleted.
- The Render PR Preview instance is deleted.
- No preview resources persist after merge or closure.

### 6. Workflow File
The pipeline is defined in:

```
.github/workflows/pr-preview.yml
```

This replaces the previous `neon.yml` workflow.

## Consequences

### Positive
- Full vertical slice validation before merging into `main`.
- Zero‑cost: both Neon and Render free tiers support ephemeral environments.
- Deterministic: every PR gets its own isolated database and API instance.
- Safe: no shared state between PRs.
- Aligned with conventions:
  - Architecture: layered testing (Infrastructure vs API)
  - Workflow: script‑first, deterministic automation
  - Documentation: ADR captures the decision
- Improved developer confidence: failures surface early and clearly.

### Negative
- Pipeline runtime increases due to:
  - Neon provisioning
  - Render deployment
  - Live HTTP tests
- Render PR Preview deploys may experience cold starts.
- Requires maintaining API_BASE_URL conventions across tests.

### Neutral / Tradeoffs
- The team must maintain two integration test suites.
- Render free tier sleeps after inactivity; acceptable for PR validation.

## Alternatives Considered

### 1. Local ephemeral containers (Docker Compose)
Rejected because:
- Does not validate real hosting behavior.
- Does not test cold starts, routing, or platform differences.
- Does not support frontend hosting integration.

### 2. Azure App Service Free Tier
Rejected because:
- No free custom domain.
- Cold start behavior inconsistent.
- More complex provisioning.

### 3. Fly.io
Rejected for now:
- Strong option, but Render PR Preview is simpler and integrates better with GitHub.

## Notes

- This ADR supports US‑139, US‑140, and US‑141.
- Future ADRs may extend this pipeline to:
  - Frontend PR Previews
  - End‑to‑end browser tests
  - Load testing
- This ADR must be updated if:
  - Hosting platform changes
  - Database provider changes
  - Test taxonomy changes

## Decision Drivers

- Zero cost  
- Predictability  
- Developer experience  
- Alignment with conventions  
- Full vertical slice validation  
- Automatic cleanup  

## References

- Documentation Conventions  
- Workflow Conventions  
- Architecture Conventions  
- US‑140: API Hosting  
- US‑141: Database Hosting  
- US‑015: CI Baseline  
