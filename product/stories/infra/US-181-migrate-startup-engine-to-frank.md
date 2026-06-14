# US‑181 — Migrate Startup Engine to Frank

## Story Grammar  
As an **architect**, I must **migrate the Startup Engine into Frank** so that **startup orchestration is governed, reusable, and consistent across products**.

## Intent  
StartupEngine is a pure orchestration engine and should be centralized.

## Acceptance Criteria  
- StartupEngine lives in Frank  
- Product only defines startup modules  
- DI auto‑registration updated  
- Guardrail tests updated  
- No product references to old startup code  
