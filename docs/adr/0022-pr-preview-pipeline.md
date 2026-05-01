# ADR‑0022: PR Preview Pipeline (Neon + Render)

**Status:** Accepted  
**Date:** 2026‑04‑30  
**Supersedes:** Earlier assumptions about PR Preview DB lifecycle and test responsibilities

---

## Context

The system requires a reliable PR Preview pipeline that:

- Creates an **isolated PostgreSQL preview database** for each PR  
- Deploys a **temporary API instance** to Render  
- Runs **Infrastructure Integration Tests** against the real database  
- Runs **API Integration Tests** against the real hosted API  
- Supports **fixing broken migrations inside the same PR**  
- Cleans up all preview resources when the PR is closed  

The previous pipeline had two major issues:

1. **Neon preview branches persisted across workflow runs**, causing schema drift and “relation already exists” errors when migrations changed.  
2. **Infrastructure Integration Tests ran migrations**, duplicating workflow behavior and causing failures when the workflow had already applied them.

These issues made PR previews unreliable and prevented iterative migration development within the same PR.

---

## Decision

### 1. Recreate the Neon preview branch on every workflow run  
The workflow now:

- Deletes the preview branch if it exists  
- Creates a fresh preview branch  
- Applies migrations once  

This ensures:

- A clean database state  
- No schema drift  
- Deterministic CI behavior  
- Ability to fix bad migrations inside the same PR  

### 2. Workflow owns all schema creation  
The workflow runs:

```
dotnet ef database update
```

Infrastructure Integration Tests **must not** run migrations.  
They assume the schema is already correct.

### 3. Infrastructure Integration Tests run against the fresh Neon preview DB  
They validate:

- EF Core mappings  
- Persistence logic  
- Repository behavior  

They do **not**:

- Modify schema  
- Spin up the API  
- Use WebApplicationFactory  

### 4. Render PR Preview is deployed using the same Neon preview branch  
The API preview receives the Neon connection string via Render’s preview API.

### 5. API Integration Tests hit the real hosted API  
Tests use:

```
API_BASE_URL=https://campfitfurdogs-api-pr-<PR>.onrender.com
```

They validate:

- Routing  
- Controllers  
- Middleware  
- End‑to‑end behavior  

### 6. Cleanup occurs only when the PR is closed  
On PR close:

- Neon preview branch is deleted  
- Render preview instance is deleted  

No preview resources persist beyond the PR lifecycle.

---

## Rationale

This architecture:

- Ensures **repeatable, deterministic** CI runs  
- Prevents schema conflicts and migration duplication  
- Matches the separation of concerns between:
  - Workflow (schema creation)
  - Infrastructure tests (persistence)
  - API tests (end‑to‑end behavior)
- Supports iterative development of migrations within the same PR  
- Aligns with Neon’s and Render’s intended preview‑environment usage  
- Eliminates manual cleanup and developer friction  

---

## Consequences

### Positive
- PR previews are stable and reproducible  
- Broken migrations can be fixed in the same PR  
- No leftover schema from previous runs  
- Infrastructure tests are clean and focused  
- API tests validate the real deployed environment  
- Automatic cleanup prevents resource leaks  

### Negative
- Recreating the Neon branch adds ~1–2 seconds of overhead  
- Preview DB data is not persisted between workflow runs  
- Developers must rely on local DBs for iterative data testing  

---

## Alternatives Considered

### 1. Keeping the Neon branch across runs  
Rejected due to schema drift and inability to fix migrations.

### 2. Running migrations inside Infrastructure tests  
Rejected because it duplicates workflow behavior and causes conflicts.

### 3. Using per‑test schemas  
Unnecessary once the workflow owns schema creation.

---

## Notes

This ADR formalizes the new PR Preview pipeline architecture and replaces any previous assumptions that:

- Infrastructure tests should run migrations  
- Neon preview branches should persist across workflow runs  
- API Integration Tests should use WebApplicationFactory  
