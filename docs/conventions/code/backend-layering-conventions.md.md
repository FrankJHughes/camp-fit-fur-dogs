# Backend Layering Conventions (CampFitFurDogs)

CampFitFurDogs follows a strict layering model enforced by guardrails.  
Each slice has clear responsibilities and prohibitions.

Layers:

- Domain  
- Application  
- Infrastructure  
- Api  

Frank provides cross‑cutting primitives but does not replace these layers.

---

## Domain Layer

The Domain layer contains:

- aggregates  
- entities  
- value objects  
- domain events  
- invariants  
- business rules  

Domain must not depend on:

- Application  
- Infrastructure  
- Api  
- Frank  

Domain must not:

- perform I/O  
- depend on EF Core  
- depend on HttpContext  
- depend on hosting providers  

---

## Application Layer

The Application layer contains:

- commands  
- queries  
- handlers  
- validators  
- domain interaction  
- transaction boundaries  
- domain event publishing  

Application must not depend on:

- Infrastructure  
- Api  

Application must not:

- access EF Core directly  
- access HttpContext  
- perform business logic (belongs in Domain)  
- return domain entities to Api  

---

## Infrastructure Layer

Infrastructure contains:

- EF Core persistence  
- repositories  
- Unit of Work  
- external integrations  
- hosting provider implementations (product‑specific)  

Infrastructure must not depend on:

- Api  

Infrastructure must not:

- contain business logic  
- expose EF Core types to Application  
- return domain entities to Api  

---

## Api Layer

Api contains:

- endpoints  
- DTO binding  
- authorization  
- command/query dispatch  
- security headers (via Frank middleware)  
- startup modules  

Api must not:

- contain business logic  
- perform persistence  
- construct aggregates  
- bypass the dispatcher  
