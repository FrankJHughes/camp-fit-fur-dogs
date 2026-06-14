# US‑190 — Production Deployment Verification Pipeline

## Story Grammar  
As an **admin**, I must be able to **verify production deployments automatically** so that **the system is always in a showcase‑ready state**.

## Intent  
Preview has strong verification. Production does not. This story adds a minimal but essential verification pipeline.

## Acceptance Criteria  
- Post‑deploy smoke tests run against production  
- Health check verification  
- Auth callback verification  
- Frontend route verification  
- Logging verification  
- Deployment fails if verification fails  
