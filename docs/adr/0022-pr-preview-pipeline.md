# ADR‑0022: PR Preview Pipeline (Neon + Render)

**Status:** Accepted  
**Date:** 2026‑04‑30  
**Supersedes:** Earlier assumptions about PR Preview DB lifecycle, API readiness, and test responsibilities

---

## Context

The system requires a **deterministic, production‑like PR Preview pipeline** that provides:

- An **isolated PostgreSQL preview database** per PR (Neon branch)
- A **temporary API instance** deployed via Render PR Previews
- **Infrastructure Integration Tests** running against the real Neon preview DB
- **API Integration Tests** running against the real hosted API
- The ability to **fix broken migrations inside the same PR**
- **Automatic cleanup** of all preview resources when the PR closes
- A **stable readiness signal** for both teardown and startup of the API

Render PR Previews are configured in **Manual** mode.  
This means:

- Render **creates** a preview instance when the PR has the `render-preview` label  
- Render **destroys** the preview instance when the label is removed  
- Render **redeploys** the preview on PR synchronize events *only if the label is present*

The workflow therefore controls deployment by controlling the label.

Earlier versions of the pipeline suffered from:

1. **Persistent Neon preview branches**, causing schema drift and migration conflicts  
2. **Infrastructure tests running migrations**, duplicating workflow behavior  
3. **Unstable API readiness**, due to Render returning intermittent `200`/`502` responses during container warmup  
4. **Inconsistent connection string formats**, requiring manual conversion between PostgreSQL URI and Npgsql formats  

These issues made PR previews unreliable and prevented iterative migration development.

---

## Decision

### 1. PR Preview lifecycle is controlled by the `render-preview` label

- **Label removed** → Render destroys the preview instance  
- **Label added** → Render creates a new preview instance  
- **Label present on synchronize** → Render redeploys the preview  

The workflow orchestrates this intentionally.

### 2. Destroy stale API before provisioning new resources

The workflow:

1. Removes the `render-preview` label  
2. Waits for the old API to be fully destroyed  
   - Probes `/health`  
   - Healthy = `404`  
   - Requires **3 consecutive** successes  
   - Timeout = **300 seconds**

This ensures the old preview is completely gone before provisioning a new DB or API.

### 3. Recreate the Neon preview branch on every workflow run

The workflow:

- Deletes the old Neon branch (if any)  
- Creates a fresh branch named `pr-<number>`  
- Sets expiration to 14 days  

This ensures:

- Clean, deterministic database state  
- No schema drift  
- Ability to fix migrations inside the same PR  

### 4. Workflow owns all schema creation

The workflow executes:

```bash
dotnet ef database update
```

Infrastructure Integration Tests **must not** run migrations.  
They assume the schema is already correct.

### 5. Connection string conversion is handled by a composite action

Neon provides a **PostgreSQL URI**:

```text
postgres://user:pass@host:5432/db?sslmode=require
```

The workflow converts it to an **Npgsql/ADO.NET connection string**:

```text
Host=...;Port=5432;Database=...;Username=...;Password=...;Ssl Mode=Require;Pooling=true;Timeout=15;
```

This ensures compatibility with EF Core and the API.

### 6. Infrastructure Integration Tests run against the fresh Neon preview DB

They validate:

- EF Core mappings  
- Persistence logic  
- Repository behavior  

They do **not**:

- Modify schema  
- Spin up the API  
- Use `WebApplicationFactory`  

### 7. Deploy fresh API by adding the `render-preview` label

The workflow:

1. Adds the `render-preview` label  
2. Render automatically builds and deploys the preview instance  
3. The workflow waits for the API to become stable  

### 8. API readiness requires three consecutive healthy responses

The workflow:

- Probes the real API route `/api/dogs`  
- Accepts `200,400,401` as healthy  
- Requires **3 consecutive** successes  
- Timeout = **300 seconds**

This accounts for Render’s:

- Cold start  
- Container swap  
- Route propagation delays  
- Intermittent `200`/`502` bounce pattern  

### 9. API Integration Tests hit the real hosted API

Tests use:

```text
API_BASE_URL=https://campfitfurdogsapi-pr-<PR>.onrender.com
```

They validate:

- Routing  
- Controllers  
- Middleware  
- Authentication/authorization behavior  
- End‑to‑end flows  

### 10. Cleanup occurs only when the PR is closed

On PR close:

- Neon preview branch is deleted  
- Render preview instance is destroyed (via label removal)  
- Artifacts are deleted  

No preview resources persist beyond the PR lifecycle.

---

## Rationale

This architecture:

- Ensures **repeatable, deterministic** CI runs  
- Prevents schema conflicts and migration duplication  
- Provides **production‑like** API behavior for integration tests  
- Handles Render’s cold‑start and container‑swap behavior safely  
- Cleanly separates responsibilities:
  - Workflow → schema creation + deployment orchestration  
  - Infrastructure tests → persistence correctness  
  - API tests → end‑to‑end correctness  
- Eliminates manual cleanup and resource leakage  
- Uses Render’s Manual PR Preview mode as an intentional deployment switch  

---

## Consequences

### Positive

- PR previews are stable, reproducible, and production‑like  
- Broken migrations can be fixed in the same PR  
- No leftover schema from previous runs  
- Infrastructure tests are clean and focused  
- API tests validate the real deployed environment  
- Automatic cleanup prevents resource leaks  
- Readiness detection is robust against Render’s intermittent 502s  
- Label‑driven deployment provides explicit control and predictability  

### Negative

- Recreating the Neon branch adds ~1–2 seconds of overhead  
- Preview DB data is not persisted between workflow runs  
- Render PR Previews may require up to 300 seconds to stabilize  
- Developers must rely on local DBs for iterative data testing  

---

## Alternatives Considered

### 1. Keeping the Neon branch across runs  
Rejected due to schema drift and inability to fix migrations.

### 2. Running migrations inside Infrastructure tests  
Rejected because it duplicates workflow behavior and causes conflicts.

### 3. Using per‑test schemas  
Unnecessary once the workflow owns schema creation.

### 4. Using automatic PR Preview mode  
Rejected because manual label‑driven deployment provides better control and avoids unintended redeployments.

---

## Notes

This ADR formalizes the **current** PR Preview pipeline architecture and replaces any previous assumptions that:

- Infrastructure tests should run migrations  
- Neon preview branches should persist across workflow runs  
- API Integration Tests should use `WebApplicationFactory`  
- A single `200` response is sufficient for API readiness  
- PostgreSQL URIs can be used directly by EF Core  
- Render PR Previews deploy automatically without label control  
