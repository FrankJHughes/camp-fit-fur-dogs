# CQRS Conventions (Frank)

Frank defines the CQRS dispatcher, handler pipeline, and auto‑registration rules.
Products implement commands, queries, and handlers, but Frank governs the rules.

---

## Commands and Queries

Commands and queries must:

- be immutable  
- contain no behavior  
- contain no Infrastructure or Api dependencies  
- represent pure intent  

Frank enforces immutability and validation.

---

## Handlers

Handlers must:

- validate inputs  
- load aggregates or required state  
- invoke domain behavior  
- persist changes via Unit of Work  
- publish domain events  

Handlers must not:

- contain business rules  
- access EF Core directly  
- access HttpContext  
- call other handlers  
- return domain entities to Api  

Handlers return DTOs or primitives.

---

## Pipeline Enforcement

Frank ensures:

- validation runs before handler execution  
- domain events are published after persistence  
- handlers cannot bypass the pipeline  
