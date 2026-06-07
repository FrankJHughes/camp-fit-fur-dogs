# ADR‑0042 — Environment‑Aware Configuration Architecture

## Status  
Accepted

## Context  
The system runs across multiple environments:

- Local development  
- PR Preview  
- Production  

Early implementations relied on ad‑hoc environment variable access and conditional logic scattered across the codebase.  
This caused several issues:

- Inconsistent configuration behavior  
- Difficulty reproducing preview issues locally  
- Risk of leaking production configuration into previews  
- Lack of a unified source of truth for environment‑specific values  
- Complex logic in Program.cs and slices  

A centralized, deterministic configuration model was required.

## Decision  
The system now uses a **unified environment‑aware configuration architecture** with the following characteristics:

1. **Strongly‑typed configuration objects**  
   - `OidcOptions`  
   - `DatabaseOptions`  
   - `CorsOptions`  
   - `SessionOptions`  
   - `HostingOptions`  

2. **Configurator engine**  
   - Runs during startup  
   - Applies environment‑specific configuration  
   - Ensures all required keys are present  
   - Validates configuration consistency  

3. **Environment abstraction**  
   - Frank provides `IEnvironment`  
   - No direct access to `Environment.GetEnvironmentVariable`  
   - Prevents environment‑specific branching in product code  

4. **Preview‑safe behavior**  
   - PR Preview URLs are injected deterministically  
   - No production secrets are available in preview environments  

5. **Fail‑fast validation**  
   - Missing or invalid configuration causes startup failure  
   - Prevents silent misconfiguration  

## Consequences  

### Positive  
- Predictable configuration across all environments.  
- Eliminates environment‑specific branching in product code.  
- Strongly‑typed configuration reduces runtime errors.  
- Preview environments behave consistently and safely.  
- Easier debugging and local reproduction of preview issues.

### Negative  
- Requires maintenance of configuration objects as the system evolves.  
- Startup fails fast when configuration is incomplete.  
