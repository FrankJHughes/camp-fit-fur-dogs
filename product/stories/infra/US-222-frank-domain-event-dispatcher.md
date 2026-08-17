# US‑222 — Domain Event Dispatcher Implementation

## Intent

As an **admin**, I must be able to rely on a **first‑class Domain Event Dispatcher** in Frank so that domain events are delivered consistently, deterministically, and without each module reinventing its own dispatching logic.

## Value

Domain events communicate that “something happened” inside the domain.  
Frank currently provides only **contracts** (`IDomainEvent`, `IDomainEventHandler<T>`, `IDomainEventDispatcher`) but **no implementation**, forcing each module to create its own dispatcher.

This results in:

- inconsistent event delivery  
- duplicated infrastructure  
- missing observability  
- unpredictable handler ordering  
- architectural drift  

A built‑in dispatcher restores symmetry with CommandDispatcher and QueryDispatcher, ensures consistent behavior across modules, and provides a stable foundation for future event‑driven features (outbox, sagas, projections, notifications).

## Acceptance Criteria

- [ ] A concrete `DomainEventDispatcher` implementation exists in Frank  
- [ ] Dispatcher resolves **all** `IDomainEventHandler<TEvent>` instances from DI  
- [ ] Dispatcher invokes handlers **sequentially**, in DI‑provided order  
- [ ] Dispatcher supports multiple handlers per event  
- [ ] Dispatcher gracefully handles the case where no handlers exist  
- [ ] Dispatcher propagates exceptions thrown by handlers (no swallowing)  
- [ ] Dispatcher does **not** perform validation (domain events are facts, not requests)  
- [ ] Dispatcher is registered via AutoRegistration with scoped lifetime  
- [ ] Unit tests verify:
  - dispatching with 0 handlers  
  - dispatching with 1 handler  
  - dispatching with multiple handlers  
  - handler exceptions propagate  
  - cancellation token is honored  
- [ ] Documentation updated in the Frank Guides (Developer, Tester, User)

## Emotional Guarantees

- **EG‑01 No Surprises** — Domain events behave the same everywhere in the system; no module invents its own dispatching rules.  
- **EG‑03 Calm Protection** — Event delivery is predictable and safe; no silent failures or hidden behavior.  
- **EG‑05 Clear Boundaries** — Domain events remain pure notifications; no accidental mixing with commands, queries, or side‑effects outside handlers.

## Notes

- Domain events are **facts**, not requests — therefore **no validation** and **no return values**.  
- Dispatcher must remain **simple, synchronous, deterministic** — no parallelism, no ordering guarantees beyond DI order.  
- This story completes the triad:
  - CommandDispatcher  
  - QueryDispatcher  
  - **DomainEventDispatcher**  
- Enables future stories:
  - Outbox integration  
  - Event replay  
  - Projections  
  - Notifications  
- **Demo:** Trigger a domain event in a test module, observe all handlers firing in order, with clear logs and no surprises.
