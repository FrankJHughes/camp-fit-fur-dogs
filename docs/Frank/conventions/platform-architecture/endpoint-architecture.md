# endpoint-architecture.md

# Endpoint Architecture (Frank)

Endpoints are the **HTTP boundary** of a Frank‑based system.

They are responsible for:

- translating HTTP requests into application operations  
- enforcing authentication and authorization boundaries  
- applying cross‑cutting concerns (logging, correlation, rate limiting, etc.)  
- shaping HTTP responses from application results  

Endpoints are **thin**, **orchestrating** components — they do not contain
business logic or persistence logic.

---

## Purpose

Endpoint architecture exists to:

- clearly separate transport concerns (HTTP) from application concerns  
- enforce security boundaries at the edge  
- provide consistent request/response shaping  
- centralize cross‑cutting HTTP behaviors  
- prevent application code from depending on HTTP primitives  

Endpoints are the **only** place where HTTP concepts are allowed.

---

## Endpoint Responsibilities

Endpoints must:

- bind HTTP input (route, query, headers, body) to DTOs  
- enforce authentication and authorization requirements  
- invoke the appropriate command or query pipeline  
- translate application results into HTTP responses  
- apply HTTP‑level cross‑cutting concerns (logging, correlation, etc.)  

Endpoints must not:

- contain business rules  
- talk directly to the database  
- construct domain entities  
- perform persistence  
- bypass the command/query pipelines  

---

## Endpoint Flow

A typical endpoint executes in this order:

1. **Routing**  
   HTTP request is matched to an endpoint.

2. **Authentication**  
   The caller is authenticated (if required).

3. **Authorization**  
   The caller’s permissions are checked against the endpoint’s policy.

4. **Input Binding**  
   Request data is bound to a request DTO.

5. **Application Invocation**  
   - command pipeline for writes  
   - query pipeline for reads  

6. **Result Mapping**  
   Application result is mapped to an HTTP response.

7. **Response Emission**  
   HTTP status code, headers, and body are written.

Cross‑cutting behaviors (logging, correlation, rate limiting, etc.) may wrap
this flow via middleware or endpoint filters, but the **core responsibilities**
remain the same.

---

## Authentication Boundary

Authentication is an **edge concern**.

Endpoints must:

- declare whether authentication is required  
- rely on the platform’s authentication mechanisms (e.g., OIDC, cookies)  
- never implement custom authentication logic per endpoint  

Application code must not:

- depend on HTTP authentication primitives  
- read authentication cookies directly  
- parse tokens directly  

Authentication state is exposed to the application layer only via:

- user identity abstractions  
- claims/principal abstractions  

---

## Authorization Boundary

Authorization is enforced at the endpoint boundary.

Endpoints must:

- declare authorization requirements (policies, roles, scopes, etc.)  
- fail fast with appropriate HTTP status codes (e.g., 403)  
- never allow unauthorized access to application operations  

Application code must not:

- perform ad‑hoc authorization checks using HTTP primitives  
- depend on route/URL information for authorization decisions  

Authorization logic belongs in:

- policies  
- handlers  
- domain rules (expressed via commands/queries), not HTTP details  

---

## Request/Response Shaping

Endpoints are responsible for:

- mapping HTTP input → request DTOs  
- mapping application responses → HTTP responses  

They must:

- use DTOs, not domain entities  
- return consistent status codes for common cases:
  - 200/201 for success  
  - 400 for validation errors  
  - 401 for unauthenticated  
  - 403 for unauthorized  
  - 404 for not found  
  - 409 for conflict  
  - 500 for unexpected failures (shaped, not raw)  

Endpoints must not:

- return domain entities directly  
- leak internal exception details  
- expose infrastructure types  

---

## Cross‑Cutting HTTP Concerns

The following concerns are handled at the endpoint/HTTP layer (often via
middleware):

- logging and correlation IDs  
- rate limiting (US‑132)  
- security headers (US‑134)  
- CORS (US‑135)  
- account lockout signaling (US‑133)  
- observability events (US‑183)  

Endpoints must not re‑implement these concerns per action.

---

## Separation from Application Layer

Endpoints may depend on:

- DTOs  
- command/query contracts  
- application services (if present as facades)  

Endpoints must not depend on:

- domain entities  
- repositories  
- EF DbContexts  
- infrastructure services (SMTP, queues, etc.)  

All such concerns must be accessed via:

- commands  
- queries  
- domain/application services behind the pipelines  

---

## Testability

Endpoint behavior must be testable at two levels:

- **Endpoint‑level tests**  
  - routing  
  - authentication/authorization behavior  
  - request/response shaping  
  - status codes  

- **Application‑level tests**  
  - command/query behavior  
  - domain rules  
  - persistence  

Endpoint tests must not duplicate application logic tests.

---

## Prohibitions

Endpoints must not:

- contain business logic  
- perform persistence  
- bypass command/query pipelines  
- construct or mutate domain entities  
- perform ad‑hoc authentication or authorization  
- leak internal errors or stack traces  

---

## Enforcement

Endpoint architecture is enforced through:

- guardrail tests  
- analyzer rules (where applicable)  
- conventions in endpoint registration  
- documentation and code review  

Endpoints must remain:

- **thin**  
- **transport‑focused**  
- **platform‑governed**  
- **product‑agnostic**  
