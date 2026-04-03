# Shared Kernel — DDD Building Blocks

## Intent
Provide compile-time-safe base types (Entity, AggregateRoot, ValueObject, DomainEvent) in a shared kernel so that all domain models inherit consistent identity, equality, and event semantics.

## Value
Eliminates boilerplate, enforces DDD patterns structurally, and makes domain code expressive and auditable.

## Acceptance Criteria
- [ ] SharedKernel project contains Entity<TId>, AggregateRoot<TId>, ValueObject, IDomainEvent
- [ ] Base types enforce equality and identity contracts via tests
- [ ] No reflection or magic — all behavior is explicit and compile-time verifiable
- [ ] NuGet-packageable structure (even if not published yet)

## Emotional Guarantees: N/A
