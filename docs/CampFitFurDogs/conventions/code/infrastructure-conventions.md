# Infrastructure Conventions (CampFitFurDogs)

The Infrastructure slice provides persistence, external integrations, and
environment‑specific implementations. It must remain free of business logic and
must not leak EF Core types outside the slice.

---

## Responsibilities

Infrastructure owns:

- EF Core DbContext  
- entity configurations  
- repositories  
- Unit of Work  
- external service clients  
- environment‑specific implementations  

Infrastructure must not:

- contain business rules  
- depend on Api  
- return domain entities to Api  
- expose EF Core types to Application  

---

## Repositories

Repositories must:

- be auto‑registered via `[AutoRegister]`  
- return domain aggregates only to Application  
- never return EF Core entities  
- never expose IQueryable  
- never perform read‑model queries  

Repositories must not:

- contain business logic  
- bypass Unit of Work  
- use raw SQL except in rare, approved cases  

---

## Unit of Work

Unit of Work must:

- coordinate `SaveChangesAsync`  
- dispatch domain events  
- contain no business logic  
- be injected only into handlers  

---

## External Integrations

External integrations must:

- use named HttpClients  
- use Frank’s HttpClient seam  
- be fully testable  
- never perform direct network calls in tests  

---

## Prohibitions

Infrastructure must not:

- depend on Api  
- depend on HttpContext  
- depend on hosting providers directly  
- contain authentication logic  
