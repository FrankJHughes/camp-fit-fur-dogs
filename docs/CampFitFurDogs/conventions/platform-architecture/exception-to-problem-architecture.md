# CampFitFurDogs — Exception‑to‑Problem - Architecture Conventions

The **Exception‑to‑Problem Architecture** in **CampFitFurDogs.Api** defines how exceptions thrown inside the application are captured, classified, translated, and surfaced as **Problem** responses at the HTTP boundary.

Internally, CampFitFurDogs uses **exceptions** to signal failure.  
Externally, it returns **ProblemDetails‑style** responses to clients.

This is a **product‑level architecture** — implemented in CampFitFurDogs, not in Frank.

---

# Purpose

The CampFitFurDogs Exception‑to‑Problem pipeline exists to:

- centralize exception handling in the API  
- prevent leakage of internal exception details  
- ensure consistent Problem shapes across endpoints  
- guarantee deterministic HTTP status codes for failures  
- support observability and diagnostics without exposing internals  

Exceptions are **internal signals**.  
Problems are the **public contract**.

---

# High‑Level Flow

````text
1. Application code throws an exception
2. CampFitFurDogs exception middleware catches it
3. Exception registry maps exception → error code / category
4. Category maps → ProblemDetails / ValidationProblemDetails
5. Problem is shaped into an HTTP response
6. Response is returned to the client (no exception details)
````

### Exception sources include:

- validation failures  
- domain rule violations  
- authentication/authorization failures  
- infrastructure failures  
- unexpected failures  

No exception escapes past the middleware.

---

# Exception Categories (CampFitFurDogs)

CampFitFurDogs classifies exceptions into categories such as:

- **Validation** — input is invalid  
- **Authentication** — caller not authenticated  
- **Authorization** — caller not authorized  
- **Domain** — business rule violation  
- **Not Found** — resource does not exist  
- **Infrastructure** — database/network/external failure  
- **Unexpected** — anything unclassified  

Each category maps to:

- a **Problem type**  
- a **HTTP status code**  

---

# Problem Shaping

Problems are shaped into safe, consistent responses, typically following:

- `ProblemDetails`  
- `ValidationProblemDetails`  

Fields include:

- `type` — problem identifier URI or code  
- `title` — human‑readable summary  
- `status` — HTTP status code  
- `detail` — optional safe detail  
- `errors` — validation errors (for validation problems)  

CampFitFurDogs must **not** include:

- stack traces  
- raw exception messages  
- internal type names  
- SQL or infrastructure details  
- secrets or sensitive data  

---

# HTTP Mapping (CampFitFurDogs)

Typical mappings:

- Validation → **400 Bad Request**  
- Authentication → **401 Unauthorized**  
- Authorization → **403 Forbidden**  
- Not Found → **404 Not Found**  
- Domain rule violation → **409 Conflict**  
- Infrastructure / unexpected → **500 Internal Server Error**  

These mappings are enforced by the exception handling middleware and registry.

---

# Integration Points

The Exception‑to‑Problem pipeline:

- wraps **all API endpoints**  
- sees exceptions thrown from:
  - command handlers  
  - query handlers  
  - domain services  
  - infrastructure services  

Endpoints in CampFitFurDogs:

- do **not** handle exceptions directly  
- do **not** shape Problems manually  
- rely entirely on the middleware  

All failure paths must flow through the **Exception‑to‑Problem** pipeline.

---

# Observability

When an exception is translated into a Problem, CampFitFurDogs:

- logs the exception with correlation information  
- emits structured error events using `ITraceEvents`  
- ensures logs contain enough context for debugging  
- ensures client‑visible responses remain safe and minimal  

Observability must follow:

- **structured payloads**  
- **no embedded JSON**  
- **no ad‑hoc logging**  
- **no vendor logging APIs**  
- **automatic correlation propagation**  

---

# Code Conventions

## API Layer

API endpoints must **not**:

- catch and swallow exceptions  
- return raw exception messages  
- construct ad‑hoc error responses  
- bypass the exception middleware  

API endpoints must:

- allow exceptions to bubble to the middleware  
- rely on the registry for classification  
- rely on the middleware for shaping  

---

## Application Layer

Application code must **not**:

- shape HTTP responses  
- translate exceptions into Problems  
- catch exceptions unless rethrowing with context  
- leak infrastructure or domain details  

Application code must:

- throw exceptions to signal failure  
- rely on the API boundary to translate them  

---

## Domain Layer

Domain code must:

- throw domain exceptions for invariant violations  
- avoid HTTP concerns  
- avoid ProblemDetails concerns  

Domain exceptions are mapped by the registry.

---

## Infrastructure Layer

Infrastructure code must:

- throw exceptions for external failures  
- avoid shaping Problems  
- avoid HTTP concerns  

Infrastructure exceptions are mapped to **Infrastructure** or **Unexpected** categories.

---

# Prohibitions

CampFitFurDogs must **never**:

- manually construct ProblemDetails in endpoints  
- return exception messages to clients  
- leak stack traces  
- bypass the exception middleware  
- use ad‑hoc logging  
- mutate observability context  
- generate correlation IDs manually  

---

# Scope

This architecture:

- is **specific to CampFitFurDogs**  
- is **not** a Frank primitive (yet)  
- may be promoted to Frank if:
  - multiple products adopt it  
  - a shared Frank exception/Problem layer is introduced  

For now, it remains a **product‑level architecture** documented under:

````text
/docs/CampFitFurDogs/architecture
````

