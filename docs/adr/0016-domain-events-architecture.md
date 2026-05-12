# ADR 0016 — Domain Events Architecture

## Status
Accepted

## Context
Domain events are used to model significant state changes within the domain and to decouple producers from consumers.

## Decision
Adopt a lightweight domain event model where events are simple C# records published through an in‑process dispatcher.

## Consequences
- Domain logic remains decoupled.
- Testing is simplified.
- No external event bus is required at this stage.