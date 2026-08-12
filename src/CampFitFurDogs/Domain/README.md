
# CampFitFurDogs.Domain

The `CampFitFurDogs.Domain` namespace contains the core business logic of the CampFitFurDogs system.  
It defines the **aggregates**, **value objects**, **domain exceptions**, and **strongly‑typed identifiers** that model the behavior, constraints, and invariants of the domain.

This layer is completely independent of the application, infrastructure, and API layers.  
It expresses **what the business is**, not how it is executed, persisted, or exposed.

---

## 🎯 Purpose of the Domain Layer

The domain layer exists to:

- Model business concepts with precision  
- Enforce invariants and rules that must always hold  
- Provide rich, intention‑revealing types  
- Protect the system from invalid or inconsistent states  
- Serve as the foundation for all workflows orchestrated by the application layer  

It is the heart of the system’s correctness.

---

## 📦 Key Sub‑Namespaces

### `CampFitFurDogs.Domain.Dogs`
Contains the full dog domain model:

- **[Dog](ca://s?q=Explain_the_Dog_aggregate)** — aggregate root  
- **[DogId](ca://s?q=Explain_DogId)** — strongly‑typed identifier  
- **[DogName](ca://s?q=Explain_DogName)** — value object  
- **[Breed](ca://s?q=Explain_Breed)** — value object  
- **[Sex](ca://s?q=Explain_Sex_enum)** — enum  

This namespace defines all invariants related to dog identity, ownership, and biological characteristics.

---

### `CampFitFurDogs.Domain.Exceptions`
Contains domain‑level exceptions, including:

- **[ExternalAuthProviderException](ca://s?q=Explain_ExternalAuthProviderException)**  
  Thrown when external identity/authentication systems violate domain assumptions.

Domain exceptions represent failures the domain cannot recover from.

---

### `CampFitFurDogs.Domain.AssemblyMarker`
A simple marker type used for assembly scanning:

- Enables reflection‑based discovery  
- Avoids string‑based assembly references  
- Used by application and infrastructure layers  

---

## 🧭 Domain Principles

The domain layer follows several core principles:

- **Ubiquitous Language**  
  Types and names reflect real business concepts.

- **Immutability**  
  Value objects are immutable and validated at creation.

- **Aggregate Integrity**  
  Aggregates enforce invariants and expose controlled mutation paths.

- **Strong Typing**  
  Identifiers (e.g., `DogId`) prevent accidental misuse of raw primitives.

- **Isolation**  
  The domain layer has no dependencies on application, infrastructure, or API concerns.

---

## 🔐 Domain Invariants

Examples of invariants enforced in this layer:

- A dog must always have a valid owner.  
- A dog’s name and breed must be non‑empty and normalized.  
- A `DogId` must never be empty.  
- Ownership cannot change after creation.  
- Value objects must always be in a valid state.  

These invariants ensure the domain remains correct regardless of how higher layers behave.

---

## 🚫 What Does *Not* Belong Here

The domain layer must **not** contain:

- Application logic (commands, queries, handlers)  
- Persistence logic (EF Core, repositories, DbContexts)  
- Infrastructure concerns (email, storage, external APIs)  
- HTTP or API DTOs  
- FluentValidation rules  
- Logging or telemetry  

Only **pure business logic** belongs here.

---

## 📚 Related Layers

- **[Application Layer](ca://s?q=Tell_me_more_about_the_application_layer)**  
  Orchestrates workflows using domain types.

- **Infrastructure Layer**  
  Implements persistence and external system integrations.

- **API Layer**  
  Exposes domain workflows via HTTP endpoints.

---

