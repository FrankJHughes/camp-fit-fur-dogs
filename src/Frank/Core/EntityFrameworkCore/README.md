# EntityFrameworkCore

The **EntityFrameworkCore** folder contains infrastructure components that support
EF Core–based persistence within the Frank.Core architecture. This folder does
not contain DbContexts or concrete entity configurations; instead, it provides
shared building blocks used by vertical slices and infrastructure modules.

Currently, this folder contains a single marker type used to anchor assembly
scanning and registration.

---

## Components

### AssemblyMarker

`AssemblyMarker` is an empty, sealed class used as an anchor for reflection‑based
assembly discovery.

It enables:

- **Assembly identification**  
  Infrastructure modules can reference this type to reliably locate the
  `Frank.Core.EntityFrameworkCore` assembly.

- **Registration and scanning**  
  DI registration, configuration discovery, and EF Core extension methods often
  require a stable reference point for locating the correct assembly.

- **Separation of concerns**  
  Marker types avoid hard‑coding assembly names and keep scanning logic
  resilient to refactoring.

This pattern is used consistently across the Frank.Core architecture to ensure
that assembly scanning remains explicit, predictable, and slice‑friendly.

---

## Design Principles

- **Minimalism**  
  Marker types contain no logic — they exist solely to anchor reflection.

- **Explicitness**  
  Assembly scanning is always tied to a concrete type, never a string name.

- **Refactor‑safe**  
  Renaming or reorganizing folders does not break scanning logic.

- **Consistency**  
  The same pattern is used in the Application layer (`AssemblyMarker.cs`) and
  other infrastructure modules.

---

## How This Folder Fits Into the Architecture

This folder provides the foundational anchor for EF Core–related discovery
mechanisms. It is used by:

- DI registration extensions  
- EF Core configuration loaders  
- assembly scanning utilities  
- vertical slice persistence modules  

Any component that needs to locate the EF Core infrastructure assembly can do so
by referencing `AssemblyMarker`.

---

## Typical Usage

```csharp
var assembly = typeof(Frank.Core.EntityFrameworkCore.AssemblyMarker).Assembly;
```

This ensures scanning always targets the correct assembly.

---

## Notes

- This folder currently contains **only** the marker type.  
- Concrete EF Core configurations live in vertical slices, not here.  
- Additional EF Core infrastructure may be added in the future.

---
