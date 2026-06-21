# api-architecture.md

# Api Architecture

The Api layer is the system’s **delivery boundary**.  
It exposes HTTP endpoints, binds request/response DTOs, and delegates all
business behavior to the Application layer.

The Api layer must remain **thin**, **stateless**, and **free of business logic**.

## Responsibilities

The Api layer is responsible for:

- defining HTTP endpoints  
- binding and validating request DTOs  
- applying authorization policies  
- dispatching commands and queries to the Application layer  
- applying security headers  
- shaping response DTOs  

The Api layer is an **adapter**, not a business layer.

## Prohibitions

The Api layer must not:

- contain business logic  
- access Infrastructure directly  
- return Domain entities  
- read identity from request bodies  
- perform persistence operations  
- orchestrate multi‑step workflows  
- depend on other slices’ Application or Domain types  

All business behavior must be delegated to the Application layer.

## Endpoint Rules

Endpoints must:

- be deterministic  
- be idempotent where appropriate  
- validate incoming DTOs  
- never expose internal exceptions  
- return typed results (never raw objects)  
- use Frank’s endpoint discovery conventions  

Endpoints must not:

- contain conditional business logic  
- mutate Domain entities directly  
- perform cross‑slice coordination  

## DTO Rules

DTOs must:

- be immutable  
- contain only transport‑safe data  
- never expose Domain types  
- never include behavior  

DTOs represent **API contracts**, not business models.

## Authorization

Authorization must:

- be declarative  
- use policies, not inline logic  
- never embed business rules  
- rely on identity resolved by the Application authentication pipeline  

## Enforcement

Api boundaries are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

The Api layer is the **entry point**, not the decision‑maker.
