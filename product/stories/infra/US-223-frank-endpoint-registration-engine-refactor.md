# US‑223 — Frank Endpoint Registration Engine Refactor

## Intent  
As an **admin**, I must ensure that the API endpoint subsystem follows Frank’s established engine‑based naming conventions so that the architecture remains consistent, discoverable, and aligned with the platform’s conceptual model.

## Value  
The current subsystem uses names such as `EndpointDiscovery` and `MapEndpoints`, which do not match the naming conventions used across Frank (e.g., Startup Engine, Registration Engine, Domain Event Dispatcher).  
Refactoring this subsystem to **Frank Endpoint Registration Engine**:

- Clarifies its role as a first‑class engine  
- Improves architectural consistency  
- Makes documentation and onboarding clearer  
- Ensures future enhancements (DI activation, grouping, versioning, metadata) fit naturally into the engine model  
- Reduces conceptual friction for developers working across multiple engines  

This story aligns naming, documentation, and conceptual framing without altering runtime behavior.

## Acceptance Criteria  
- [ ] AC‑1: The subsystem is renamed to **Endpoint Registration Engine** across code, docs, and guides  
- [ ] AC‑2: `EndpointDiscovery` is renamed to `EndpointRegistrationEngine` (or equivalent final naming)  
- [ ] AC‑3: Extension method `MapEndpoints` continues to function but delegates to the renamed engine  
- [ ] AC‑4: All developer, tester, and user guides reflect the new naming  
- [ ] AC‑5: No functional behavior changes — only naming and documentation alignment  
- [ ] AC‑6: All references in Startup code, samples, and templates are updated  
- [ ] AC‑7: Old names are removed or deprecated cleanly (no breaking changes without migration notes)  

## Emotional Guarantees  
- **EG‑01 — No Surprises:** The refactor introduces no behavioral changes; only clarity improves.  
- **EG‑05 — Clear Boundaries:** Developers immediately understand the subsystem’s purpose and how it fits into the engine architecture.

## Notes  
- This story does **not** introduce DI activation, grouping, versioning, or metadata conventions — those will be separate stories.  
- This story aligns naming only; implementation changes must be minimal and safe.  
- Ensure the rename is reflected in:  
  - Code  
  - Documentation  
  - Developer/Test/User guides  
  - ADRs  
  - Samples and templates  
- This story unblocks future enhancements to the Endpoint Registration Engine.
