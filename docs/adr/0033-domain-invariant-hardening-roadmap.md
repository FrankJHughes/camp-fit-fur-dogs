# ADR-0033 — Domain Invariant Hardening Roadmap

## Status  
Accepted

## Context  
ADR‑0032 established the **CreateAccount Contract Specification**, defining a unified contract across frontend, backend, and domain layers. That ADR exposed a deeper architectural reality:

- Many ValueObjects (Email, PhoneNumber, Name, Password) are currently **skeletons**  
- Domain invariants are minimal and insufficient to protect the domain  
- The backend (FluentValidation) is stricter than the domain, which is backwards  
- The frontend and backend enforce rules the domain does not  
- Invalid but non-empty values can reach the database  
- Integration tests revealed mismatches between expected and actual domain behavior  

The domain must be the **final authority** on what constitutes valid business data.  
Right now, it is not.

This ADR defines the roadmap for **hardening domain invariants** so the domain becomes self‑protecting, consistent, and aligned with the canonical contract.

## Decision  
We will introduce a structured, incremental hardening of domain invariants across all customer‑related ValueObjects. This work will occur in phases to avoid destabilizing existing slices.

The roadmap is divided into four phases:

---

## Phase 1 — Foundational Invariants (High Priority)

### Email  
- Enforce RFC‑lite validation  
- Reject multiple '@'  
- Reject consecutive dots  
- Reject invalid domain segments  
- Normalize domain to lowercase  
- Trim whitespace  

### PhoneNumber  
- Enforce valid phone number format  
- Normalize to E.164 or US‑specific canonical form  
- Reject alphabetic characters  
- Reject invalid punctuation  

### Name (FirstName, LastName)  
- Reject digits  
- Reject symbols except hyphens and apostrophes  
- Enforce length constraints  
- Trim whitespace  

### PasswordHash  
- Ensure hash begins with `$2a$`, `$2b$`, or `$2y$`  
- Reject raw passwords accidentally passed to the VO  
- Reject unsupported hash formats  

### CustomerId  
- Validate Guid format  
- Reject empty Guid  

**Outcome:**  
The domain becomes capable of rejecting structurally invalid data even if the frontend or backend fails to validate it.

---

## Phase 2 — Semantic Invariants (Medium Priority)

### Email  
- Validate domain TLD length  
- Reject disposable email domains (optional)  
- Normalize Unicode characters  

### PhoneNumber  
- Validate region-specific numbering rules  
- Reject impossible numbers (e.g., too short, too long)  

### Name  
- Normalize Unicode  
- Enforce capitalization rules (optional)  

### Password  
- Optional: enforce minimum strength in domain  
- Optional: enforce entropy requirements  

**Outcome:**  
The domain enforces not just syntactic correctness but semantic correctness.

---

## Phase 3 — Cross‑Field Invariants (Future)

- Ensure email uniqueness at domain level (via domain service)  
- Ensure phone uniqueness (optional)  
- Ensure password is not equal to email or name  
- Ensure phone and email are not identical across multiple accounts  

**Outcome:**  
The domain enforces business‑level invariants that span multiple fields or aggregates.

---

## Phase 4 — Domain‑Driven Normalization (Future)

Introduce normalization rules that guarantee consistent storage:

- Email canonicalization  
- Phone canonicalization  
- Name canonicalization  
- PasswordHash canonicalization  

This ensures that two semantically identical inputs produce identical ValueObjects.

---

## Implementation Strategy

### 1. Incremental Refactoring  
Each ValueObject will be hardened in isolation to avoid breaking existing slices.

### 2. Contract Alignment  
All new invariants must align with ADR‑0032 (CreateAccount Contract Spec).

### 3. Test‑Driven Hardening  
For each invariant:
- Add failing domain tests  
- Add failing integration tests  
- Implement invariant  
- Update FluentValidation  
- Update frontend validation  

### 4. Backward Compatibility  
Where possible, new invariants should not break existing persisted data.  
If unavoidable, a migration plan will be created.

---

## Consequences  

### Positive  
- Domain becomes self‑protecting  
- Eliminates invalid data reaching the database  
- Aligns frontend, backend, and domain behavior  
- Reduces reliance on FluentValidation  
- Increases system correctness and predictability  
- Supports future slices (email verification, login, password reset)  

### Negative  
- Requires coordinated updates across frontend, backend, and domain  
- May require data migrations  
- Increases complexity of ValueObjects  
- Requires updates to existing tests  

---

## Notes  
This ADR defines the **roadmap**, not the implementation.  
Each ValueObject hardening step will be implemented in its own PR and validated through integration tests.

ADR‑0032 and ADR‑0033 together form the foundation for a stable, aligned, contract‑driven customer domain.

