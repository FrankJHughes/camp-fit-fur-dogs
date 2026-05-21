# ADR-0034 — Email ValueObject Hardening

## Status  
Accepted

## Context  
The current `Email` ValueObject only enforces minimal invariants:

- Not empty  
- Contains `'@'`  
- Lowercased  

This is insufficient to protect the domain. Invalid but non-empty emails can pass through backend validation and be persisted. ADR‑0032 (CreateAccount Contract Spec) and ADR‑0033 (Domain Invariant Hardening Roadmap) both identify email validation as a high‑priority domain hardening target.

## Decision  
We will harden the `Email` ValueObject to enforce RFC‑lite validation and canonical normalization.

### New Domain Invariants  
- Must contain exactly one `'@'`  
- Local part must not contain consecutive dots  
- Domain must not contain consecutive dots  
- Domain must contain at least one dot  
- Domain TLD must be 2–63 characters  
- No whitespace allowed  
- No leading or trailing dots  
- No Unicode control characters  
- Entire email lowercased  
- Trimmed before validation  

### Normalization  
- Trim whitespace  
- Lowercase entire email  
- Normalize Unicode to NFC  

### Error Semantics  
Invalid emails throw `InvalidEmailException` with a descriptive message.

### Backend Alignment  
FluentValidation `.EmailAddress()` must match domain rules.  
Frontend must enforce the same rules.

## Consequences  
### Positive  
- Prevents invalid emails from entering the domain  
- Aligns frontend, backend, and domain  
- Supports future slices (email verification, login)  
- Reduces reliance on backend validation  

### Negative  
- Requires updating tests  
- May require migration if invalid emails already exist  

## Notes  
This ADR implements Phase 1 of ADR‑0033 for the Email VO.
