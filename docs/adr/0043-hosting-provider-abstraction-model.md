# ADR‑0043 — Hosting Provider Abstraction Model

## Status  
Accepted

## Context  
The system must run consistently across multiple hosting providers:

- Render (API hosting)  
- Neon (database hosting)  
- Vercel (frontend hosting + PR Previews)  

Early implementations relied on provider‑specific logic scattered across the codebase.  
This created several problems:

- Difficult to test hosting behavior  
- Hard to support PR Preview environments  
- Provider‑specific branching leaked into slices  
- Inconsistent environment variable handling  
- Tight coupling to provider APIs  

A unified abstraction was needed to isolate hosting provider differences.

## Decision  
The system now uses a **Hosting Provider Abstraction Model** provided by Frank.

### Characteristics

1. **Provider‑agnostic interfaces**  
   - `IHostingEnvironment`  
   - `IArtifactClient`  
   - `IEnvironment`  
   - `IHostingMetadataProvider`  

2. **Provider‑specific implementations**  
   - Render implementation  
   - Neon implementation  
   - Vercel implementation  

3. **Test seams**  
   - Fake hosting provider for integration tests  
   - Fake environment for deterministic behavior  
   - Fake artifact client for PR Preview simulation  

4. **Preview‑aware behavior**  
   - PR Preview URLs are resolved through the hosting provider abstraction  
   - No provider‑specific logic in product code  

5. **Startup integration**  
   - Hosting provider is resolved once at startup  
   - All downstream services use the abstraction  

## Consequences  

### Positive  
- Product code is fully provider‑agnostic.  
- PR Preview behavior is deterministic and testable.  
- Hosting provider differences are isolated to Infrastructure.  
- Easier to add or replace hosting providers in the future.  
- Improved reliability of preview and production deployments.

### Negative  
- Requires maintenance of provider implementations.  
- Adds an abstraction layer that must remain consistent across providers.  
