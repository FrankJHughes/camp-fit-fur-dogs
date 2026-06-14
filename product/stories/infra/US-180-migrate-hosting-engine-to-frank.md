# US‑180 — Migrate Hosting Engine to Frank

## Story Grammar  
As an **architect**, I must **migrate the Hosting Engine into Frank** so that **hosting orchestration becomes a governed, reusable capability across products**.

## Intent  
The Hosting Engine is deterministic, cross‑cutting, and enforces invariants — it belongs in Frank.

## Acceptance Criteria  
- HostingEngine lives in Frank  
- Product only defines modules  
- DI auto‑registration updated  
- Guardrail tests updated  
- No product references to old hosting code  
