# Domain Conventions (CampFitFurDogs)

The Domain layer defines the business rules and invariants of the system.  
It must remain pure, deterministic, and free of infrastructure concerns.

---

## Aggregates

Aggregates must:

- enforce invariants  
- expose behavior, not state mutation  
- raise domain events when state changes  
- use value objects for complex fields  

Aggregates must not:

- expose setters  
- depend on EF Core  
- depend on Application or Infrastructure  
- contain DTOs  

---

## Value Objects

Value objects must:

- be immutable  
- implement equality by value  
- validate invariants in constructors  
- contain no behavior with side effects  

---

## Domain Events

Domain events must:

- represent meaningful business occurrences  
- be raised inside aggregates  
- be published by Application handlers via Unit of Work  

Domain events must not:

- contain infrastructure types  
- be published directly from Api  

---

## Prohibitions

Domain must not:

- perform I/O  
- depend on HttpContext  
- depend on hosting providers  
- depend on Frank abstractions  
