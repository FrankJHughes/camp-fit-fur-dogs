# CampFitFurDogs.Host — Hosting Modules

The **HostingModules** in the CampFitFurDogs.Api assembly provide the environment‑specific
configuration logic used by the Camp Fit Fur Dogs platform.  
The **Host** project is responsible for *orchestrating* these modules during startup.

This folder does **not** live inside the Host project, but the Host project is the
layer that *executes* the hosting pipeline.

---

## Purpose

The Host project uses hosting modules to adapt the application to its runtime
environment:

- Local development  
- Render PR Preview deployments  
- Production hosting  
- CI/CD environments  

The Host project composes these modules and applies them through the hosting engine
before service registration and endpoint mapping.

---

## How Hosting Modules Are Used

The Host project calls:

```csharp
await Hosting.AdaptToHostingEnvironment(builder);
```

This:

1. Constructs all hosting modules from the Api assembly  
2. Initializes a `HostingEngine`  
3. Applies environment‑specific configuration overrides  
4. Mutates the `WebApplicationBuilder` before DI registration  

This keeps startup clean and ensures hosting behavior is centralized in one place.

---

## Design Principles

- **Separation of concerns** — hosting orchestration lives in the Host project  
- **Environment isolation** — hosting modules encapsulate environment‑specific logic  
- **Composability** — multiple modules can be combined  
- **Predictability** — each module declares activation rules and override behavior  

---

## Summary

The Host project is responsible for *executing* hosting modules, not defining them.
It orchestrates the hosting pipeline, applies environment‑specific configuration,
and ensures the application starts with the correct settings for its environment.
