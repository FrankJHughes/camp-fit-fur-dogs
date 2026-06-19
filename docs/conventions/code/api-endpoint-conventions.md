# API Endpoint Conventions (CampFitFurDogs)

The Api slice defines HTTP endpoints, DTO binding, and authorization.  
Endpoints must remain thin and must not contain business logic.

---

## Endpoint Structure

Endpoints must:

- implement `IEndpoint`  
- define routes explicitly  
- bind DTOs  
- authorize requests  
- dispatch commands/queries via Frank’s dispatcher  

Endpoints must not:

- contain business logic  
- perform persistence  
- construct aggregates  
- bypass the dispatcher  
- compute redirect URLs (callback endpoints use builders)  

---

## Routing Rules

- One endpoint per command/query  
- No attribute routing  
- No monolithic endpoint files  
- Routes must be stable and predictable  

---

## DTO Conventions

DTOs must:

- be immutable  
- contain no behavior  
- contain no domain types  
- contain no EF Core types  

---

## Authorization

Authorization must:

- use policies defined in Application  
- never be hard‑coded in endpoints  
- never depend on HttpContext directly (use abstractions)  

---

## Error Behavior

Endpoints must not:

- catch exceptions from handlers  
- catch exceptions from OIDC protocol pipeline  
- catch exceptions from Application callback pipeline  

Errors are handled by product‑level global exception middleware.
