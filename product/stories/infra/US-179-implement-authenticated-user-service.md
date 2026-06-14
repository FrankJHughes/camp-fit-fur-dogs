# US‑179 — Implement Authenticated User Service

## Story Grammar  
As an **architect**, I must **implement a unified AuthenticatedUserService** so that **all layers share a consistent identity model**.

## Intent  
Identity is currently scattered across middleware, callbacks, and test fakes. This story centralizes it.

## Acceptance Criteria  
- Single source of truth for authenticated user  
- Works in API runtime  
- Works in test harness  
- Works with fake identity  
- Works with session model  
- No direct HttpContext access outside API layer  

