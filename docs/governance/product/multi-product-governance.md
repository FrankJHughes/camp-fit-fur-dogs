# Multi-Product Governance

This document defines how multiple products coexist within the repository.  
Camp Fit Fur Dogs and Frank (Frank) are **independent products** with shared infrastructure but separate lifecycles, boundaries, and governance.

This governance ensures:

- Clear separation of concerns  
- Predictable dependency direction  
- Independent release cycles  
- No cross-product leakage  
- Stable architecture over time  

---

# 1. Product Definitions

## Camp Fit Fur Dogs (Primary Product)
The customer-facing application consisting of:

- Backend (ASP.NET Core API)  
- Frontend (Next.js)  
- Product-specific domain logic  
- Booking, customer, and operational features  

## Frank (Frank)
A standalone product providing:

- Domain primitives  
- CQRS abstractions  
- Dispatchers  
- Validation pipeline  
- DI auto‑registration engine  
- `[AutoRegister]` attribute  
- EF Core configuration scanning  
- Endpoint discovery infrastructure  
- Hosting provider infrastructure  
- Architecture guardrails  
- Error boundary helpers  

Frank is not a folder — it is a **product** with its own:

- Changelog  
- Milestones  
- Backlog  
- Versioning  
- Release cycle  

Frank must remain product‑agnostic and must not depend on Camp Fit Fur Dogs.

---

# 2. Allowed Dependency Directions

The dependency graph must always follow:

```
Camp Fit Fur Dogs → Frank (Frank)
```

Prohibited:

- Frank → Camp Fit Fur Dogs  
- Frontend → Backend internals  
- Backend → Frontend  
- Any product → Another product’s domain model  
- Frank → product-specific abstractions  
- Manual DI registration of Frank services in Camp Fit Fur Dogs  

Allowed:

- Backend → Frank  
- Frontend → Backend API surface  
- Backend → Frontend only through HTTP responses  

This ensures Frank remains clean, reusable, and product-agnostic.

---

# 3. Multi-Product Changelog Governance

Each product maintains its own changelog:

- `CHANGELOG.md` in the root for Camp Fit Fur Dogs  
- `CHANGELOG.md` inside the Frank project for Frank  

Rules:

- A change affecting both products requires entries in both changelogs  
- A Frank change must never appear in the Camp Fit Fur Dogs changelog  
- A Camp Fit Fur Dogs change must never appear in the Frank changelog  
- Version numbers are independent  
- Frank breaking changes require a major version bump  

Cross-product drift is prohibited.

---

# 4. Multi-Product Milestone Governance

Each product has its own milestone system.

Camp Fit Fur Dogs milestones:

- M0 — Foundation  
- M1 — Authentication & Core UX  
- M2 — Hosting, Deployment, Security  
- M3 — Booking & Customer Experience  

Frank milestones:

- Foundation  
- Infrastructure  
- CQRS & Dispatchers  
- EF Core Integration  
- Endpoint Discovery  
- Architecture Guardrails  
- Hosting Provider Infrastructure  

Rules:

- A story belongs to exactly one product  
- A story belongs to exactly one milestone  
- Milestones must not mix products  
- Milestones must not depend on each other across products  

---

# 5. Multi-Product Backlog Governance

Each product has its own backlog:

- Camp Fit Fur Dogs stories live under `product/stories/**`  
- Frank stories live under `product/stories/shared-kernel/**`  

Rules:

- Stories must not mix concerns from both products  
- Frank stories must not reference Camp Fit Fur Dogs behavior  
- Camp Fit Fur Dogs stories must not reference Frank internals  
- Dependencies must not cross product boundaries except:  
  - Camp Fit Fur Dogs may depend on Frank stories  
  - Frank may not depend on Camp Fit Fur Dogs stories  

---

# 6. Multi-Product CI Governance

CI must treat products independently:

- Backend tests run for backend changes  
- Frontend tests run for frontend changes  
- Frank tests run for Frank changes  
- Infra changes run all suites  
- Docs-only changes run none  

Frank changes must trigger:

- Frank tests  
- Backend tests (because backend depends on Frank)  
- Frontend tests (because Frank affects endpoint discovery and DI)  

Camp Fit Fur Dogs changes must not trigger Frank tests.

---

# 7. Multi-Product Release Governance

Each product releases independently.

Camp Fit Fur Dogs release:

- Version bump in root `CHANGELOG.md`  
- Deployment of backend + frontend  
- Release notes referencing story IDs  

Frank release:

- Version bump in Frank `CHANGELOG.md`  
- NuGet package version bump (future)  
- Release notes referencing story IDs  

Rules:

- A Frank release must not require a Camp Fit Fur Dogs release  
- A Camp Fit Fur Dogs release must not require a Frank release  
- Breaking changes in Frank require a major version bump  
- Frank releases must maintain DI compatibility  
- Frank releases must maintain EF Core configuration compatibility  
- Frank releases must maintain hosting provider compatibility  

---

# 8. Multi-Product Documentation Governance

Documentation must:

- Clearly distinguish product boundaries  
- Avoid mixing product-specific rules  
- Reference Frank only where appropriate  
- Avoid duplicating Frank documentation in product docs  

Frank documentation lives with Frank.  
Camp Fit Fur Dogs documentation lives under `docs/`.  
Canonical conventions live under `docs/conventions/`.

Guides must not mix product boundaries.

---

# 9. Multi-Product Governance Enforcement

- Reviewers enforce product boundaries  
- CI enforces dependency direction  
- Scripts enforce metadata correctness  
- Product Owner enforces milestone separation  
- Architecture tests enforce layer boundaries  
- Frank guardrails enforce DI and hosting provider correctness  

No PR may merge if:

- A product boundary is violated  
- A dependency direction is reversed  
- A changelog entry is placed in the wrong product  
- A story mixes concerns from multiple products  
- Frank is polluted with product-specific logic  
- Camp Fit Fur Dogs bypasses Frank DI or hosting provider infrastructure
