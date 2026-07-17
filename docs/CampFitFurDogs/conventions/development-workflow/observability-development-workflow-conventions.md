# CampFitFurDogs - Observability - Development Workflow Conventions  
*How observability must be incorporated into stories, tasks, PRs, CI, and releases.*

These conventions define how CampFitFurDogs developers must incorporate observability into their workflow using the Frank Observability platform.

They complement (but do not replace):

- Frank Observability Code Conventions  
- Architecture Governance  
- CI Governance  
- ADR‑0060 (Observability Architecture)  

These conventions ensure that observability is **designed**, **implemented**, **tested**, and **validated** consistently across the entire development lifecycle.

---

# 1. Story Requirements

Stories that involve observability must:

- reference Frank Observability conventions  
- specify *what* must be observable, not *how*  
- include acceptance criteria for:
  - event emission  
  - metric emission  
  - correlation propagation  

Stories must **not**:

- describe implementation details  
- specify vendor‑specific behavior  
- require ad‑hoc logging  

Stories define **intent**, not mechanics.

---

# 2. Task Requirements

Tasks must:

- identify which **events** and **metrics** are added or modified  
- include a test plan covering:
  - correlation propagation  
  - event emission correctness  
  - metric emission correctness  
- update documentation when new events or metrics are introduced  
  - event catalog  
  - metric catalog  
  - slice‑level observability docs (if applicable)

Tasks must **not**:

- introduce new observability abstractions  
- bypass Frank observability primitives  
- use Stopwatch or vendor APIs  

Tasks define **implementation**, not architecture.

---

# 3. PR Requirements

PRs must:

- include event/metric emission where appropriate  
- follow naming conventions (`slice.module.action`, `slice.module.metric_name`)  
- remove any ad‑hoc logging  
- include tests for:
  - correlation propagation  
  - event correctness  
  - metric correctness  

PRs must **not**:

- introduce manual correlation IDs  
- introduce Stopwatch timing  
- introduce vendor‑specific logging or metrics  
- mutate observability context  

PRs enforce **correctness**, not design.

---

# 4. CI Requirements

CI must validate:

- no forbidden logging APIs  
- no Stopwatch usage  
- all new events/metrics follow naming conventions  
- all observability tests pass deterministically  
- no mutation of observability context  
- no manual correlation ID creation  

CI enforces **governance**, not implementation.

---

# 5. Release Requirements

A feature may ship only when:

- all observability tests pass  
- event/metric catalog is updated  
- no forbidden patterns remain  
- correlation propagation is validated end‑to‑end  
- preview environment emits correct events/metrics  

Releases enforce **operational readiness**, not development correctness.

---

# Summary

CampFitFurDogs observability workflow conventions ensure that:

- observability is designed at the story level  
- implemented at the task level  
- validated at the PR level  
- enforced at the CI level  
- verified at release time  

These conventions guarantee:

- deterministic observability  
- consistent event and metric naming  
- safe correlation propagation  
- clean operational behavior  
- alignment with Frank’s observability platform  
