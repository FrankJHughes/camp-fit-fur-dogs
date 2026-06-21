# Domain Aggregate Root — User Guide

This guide explains how users of the Frank platform should work with **Domain Aggregate Roots** today, and how the experience will evolve in the future.

Domain Aggregate Roots represent the **core business objects** in a Frank‑based system.  
They enforce business rules, maintain consistency, and raise domain events when meaningful changes occur.

This guide focuses on how to *use* aggregates — not how to implement the underlying capability.

---

# 1. Current State (Before Future Enhancements)

Frank currently provides the following domain primitives:

- **AggregateRoot<TId>** — the root of a consistency boundary  
- **Entity<TId>** — identity‑based domain objects  
- **ValueObject** — immutable, structurally equal objects  
- **AggregateId** — strongly‑typed identifiers  
- **Domain event lifecycle** — raise and clear events  

### What this means for you today

As a user of aggregates:

- You **create** aggregates using constructors or factory methods  
- You **call methods** on aggregates to perform business operations  
- Aggregates **raise domain events** when something meaningful happens  
- You **retrieve domain events** from the aggregate  
- You **clear domain events** after dispatching them  
- You **do not persist domain events**  
- You **do not mutate aggregates directly**  

### What aggregates do today

- Enforce business rules  
- Maintain internal consistency  
- Raise domain events  
- Expose identity through strongly‑typed IDs  
- Compare entities by identity  
- Compare value objects structurally  

### What aggregates do *not* do today

- No automatic domain event dispatching  
- No lifecycle hooks  
- No versioning or concurrency tokens  
- No snapshotting  
- No persistence behavior  
- No validation framework  
- No automatic invariant enforcement  

Aggregates are intentionally minimal and pure.

---

# 2. Intended Future State (After Capability Expansion)

As the platform evolves, aggregates will gain richer behavior.

### Future enhancements may include:

- **Automatic domain event dispatching**  
- **Integration with Unit of Work**  
- **Integration with Outbox pattern**  
- **Aggregate lifecycle hooks**  
- **Versioning and concurrency**  
- **Snapshotting**  
- **Built‑in invariant enforcement**  
- **Value object validation and conversion**  
- **Aggregate factories**  
- **Domain services and policies**  

These features will make aggregates more powerful and easier to use.

---

# 3. How You Use Aggregates Today

## 3.1 Create an aggregate

Aggregates are created using constructors or factory methods.

```csharp
var customer = new Customer(
    new CustomerId(Guid.NewGuid()),
    new Email("test@example.com"));
```

## 3.2 Perform operations

Call methods on the aggregate to perform business actions.

```csharp
customer.ChangeEmail(new Email("new@example.com"));
```

## 3.3 Retrieve domain events

Aggregates raise domain events when meaningful changes occur.

```csharp
var events = customer.DomainEvents;
```

## 3.4 Clear domain events after dispatch

After dispatching events, clear them.

```csharp
customer.ClearDomainEvents();
```

## 3.5 Use strongly‑typed IDs

IDs are value objects, not raw GUIDs.

```csharp
var id = customer.Id; // CustomerId
```

## 3.6 Use value objects for domain concepts

Value objects enforce immutability and structural equality.

```csharp
var email = new Email("test@example.com");
```

---

# 4. What Aggregates Guarantee

### **Consistency**
Aggregates enforce their own invariants.

### **Identity**
Entities compare equal when their IDs match.

### **Immutability of Value Objects**
Value objects cannot be mutated after creation.

### **Domain Event Lifecycle**
- Events are raised when state changes  
- Events accumulate in memory  
- Events must be cleared manually  

### **Encapsulation**
State changes only through aggregate methods.

---

# 5. What Aggregates Do *Not* Guarantee

### **No automatic persistence**
Aggregates do not save themselves.

### **No automatic event dispatching**
Domain events must be dispatched by the application layer.

### **No validation framework**
Aggregates enforce invariants manually.

### **No lifecycle hooks**
No `OnCreated`, `OnUpdated`, or `OnDeleted` yet.

### **No versioning**
Aggregates do not track concurrency or snapshots.

### **No cross‑aggregate consistency**
Aggregates enforce only their own invariants.

---

# 6. Example: Using an Aggregate in an Application Handler

```csharp
public sealed class ChangeCustomerEmailHandler
{
    private readonly ICustomerRepository _repo;
    private readonly IDomainEventDispatcher _dispatcher;

    public ChangeCustomerEmailHandler(
        ICustomerRepository repo,
        IDomainEventDispatcher dispatcher)
    {
        _repo = repo;
        _dispatcher = dispatcher;
    }

    public async Task HandleAsync(ChangeCustomerEmailCommand cmd, CancellationToken ct)
    {
        var customer = await _repo.GetAsync(cmd.CustomerId, ct);

        customer.ChangeEmail(new Email(cmd.NewEmail));

        foreach (var evt in customer.DomainEvents)
            await _dispatcher.DispatchAsync(evt, ct);

        customer.ClearDomainEvents();

        await _repo.SaveAsync(customer, ct);
    }
}
```

This demonstrates:

- Calling aggregate methods  
- Retrieving domain events  
- Dispatching domain events  
- Clearing domain events  
- Persisting the aggregate  

---

# 7. Summary

**Current State:**  
Aggregates provide:

- Strongly‑typed identity  
- Structural equality for value objects  
- Domain event lifecycle  
- Invariant enforcement through methods  
- Encapsulation of domain behavior  

**Future State:**  
Aggregates will support:

- Automatic event dispatching  
- Lifecycle hooks  
- Versioning  
- Snapshotting  
- Outbox integration  
- Domain services and policies  

As a user of this capability:

- Today, you call aggregate methods and handle domain events manually  
- In the future, Frank will automate more of the domain lifecycle  

