# exception-to-problem-architecture.md

# CampFitFurDogs — Exception‑to‑Problem Architecture

The **Exception‑to‑Problem Architecture** in **CampFitFurDogs.Api** defines how
exceptions thrown inside the application are captured, classified, translated,
and surfaced as **Problem** responses at the HTTP boundary.

Internally, CampFitFurDogs uses **exceptions** to signal failure.  
Externally, it returns **ProblemDetails**-style responses to clients.

This is a **product‑level** architecture — it is implemented in
CampFitFurDogs, not in the Frank platform.

---

## Purpose

The CampFitFurDogs Exception‑to‑Problem pipeline exists to:

- centralize exception handling in the API  
- prevent leakage of internal exception details  
- ensure consistent Problem shapes across endpoints  
- guarantee deterministic HTTP status codes for failures  
- support observability and diagnostics without exposing internals  

Exceptions are **internal signals**.  
Problems are the **public contract**.

---

## High‑Level Flow

1. **Application code throws an exception**  
   - validation failure  
   - domain rule violation  
   - authentication/authorization failure  
   - infrastructure failure  
   - unexpected failure  

2. **CampFitFurDogs exception handling middleware catches the exception**  
   No exception escapes past this middleware.

3. **Exception registry maps the exception to an error code / category**  
   This classifies the failure.

4. **Error code/category maps to a Problem**  
   - `ProblemDetails`  
   - `ValidationProblemDetails`  
   - domain‑specific problem  
   - infrastructure problem  
   - authentication/authorization problem  

5. **Problem is shaped into an HTTP response**  
   - status code  
   - content type  
   - body  

6. **Response is returned to the client**  
   No exception details are exposed.

---

## Exception Categories (CampFitFurDogs)

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

## Problem Shaping

Problems are shaped into safe, consistent responses, typically following
`ProblemDetails` / `ValidationProblemDetails` patterns:

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

## HTTP Mapping (CampFitFurDogs)

Typical mappings:

- Validation → **400 Bad Request**  
- Authentication → **401 Unauthorized**  
- Authorization → **403 Forbidden**  
- Not Found → **404 Not Found**  
- Domain rule violation → **409 Conflict**  
- Infrastructure / unexpected → **500 Internal Server Error**  

These mappings are enforced by the exception handling middleware and registry.

---

## Integration Points

The Exception‑to‑Problem pipeline in CampFitFurDogs:

- wraps all API endpoints  
- sees exceptions thrown from:
  - command handlers  
  - query handlers  
  - domain services  
  - infrastructure services  
- shapes them into Problems before they reach the client  

Endpoints in CampFitFurDogs:

- do **not** handle exceptions directly  
- do **not** shape Problems manually  
- rely on the middleware to do so consistently  

---

## Observability

When an exception is translated into a Problem, CampFitFurDogs:

- logs the exception with correlation information  
- records structured error events for diagnostics  
- ensures that logs contain enough context for debugging  
- keeps client‑visible responses safe and minimal  

---

## Prohibitions

In CampFitFurDogs.Api:

- endpoints must not:
  - catch and swallow exceptions  
  - return raw exception messages  
  - construct ad‑hoc error responses  

- application code must not:
  - shape HTTP responses directly for failures  
  - bypass the exception handling middleware  

All failure paths must flow through the **Exception‑to‑Problem** pipeline.

---

## Scope

This architecture:

- is **specific to CampFitFurDogs**  
- is **not yet a Frank platform primitive**  
- may be promoted to a Frank‑level convention in the future if:
  - multiple products adopt it  
  - a shared Frank exception/Problem layer is introduced  

For now, it remains a **product‑level architecture** documented under
`/docs/CampFitFurDogs/architecture`.
