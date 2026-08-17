# CampFitFurDogs Overview

CampFitFurDogs defines the product‑specific business domain, runtime surface, and vertical‑slice architecture for the dog‑management application. It builds on the Frank Core platform while providing all behavior, rules, and persistence required for the product.

This layer contains everything that makes CampFitFurDogs *a product*, not a platform.

---

## Product Responsibilities

CampFitFurDogs owns all business capabilities related to dog and owner management, including:

- **Dog profile management**  
  Creation, editing, listing, and retrieval of dog records.

- **Owner and user relationships**  
  Linking dogs to owners, enforcing ownership invariants, and integrating with Frank Identity.

- **API endpoints for business operations**  
  Vertical‑slice endpoints for dog registration, editing, listing, and retrieval.

- **Persistence and integration logic**  
  Mapping domain aggregates to database entities and implementing read/write abstractions.

The product layer focuses exclusively on business behavior while delegating cross‑cutting concerns to Frank Core.

---

## Source Alignment

CampFitFurDogs is organized into four primary subsystems:

- **API**  
  `src/CampFitFurDogs/Api`  
  Defines endpoints, request/response models, and routing.

- **Application**  
  `src/CampFitFurDogs/Application`  
  Contains command/query handlers, persistence abstractions, and orchestration logic.

- **Domain**  
  `src/CampFitFurDogs/Domain`  
  Defines aggregates, value objects, invariants, and domain rules.

- **Infrastructure**  
  `src/CampFitFurDogs/Infrastructure`  
  Implements persistence using EF Core, readers/writers, DbContext, and Unit of Work.

Each subsystem is documented in its corresponding folder under:

```
docs/01-campfitfurdogs/
```

---

## Design Intent

CampFitFurDogs is intentionally designed to:

- **Consume Frank platform abstractions**  
  Identity, routing, CQRS, Result<T>, EF Core conventions, and middleware.

- **Focus on business behavior**  
  All dog‑related rules, invariants, and workflows live in the product layer.

- **Maintain strict separation of concerns**  
  API → Application → Domain → Infrastructure, with clear boundaries.

- **Enable reuse and consistency**  
  By building on Frank Core, CampFitFurDogs inherits proven patterns and avoids re‑implementing foundational concerns.

The product layer remains small, focused, and maintainable — while the Frank platform provides the heavy lifting.

---

## Summary

CampFitFurDogs provides:

- a clean vertical‑slice architecture  
- domain‑driven modeling of dogs and owners  
- application‑layer orchestration using Frank’s CQRS abstractions  
- API endpoints built on Frank’s routing and middleware  
- infrastructure built on Frank’s EF Core foundation  

It is a product built on a platform — with clear boundaries, predictable behavior, and a consistent architectural style.

