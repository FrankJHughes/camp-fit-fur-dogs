# Frank — Conventions — Development Workflow — Observability  
*How Frank contributors must develop, review, and validate observability‑related changes.*

These conventions define the **workflow expectations** for contributors modifying or extending Frank’s observability platform.  
They ensure that all changes remain:

- deterministic  
- correlation‑safe  
- structured  
- vendor‑agnostic  
- aligned with ADR‑0060  
- consistent across HostingEngine, StartupEngine, Modules, and Testing  

These conventions apply to **Frank contributors**, not CampFitFurDogs.

---

# 1. Story Requirements

Stories involving Frank Observability must:

- reference **ADR‑0060**  
- define which engines or subsystems are affected:
  - HostingEngine  
  - StartupEngine  
  - Module system  
  - Test harness  
- include acceptance criteria for:
  - correlation propagation  
  - event emission  
  - metric emission  
  - deterministic behavior  

Stories must **not**:

- define application‑specific events  
- define application‑specific metrics  
- introduce product‑specific observability rules  

Stories define **intent**, not implementation.

---

# 2. Task Requirements

Tasks must:

- link to a Frank Observability story  
- include explicit test requirements:
  - correlation tests  
  - event tests  
  - metric tests  
  - determinism tests  
- include integration notes for affected engines  
- include documentation updates (Frank‑scoped)  

Tasks must **not**:

- introduce new observability abstractions  
- bypass existing observability primitives  
- introduce vendor‑specific behavior  

Tasks define **implementation**, not architecture.

---

# 3. PR Requirements

PRs must:

- reference the story and **ADR‑0060**  
- include:
  - correlation propagation tests  
  - event emission tests  
  - metric emission tests  
  - determinism tests  
- update Frank‑scoped documentation:
  - Observability architecture  
  - Engine integration notes  
  - Test harness documentation  

PRs must **not**:

- introduce vendor‑specific exporters  
- introduce ad‑hoc logging  
- introduce Stopwatch timing  
- mutate observability context  
- generate correlation IDs manually  

PRs enforce **correctness**, not design.

---

# 4. CI/CD Requirements

CI must validate:

- correlation propagation  
- event schema stability  
- metric naming conventions  
- deterministic behavior  
- absence of forbidden patterns:
  - Stopwatch  
  - vendor APIs  
  - manual correlation IDs  
  - ad‑hoc logging  

Preview environments must:

- emit events and metrics through Frank abstractions  
- propagate correlation IDs across all requests  
- validate end‑to‑end observability behavior  

CI/CD enforces **governance**, not implementation.

---

# 5. Release Requirements

A Frank Observability change may release only when:

- all observability tests pass  
- documentation is updated  
- ADR‑0060 remains consistent  
- no forbidden patterns exist  
- preview environment validation is complete  

Releases enforce **operational readiness**, not development correctness.

---

# Summary

Frank Observability workflow conventions ensure that:

- stories define intent  
- tasks define implementation  
- PRs enforce correctness  
- CI/CD enforces governance  
- releases enforce operational safety  

These conventions guarantee that Frank’s observability platform remains:

- deterministic  
- correlation‑safe  
- structured  
- vendor‑agnostic  
- aligned with ADR‑0060  
- consistent across all engines and modules  
