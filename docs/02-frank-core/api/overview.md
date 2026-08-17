# Frank.Core.Api — Overview

The `Frank/Core` API subsystem provides the hosting, routing, middleware, and endpoint‑composition primitives used by all products built on the Frank platform. It defines how HTTP requests enter the system, how they are processed, and how API surfaces are assembled at runtime.

This folder documents the platform‑level API behaviors that CampFitFurDogs and other products consume.

---

## Purpose

The Frank.Core.Api subsystem exists to:

- define the runtime HTTP pipeline  
- provide cross‑cutting middleware (security, CORS, logging, exception handling)  
- discover and register endpoints automatically  
- unify hosting behavior across environments  
- ensure consistent request/response handling for all products  

It keeps the architecture readable, predictable, and maintainable for future contributors.

---

## Source Alignment

- **Primary implementation area:**  
  `src/Frank/Core`

- **Documentation folder:**  
  `docs/02-frank-core/api`

This documentation must remain aligned with the actual Frank.Core.Api implementation and updated as the platform evolves.

---

## What Belongs Here

This section should describe:

### [API Responsibilities](ca://s?q=Frank_Core_Api_Responsibilities)
- hosting and startup orchestration  
- middleware pipeline behavior  
- endpoint discovery and routing  
- configuration loading  
- environment‑specific hosting modules  
- health checks and graceful shutdown  

### [Platform Integration](ca://s?q=Frank_Core_Api_Platform_Integration)
How Frank.Core.Api connects to:

- Frank.Core.Application (dispatchers, pipeline behaviors)  
- Frank.Identity.Api (authentication, authorization)  
- Frank.Core.Infrastructure (logging, observation, configuration)  
- product‑level API surfaces (e.g., CampFitFurDogs.Api)  

### [Runtime Collaboration Points](ca://s?q=Frank_Core_Api_Runtime_Collaboration)
- middleware ordering  
- correlation and observation context  
- exception mapping  
- structured logging  
- routing groups and endpoint mapping  

### [Composition Flow](ca://s?q=Frank_Core_Api_Composition_Flow)
How the API layer composes with the rest of the platform:

```
Request → Middleware → Routing → Dispatch → Domain → Persistence → Response
```

This ensures consistent behavior across all products.

---

## Notes

Keep this document grounded in the actual Frank.Core.Api source code.  
Whenever hosting, routing, middleware, or endpoint discovery changes, update this section to reflect the current platform architecture.

