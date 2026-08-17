# Frank Core Overview

Frank Core is the reusable platform foundation shared across all products in the ecosystem. It provides the architectural primitives, runtime behaviors, and cross‑cutting abstractions that CampFitFurDogs and other applications depend on. Frank defines the *how* of the system so product code can focus exclusively on the *what*.

Frank Core is intentionally generic, stable, and domain‑agnostic. It contains no product‑specific business rules.

---

## Key Modules

Frank Core is organized into several platform modules, each responsible for a distinct architectural concern.

### [Api](ca://s?q=Frank_Core_Api_Overview)
Provides runtime hosting and request handling:

- middleware pipeline (CORS, security headers, exception handling)
- endpoint discovery via `IEndpoint`
- routing conventions and grouping
- OpenAPI integration
- platform‑level observability

### [Application](ca://s?q=Frank_Core_Application_Overview)
Defines orchestration primitives:

- `ICommandDispatcher` and `IQueryDispatcher`
- pipeline behaviors (validation, logging, authorization)
- `Result<T>` pattern for error handling
- application‑level contracts for handlers

### [Domain](ca://s?q=Frank_Core_Domain_Overview)
Provides foundational domain abstractions:

- `AggregateRoot<TId>`
- `ValueObject`
- `AggregateId`
- domain exception base types
- domain event primitives

### [EntityFrameworkCore](ca://s?q=Frank_Core_EntityFrameworkCore_Overview)
Defines persistence integration patterns:

- base `DbContext` conventions
- strongly typed ID conversions
- value object conversions
- unit of work base class
- migration scaffolding patterns

### [Infrastructure](ca://s?q=Frank_Core_Infrastructure_Overview)
Provides runtime adapters and utilities:

- clock/time abstractions
- validation helpers
- environment configuration
- outbox/event dispatching primitives
- cross‑cutting utilities shared across products

---

## Design Philosophy

Frank Core is designed around several principles:

### 1. **Platform First**
Frank defines the architectural rules and primitives.  
Products consume them — they do not redefine them.

### 2. **Domain‑Agnostic**
Frank contains *no business logic*.  
It provides the scaffolding for domain logic to run consistently.

### 3. **Vertical‑Slice Friendly**
Frank supports vertical slices by:

- keeping API routing modular  
- keeping application orchestration generic  
- keeping domain primitives reusable  
- keeping persistence conventions consistent  

### 4. **Extensible Without Forking**
Products extend Frank through:

- hosting modules  
- custom middleware  
- additional pipeline behaviors  
- domain events  
- product‑specific readers/writers  

Frank stays stable while products evolve.

### 5. **Consistency Across Products**
Every product built on Frank:

- handles commands and queries the same way  
- uses the same routing model  
- uses the same persistence conventions  
- uses the same identity primitives  
- uses the same error handling and result patterns  

This dramatically reduces cognitive load for contributors.

---

## How CampFitFurDogs Uses Frank

CampFitFurDogs consumes Frank Core at every layer:

- **API** — endpoint discovery, routing, middleware  
- **Application** — dispatchers, pipeline behaviors, `Result<T>`  
- **Domain** — aggregate base classes, value objects, identity primitives  
- **Infrastructure** — DbContext base, unit of work, EF Core conventions  

Frank provides the platform; CampFitFurDogs provides the product behavior.

---

## Summary

Frank Core is the architectural backbone of the system:

- reusable  
- stable  
- domain‑agnostic  
- extensible  
- consistent across products  

It defines the patterns that product code depends on, enabling CampFitFurDogs to focus entirely on business logic while inheriting a robust, production‑ready foundation.

