# Identity Application Layer

The **Application** folder contains the orchestration layer for the Identity subsystem.  
It wires together all vertical slices—**Sessions**, **Users**, and **Callback**—into a cohesive module that can be registered into any hosting environment.

This layer is responsible for:

- Composing all Identity subsystems  
- Registering CQRS command and query handlers  
- Registering FluentValidation validators  
- Providing assembly markers for discovery  
- Exposing a single DI entry point for the entire Identity application module  

The Application layer does **not** contain business logic itself; instead, it aggregates and registers the logic defined in its subfolders.

---

## Folder Structure

```
Application/
├── AssemblyMarker.cs
└── ServiceCollectionExtensions.cs
```

Submodules included via DI:

```
Sessions/
Users/
Callback/
```

Each of these submodules contains its own handlers, validators, and abstractions.

---

# AssemblyMarker

A simple type used for assembly scanning.

### Responsibilities

- Acts as a stable anchor for:
  - CQRS handler discovery  
  - Validator discovery  
  - Reflection‑based registration  
- Ensures the correct assembly is referenced without relying on string‑based namespace matching

### Notes

- Contains no behavior  
- Never instantiated  
- Used exclusively for DI registration and scanning  

---

# ServiceCollectionExtensions

The root DI registration entry point for the Identity application layer.

### Responsibilities

- Register the **Sessions** subsystem  
- Register the **Users** subsystem  
- Register the **Callback** subsystem  
- Register all FluentValidation validators in the assembly  
- Provide a single, predictable method for wiring Identity into an API or worker service

### Registration Flow

```
AddFrankIdentityApplication()
        ↓
AddFrankIdentityApplicationSessions()
        ↓
AddFrankIdentityApplicationUsers()
        ↓
AddFrankIdentityApplicationCallback()
        ↓
AddValidatorsFromAssembly(AssemblyMarker.Assembly)
```

### Notes

- Uses assembly scanning with `DiscoveryOptions`  
- Ensures only Identity‑related CQRS handlers are registered  
- Keeps the hosting layer clean and minimal  

---

# Identity Application Composition

The Application layer composes three major subsystems:

### **Sessions**
- Token generation  
- Session creation  
- Session retrieval  
- Session revocation  

### **Users**
- User resolution (OIDC → internal user)  
- User creation  
- User queries  

### **Callback**
- OIDC callback pipeline  
- External identity acquisition  
- Save pipeline (user + session creation)  

Together, these subsystems form the complete authentication and identity‑management workflow.

---

# Summary

The Application folder provides the orchestration and registration layer for the Identity subsystem:

### Core Components
- `AssemblyMarker`
- `ServiceCollectionExtensions`

### Responsibilities
- Compose all Identity vertical slices  
- Register CQRS handlers  
- Register validators  
- Provide a single DI entry point  

This folder ensures that the Identity application layer is cleanly structured, discoverable, and easy to integrate into any host.

---
