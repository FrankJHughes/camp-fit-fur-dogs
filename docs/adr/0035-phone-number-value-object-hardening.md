# ADR-0035 — PhoneNumber ValueObject Hardening

## Status  
Accepted

## Context  
The current `PhoneNumber` ValueObject only enforces:

- Not empty  
- Trimmed  

This allows invalid phone numbers (alphabetic, malformed, too short, too long) to enter the domain and database. ADR‑0032 and ADR‑0033 identify phone normalization as a required step for contract alignment.

## Decision  
We will harden the `PhoneNumber` ValueObject with structural validation and canonical normalization.

### New Domain Invariants  
- Must contain only digits and allowed punctuation (`+`, `-`, `(`, `)`, spaces)  
- Must contain at least 10 digits (US standard)  
- Must not exceed 15 digits (E.164 max)  
- Must not contain alphabetic characters  
- Must not contain emoji or symbols  
- Must not contain repeated punctuation sequences  

### Normalization  
Two supported canonical formats:

#### Option A — E.164 (recommended)  
`+1XXXXXXXXXX`

#### Option B — US National Format  
`(XXX) XXX-XXXX`

The domain will normalize to **E.164**.

### Error Semantics  
Invalid phone numbers throw `InvalidPhoneNumberException`.

### Backend Alignment  
FluentValidation must enforce the same rules.  
Frontend must enforce the same rules.

## Consequences  
### Positive  
- Prevents invalid phone numbers from entering the domain  
- Ensures consistent storage format  
- Supports SMS‑based features in the future  
- Aligns frontend, backend, and domain  

### Negative  
- Requires updating tests  
- Requires frontend and backend updates  
- May require data migration  

## Notes  
This ADR implements Phase 1 of ADR‑0033 for the PhoneNumber VO.
