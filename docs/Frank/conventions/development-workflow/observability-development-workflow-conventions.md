# Frank Observability Workflow Conventions

These conventions define how Frank contributors must develop, review, and validate observability‑related changes.

---

## 1. Story Requirements

Stories involving Frank Observability must:

- Reference ADR‑0060  
- Define affected engines (Hosting, Startup, Modules, Testing)  
- Include acceptance criteria for:
  - Correlation propagation  
  - Event emission  
  - Metric emission  
  - Deterministic behavior  

Stories must not define application‑specific events or metrics.

---

## 2. Task Requirements

Tasks must:

- Link to a Frank Observability story  
- Include test requirements  
- Include integration notes for affected engines  
- Include documentation updates  

Tasks must not introduce new observability abstractions.

---

## 3. PR Requirements

PRs must:

- Reference the story and ADR‑0060  
- Include:
  - Correlation tests  
  - Event tests  
  - Metric tests  
  - Determinism tests  
- Update Frank‑scoped documentation  

PRs must not:

- Introduce vendor‑specific exporters  
- Introduce ad‑hoc logging  
- Introduce Stopwatch timing  

---

## 4. CI/CD Requirements

CI must validate:

- Correlation propagation  
- Event schema stability  
- Metric naming conventions  
- Deterministic behavior  
- No forbidden patterns  

Preview environments must:

- Emit events and metrics through Frank abstractions  
- Propagate correlation IDs across all requests  

---

## 5. Release Requirements

A Frank Observability change may release only when:

- All tests pass  
- Documentation is updated  
- ADR‑0060 remains consistent  
- No forbidden patterns exist  
