# infrastructure-architecture.md

# Infrastructure Architecture

The Infrastructure layer provides all technical capabilities required to support
the Application and Domain layers. It implements persistence, external
integrations, and environment‑specific concerns — but **never business logic**.

Infrastructure is replaceable, testable, and strictly separated from the Domain.

## Responsibilities

The Infrastructure layer provides:

- EF Core persistence  
- Repository implementations  
- Query/read models  
- Unit of Work  
- External service integrations  
- Hosting provider implementations  
- Configuration binding and environment access (via Frank abstractions)  

Infrastructure **implements** abstractions defined in Application or Frank.

## Prohibitions

Infrastructure must not:

- contain business rules  
- expose EF Core types to Application  
- depend on Api  
- depend on other slices’ Application or Domain types  
- read environment variables directly (must use Frank abstractions)  
- perform cross‑slice orchestration  

Infrastructure is a **technical detail**, not a business layer.

## Repository Rules

Repositories must:

- return Domain entities or read models  
- never expose EF Core entities  
- never leak IQueryable  
- encapsulate all persistence concerns  
- be deterministic and testable  

## External Integrations

External integrations must:

- use Frank’s HTTP and environment abstractions  
- be isolated behind interfaces  
- avoid leaking external DTOs into Application or Domain  
- be fully mockable  

## Hosting Providers

Hosting providers:

- live in Infrastructure  
- implement Frank’s hosting abstractions  
- must not contain business logic  
- must not perform HTTP calls  
- must not parse JSON/ZIP  
- must not write configuration  

## Enforcement

Infrastructure boundaries are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Infrastructure is the **technical adapter layer** — nothing more.
