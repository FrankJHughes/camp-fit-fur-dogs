# Multi-Product Governance

This document defines how multiple products coexist within the repository.  
Camp Fit Fur Dogs and Frank (SharedKernel) are **independent products** with shared infrastructure but separate lifecycles, boundaries, and governance.

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

## Frank (SharedKernel)
A standalone product providing:

- Domain primitives  
- CQRS abstractions  
- Dispatchers  
- Validation pipeline  
- EF Core base classes  
- Endpoint discovery infrastructure  
- Architecture guardrails  

Frank is not a folder — it is a **product** with its own:

- Changelog  
- Milestones  
- Backlog  
- Versioning  
- Release cycle  

---

# 2. Allowed Dependency Directions

The dependency graph must always follow:

```
Camp Fit Fur Dogs → Frank (SharedKernel)
```

Prohibited:

- Frank → Camp Fit Fur Dogs  
- Frontend → Backend internals  
- Backend → Frontend  
- Any product → Another product’s domain model  

Allowed:

- Backend → SharedKernel  
- Frontend → Backend API surface  
- Backend → Frontend only through HTTP responses  

This ensures SharedKernel remains clean, reusable, and product-agnostic.

---

# 3. Multi-Product Changelog Governance

Each product maintains its own changelog:

- `CHANGELOG.md` in the root for Camp Fit Fur Dogs  
- `CHANGELOG.md` inside the SharedKernel project for Frank  

Rules:

- A change affecting both products requires entries in both changelogs  
- A SharedKernel change must never appear in the Camp Fit Fur Dogs changelog  
- A Camp Fit Fur Dogs change must never appear in the SharedKernel changelog  
- Version numbers are independent  

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
- SharedKernel stories must not reference Camp Fit Fur Dogs behavior  
- Camp Fit Fur Dogs stories must not reference SharedKernel internals  
- Dependencies must not cross product boundaries except:  
  - Camp Fit Fur Dogs may depend on SharedKernel stories  
  - SharedKernel may not depend on Camp Fit Fur Dogs stories  

---

# 6. Multi-Product CI Governance

CI must treat products independently:

- Backend tests run for backend changes  
- Frontend tests run for frontend changes  
- SharedKernel tests run for SharedKernel changes  
- Infra changes run all suites  
- Docs-only changes run none  

SharedKernel changes must trigger:

- SharedKernel tests  
- Backend tests (because backend depends on SharedKernel)  

Camp Fit Fur Dogs changes must not trigger SharedKernel tests.

---

# 7. Multi-Product Release Governance

Each product releases independently.

Camp Fit Fur Dogs release:

- Version bump in root `CHANGELOG.md`  
- Deployment of backend + frontend  
- Release notes referencing story IDs  

Frank release:

- Version bump in SharedKernel `CHANGELOG.md`  
- NuGet package version bump (future)  
- Release notes referencing story IDs  

Rules:

- A SharedKernel release must not require a Camp Fit Fur Dogs release  
- A Camp Fit Fur Dogs release must not require a SharedKernel release  
- Breaking changes in SharedKernel require a major version bump  

---

# 8. Multi-Product Documentation Governance

Documentation must:

- Clearly distinguish product boundaries  
- Avoid mixing product-specific rules  
- Reference SharedKernel only where appropriate  
- Avoid duplicating SharedKernel documentation in product docs  

SharedKernel documentation lives with SharedKernel.  
Camp Fit Fur Dogs documentation lives under `docs/`.

---

# 9. Multi-Product Governance Enforcement

- Reviewers enforce product boundaries  
- CI enforces dependency direction  
- Scripts enforce metadata correctness  
- Product Owner enforces milestone separation  
- Architecture tests enforce layer boundaries  

No PR may merge if:

- A product boundary is violated  
- A dependency direction is reversed  
- A changelog entry is placed in the wrong product  
- A story mixes concerns from multiple products  

