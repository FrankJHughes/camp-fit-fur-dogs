# US‑185 — Auto‑registration Opt‑out

## Story Grammar  
As an **architect**, I must be able to **opt out specific interfaces or classes from auto‑registration** so that **the DI container remains predictable and avoids accidental registrations**.

## Intent  
Auto‑registration is powerful but currently too coarse. This story introduces fine‑grained control.

## Acceptance Criteria  
- Attribute‑based opt‑out supported  
- Namespace‑based opt‑out supported  
- Interface‑based opt‑out supported  
- Works for both API and background services  
- Guardrail tests added  
- Documentation updated  
