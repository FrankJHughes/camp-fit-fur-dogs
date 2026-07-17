# application-architecture.md

# Application Architecture

The Application layer orchestrates business use cases. It coordinates Domain
behavior, enforces validation, and defines transactional boundaries. It contains
no business rules of its own — those belong exclusively to the Domain.

## Responsibilities

The Application layer is responsible for:

- orchestrating use cases  
- executing CQRS handlers  
- validating incoming requests  
- interacting with Domain aggregates  
- managing transaction boundaries  
- publishing Domain events  

Application logic is **procedural orchestration**, not business logic.

## Prohibitions

The Application layer must not:

- access `HttpContext`  
- access Infrastructure directly  
- perform persistence logic  
- contain business rules  
- expose EF Core types  
- depend on other slices’ Application or Domain types  

## Handler Structure

Handlers must:

- be deterministic  
- be side‑effect‑controlled  
- delegate all business rules to the Domain  
- return DTOs or result objects, never Domain entities  

## Enforcement

These rules are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

The Application layer is the system’s orchestration boundary — nothing more.
