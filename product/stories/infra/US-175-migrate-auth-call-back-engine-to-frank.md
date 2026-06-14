# US‑175 — Frank: Auth Callback Engine Migration

## Story Grammar

As an **architect**, I must be able to **move the authentication callback pipeline into Frank as a governed engine** so that **all products can reuse a deterministic, invariant‑checked, secure callback orchestration model**.

---

# Intent

The current authentication callback pipeline (`AuthCallbackExecutor`, `AuthCallbackService`, `IAuthCallbackStep`, `AuthCallbackContext`) lives inside the product.  
It is:

- deterministic  
- invariant‑checked  
- pure orchestration  
- cross‑cutting  
- reusable  
- security‑critical  

This makes it a **Frank Engine**, not product logic.

This story migrates the entire callback pipeline into Frank under a new subsystem:

````text
Frank/Authentication/AuthCallbackEngine/
````

---

# Motivation

- Multiple products will require OIDC callback orchestration  
- The pipeline is pure and cross‑cutting  
- The pipeline enforces invariants and security boundaries  
- The pipeline is step‑driven and extensible  
- The pipeline is not domain‑specific  
- The pipeline must be governed, documented, and reusable  

---

# Scope

This story includes:

- Moving the following types into Frank:
  - `AuthCallbackExecutor`
  - `AuthCallbackService`
  - `IAuthCallbackStep`
  - `AuthCallbackContext`
  - `AuthCallbackDiagnosticEvent`
- Creating a new Frank engine folder:
  - `Frank/Authentication/AuthCallbackEngine/`
- Creating engine‑level documentation:
  - `AuthCallbackEngineGuide.md`
  - `AuthCallbackErrorModel.md`
  - `StepAuthoringGuide.md`
- Updating DI auto‑registration rules
- Updating namespace conventions
- Updating product code to reference Frank instead of Application
- Updating tests to reference Frank engine types

---

# Acceptance Criteria

- [ ] All callback engine types live in Frank  
- [ ] Product contains **only** product‑specific steps  
- [ ] Engine documentation exists and is complete  
- [ ] DI auto‑registration works for steps  
- [ ] All invariants preserved  
- [ ] All diagnostics preserved  
- [ ] All existing callback tests pass  
- [ ] No product layer references Application.Authentication  
- [ ] No breaking changes to public API  
- [ ] Guardrail tests updated to enforce new boundaries  

---

# Out of Scope

- Session validation middleware (US‑111)  
- Identity mapping refactor  
- Multi‑tenant identity provider support  

---

# Dependencies

- US‑110 — Authentication: Owner Login  
- US‑111 — Authentication: Session Management  

---

# Notes

This is a **pure migration** — no behavior changes.  
Refactoring into a more formal engine architecture will occur in a later story.

