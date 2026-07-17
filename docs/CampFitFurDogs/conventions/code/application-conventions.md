# Application Conventions (CampFitFurDogs)

The Application layer orchestrates use cases by coordinating domain behavior,
validation, and persistence. It must remain thin and free of business rules.

---

## Commands and Queries

Commands and queries must:

- be immutable  
- represent intent  
- contain no behavior  
- contain no Infrastructure or Api dependencies  

Validators must exist for all commands/queries with input.

---

## Handlers

Handlers must:

- validate inputs  
- load aggregates or required state  
- invoke domain behavior  
- persist via Unit of Work  
- publish domain events  

Handlers must not:

- contain business rules  
- access EF Core directly  
- access HttpContext  
- call other handlers  
- return domain entities to Api  

Handlers return DTOs or primitives.

---

## Transaction Boundaries

Handlers define transaction boundaries:

- all domain changes occur inside a single Unit of Work  
- domain events are published after persistence  

---

## Prohibitions

Application must not:

- depend on Infrastructure  
- depend on Api  
- perform protocol logic (OIDC)  
- compute redirect URLs (belongs in callback builder)  
