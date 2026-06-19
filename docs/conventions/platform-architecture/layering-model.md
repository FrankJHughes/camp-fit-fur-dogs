# layering-model.md

# Layering Model

CampFitFurDogs uses a strict layered architecture to ensure clarity, testability,
and separation of concerns. Each layer has a well‑defined purpose and a narrow,
enforced dependency surface.

## Layers

- **Domain** — business rules, aggregates, value objects, domain events  
- **Application** — use‑case orchestration, CQRS handlers, validation  
- **Infrastructure** — persistence, external integrations  
- **Api** — endpoints, DTO binding, authorization  
- **Frank** — cross‑cutting primitives and platform services  

## Layering Rules

- Domain depends on **nothing**  
- Application depends on **Domain** and **Frank abstractions**  
- Infrastructure depends on **Application**, **Domain**, and **Frank abstractions**  
- Api depends on **Application**, **Domain**, and **Frank**  

Frank is the **only allowed cross‑layer dependency**.

These rules are enforced by guardrail tests and must not be violated.
