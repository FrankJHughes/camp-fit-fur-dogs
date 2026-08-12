
# CampFitFurDogs.Domain.Exceptions

The `CampFitFurDogs.Domain.Exceptions` namespace contains domain‑level exception types used to signal violations of domain assumptions or failures originating from external systems that the domain depends on.  
These exceptions represent **pure domain concerns** and are thrown when the domain model cannot proceed due to invalid state, broken invariants, or external identity/authentication failures.

This namespace is intentionally small and focused. Domain exceptions should be rare, meaningful, and represent conditions that the domain itself cannot resolve.

---

## 🎯 Purpose

Domain exceptions exist to:

- Protect domain invariants  
- Surface unrecoverable domain‑level failures  
- Signal when external systems violate domain assumptions  
- Provide clear, intention‑revealing error types for upstream layers  

They are consumed by the **[application layer](ca://s?q=Tell_me_more_about_the_application_layer)**, which translates them into appropriate application‑level responses or error codes.

---

## 📦 Included Exceptions

### `ExternalAuthProviderException`
Thrown when an external authentication or identity provider fails in a way that breaks domain assumptions.

Examples include:

- Missing or malformed identity data  
- Provider returning an error during identity resolution  
- External system failing to supply required user information  

This exception indicates that the domain cannot continue because an external dependency violated a contract the domain relies on.

---

## 🧭 When to Use Domain Exceptions

Domain exceptions should be used only when:

- A domain invariant is violated  
- A domain rule cannot be satisfied  
- An external system breaks a domain assumption  
- The domain cannot self‑correct or recover  

They should **not** be used for:

- Validation errors (handled by FluentValidation in the application layer)  
- Infrastructure failures (handled in the infrastructure layer)  
- API‑level concerns (handled in the API layer)  

For those cases, see:

- [application exceptions](ca://s?q=Show_me_application_exceptions)  
- [infrastructure exceptions](ca://s?q=Explain_infrastructure_exceptions)  
- [API error handling](ca://s?q=How_does_API_error_handling_work)

---

## 🚫 What Does *Not* Belong Here

The domain exceptions namespace must **not** contain:

- Application‑layer exceptions  
- Infrastructure exceptions  
- HTTP or API error types  
- Validation logic  
- Logging or telemetry concerns  

Only **pure domain failures** belong here.

---

## 📚 Related Namespaces

- `CampFitFurDogs.Domain` — aggregates, value objects, invariants  
- `CampFitFurDogs.Application.Exceptions` — structural and identity‑consistency failures  
- `CampFitFurDogs.Infrastructure` — persistence and external system errors  
- `CampFitFurDogs.Api` — HTTP‑level error mapping  

---

