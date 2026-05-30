# Domain Events Architecture

This guide describes how domain events are modeled, raised, collected, and dispatched in Camp Fit Fur Dogs.  
It belongs to the **architecture** category because it governs cross‑cutting behavior across all vertical slices.

Domain events ensure that **business rules remain in the Domain**, while **side effects remain in Application and Infrastructure**.

---

# 1. Goals

- **Decouple** domain logic from side effects  
- **Centralize** event dispatching in the Application layer  
- **Keep** the Domain layer pure and free of infrastructure concerns  
- **Make** domain events testable, observable, and deterministic  
- **Ensure** all event dispatching flows through the Unit of Work  

---

# 2. Domain Responsibilities

## 2.1 Raising Events

Domain entities and aggregates raise events to describe something that *has happened*.

- Domain events are simple immutable classes (e.g., `DogRegisteredDomainEvent`)  
- Events are raised inside aggregate methods (e.g., `RegisterDog`)  
- Domain does **not** know how events are dispatched  
- Domain does **not** know who handles events  

This enforces **[Architecture Governance](ca://s?q=Open_architecture_governance)**.

---

## 2.2 Event Storage on Aggregates

Aggregates expose:

- A list of pending domain events  
- A protected method to add events  
- A method for Application to clear events  

The Domain layer **must not**:

- Dispatch events  
- Depend on Application or Infrastructure  
- Perform side effects  

Domain events are **pure business facts**, nothing more.

---

# 3. Application Responsibilities

The Application layer owns:

- Event dispatching  
- Event handler resolution  
- Transaction boundaries  
- Side‑effect orchestration  

## 3.1 IDomainEventDispatcher

````csharp
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<object> domainEvents, CancellationToken cancellationToken = default);
}
````

Responsibilities:

- Accept a batch of domain events  
- Resolve handlers via SharedKernel auto‑registration  
- Invoke handlers in a deterministic order  
- Run inside the Application layer (never API or Infrastructure)  

---

## 3.2 IDomainEventHandler

````csharp
public interface IDomainEventHandler<in TDomainEvent>
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
````

Conventions:

- Handlers live in Application  
- Handlers are auto‑registered via `[AutoRegister]`  
- Handlers must not depend on API or UI  
- Handlers may depend on repositories, readers, or domain services  
- Handlers must not raise domain events directly  

Handlers implement **side effects**, not business rules.

---

## 3.3 DomainEventDispatcher

The concrete dispatcher:

- Resolves all `IDomainEventHandler<T>` for each event type  
- Invokes handlers asynchronously  
- Runs **after** the command handler completes  
- Runs **inside** the Unit of Work boundary  
- Is invoked automatically by the persistence pipeline  

The dispatcher is registered via SharedKernel DI.

---

# 4. Infrastructure Responsibilities

Infrastructure participates in domain event flow only through:

- Transaction boundaries  
- Unit of Work implementation  
- EF Core change tracking  

Infrastructure may:

- Integrate with external systems *from Application handlers*  
- Persist domain events (future capability)  

Infrastructure must **not**:

- Dispatch domain events  
- Contain domain logic  
- Depend on API or UI  
- Raise domain events  

Infrastructure is a **consumer**, not a producer.

---

# 5. Event Flow

The full event lifecycle:

1. **Command handler executes**  
2. Handler calls aggregate methods  
3. Aggregates raise domain events  
4. Handler calls `CommitAsync` on `IUnitOfWork`  
5. Unit of Work:  
   - Saves changes  
   - Collects domain events  
   - Clears domain events from aggregates  
   - Calls `IDomainEventDispatcher.DispatchAsync`  
6. Dispatcher resolves and invokes all matching handlers  
7. Handlers perform side effects (email, logging, projections, etc.)  

This enforces **[Dispatcher Pipeline](ca://s?q=Open_dispatcher_pipeline_guide)**.

---

# 6. Purity Rules

- Domain events are defined in Domain or SharedKernel  
- Domain must not reference Application or Infrastructure  
- Event handlers live in Application  
- Event handlers must not contain business rules  
- API must never dispatch domain events  
- Infrastructure must never dispatch domain events  
- Domain events must not cross the API boundary  
- Domain events must not be returned from endpoints  
- Domain events must not be serialized  

This enforces **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**.

---

# 7. Contributor Guidelines

When adding a new domain event:

1. Define the event in Domain (or SharedKernel if cross‑aggregate)  
2. Raise it from aggregate methods  
3. Add one or more `IDomainEventHandler<T>` implementations in Application  
4. Mark handlers with `[AutoRegister]`  
5. Do **not** dispatch events manually  
6. Do **not** raise events from handlers  
7. Let the Unit of Work + dispatcher pipeline handle everything  
8. Ensure handlers contain no business logic  
9. Ensure handlers contain no Infrastructure references  

If a domain event handler grows beyond ~20–30 lines, logic is leaking into the wrong layer.

---

# Related Documents

- **[Dispatcher Pipeline](ca://s?q=Open_dispatcher_pipeline_guide)**  
- **[API Endpoint Purity](ca://s?q=Generate_API_Endpoint_Purity_Guide)**  
- **[Architecture Governance](ca://s?q=Open_architecture_governance)**  
- **[Code Conventions](ca://s?q=Open_code_conventions)**  
- **[Dependency Injection Architecture](ca://s?q=Open_dependency_injection_architecture)**  
