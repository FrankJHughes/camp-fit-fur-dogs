# architecture-boundaries.md

# Architecture Boundaries

CampFitFurDogs enforces strict architectural boundaries to preserve clarity,
testability, maintainability, and long‑term system integrity.  
Every layer, slice, and module has a defined responsibility — and an equally
important set of prohibitions.

Boundaries are not guidelines.  
They are **contracts**.

---

## Layer Boundaries

### Domain  
- Contains business rules, invariants, aggregates, value objects  
- Depends on **nothing**  
- Must not reference Application, Infrastructure, Api, or Frank  

### Application  
- Orchestrates use cases  
- Depends on Domain + Frank abstractions  
- Must not reference Infrastructure or Api  

### Infrastructure  
- Implements persistence and external integrations  
- Depends on Application, Domain, and Frank abstractions  
- Must not reference Api  

### Api  
- Exposes HTTP endpoints  
- Depends on Application, Domain, and Frank  
- Must not contain business logic  

### Frank  
- Provides cross‑cutting primitives  
- Must not depend on CampFitFurDogs  

---

## Slice Boundaries

Vertical slices must remain **self‑contained**.

A slice may include:

- Application handlers  
- Domain models  
- Infrastructure implementations  
- Api endpoints  

A slice must **not**:

- reference another slice’s Domain or Application types  
- orchestrate another slice’s workflows  
- share DTOs, entities, or handlers  

Cross‑slice communication occurs only through:

- Domain events  
- Frank abstractions  
- shared primitives  

Slices are the **unit of modularity**.

---

## Authentication Boundaries

Authentication is split into three layers:

1. **Frank protocol pipeline** — OIDC mechanics  
2. **Application authentication pipeline** — identity + session  
3. **Api endpoint** — delivery boundary  

Each layer has a single responsibility and must not leak concerns into the others.

---

## Security Boundaries

Security is enforced globally through:

- Frank security middleware  
- Api authorization policies  
- Application identity resolution  

Slices must not override or weaken security.

Security is a **system‑wide contract**, not a per‑feature decision.

---

## Infrastructure Boundaries

Infrastructure must:

- implement abstractions  
- remain replaceable  
- avoid leaking EF Core types  
- avoid cross‑slice dependencies  

Infrastructure is a **technical detail**, not a business layer.

---

## Enforcement

Boundary integrity is enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Boundaries exist to protect the system — and they must be followed.
