# domain-architecture.md

# Domain Architecture

The Domain layer contains the core business rules of CampFitFurDogs.  
It is the **center of the system**, and all other layers exist to support it.

The Domain must remain **pure**, **isolated**, and **independent**.

## Responsibilities

The Domain layer defines and enforces:

- Aggregates  
- Entities  
- Value objects  
- Domain events  
- Invariants  
- Business rules  
- Consistency boundaries  

The Domain expresses **what the business does**, not how the system works.

## Prohibitions

The Domain layer must not depend on:

- Application  
- Infrastructure  
- Api  
- Frank  
- External libraries (beyond basic .NET primitives)  

The Domain must not contain:

- Persistence logic  
- HTTP logic  
- Serialization logic  
- Infrastructure concerns  
- Framework‑specific types  
- Cross‑slice references  

## Domain Events

Domain events:

- represent meaningful business occurrences  
- are raised by aggregates  
- are published by the Application layer  
- must be immutable  

## Aggregate Rules

Aggregates must:

- enforce invariants  
- protect internal state  
- expose behavior, not data  
- validate all state transitions  

Aggregates must not:

- expose mutable collections  
- allow external mutation  
- depend on other aggregates  

## Enforcement

Domain purity is enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

The Domain is the **source of truth** for all business behavior.
