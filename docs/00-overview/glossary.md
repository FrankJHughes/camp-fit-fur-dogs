# Glossary

## Aggregate
A domain object that enforces invariants and consistency for a business concept, such as a `Dog` entity.

## Application layer
The use-case orchestration layer where commands, queries, and transaction boundaries are coordinated.

## Domain layer
The core business model containing the rules and invariants that make the system meaningful.

## Infrastructure layer
The implementation details for persistence, I/O, EF Core, and adapters behind interfaces.

## API layer
The HTTP composition layer that wires middleware, endpoints, and runtime services together.

## Vertical slice
A feature-oriented grouping of API, application, domain, and infrastructure code that belongs to one capability.

## Unit of work
The transactional boundary used to commit related changes atomically.
