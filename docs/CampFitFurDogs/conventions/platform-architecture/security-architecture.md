# security-architecture.md

# Security Architecture

CampFitFurDogs enforces a strict, centralized security model.  
Security is **not optional**, **not per‑endpoint**, and **not developer‑defined**.  
It is applied globally through Frank’s hardened security primitives.

Security responsibilities are split across:

- **Frank** — provides the security middleware and header policies  
- **Api** — applies the middleware  
- **Application** — enforces business‑level authorization  
- **Infrastructure** — must not bypass or weaken security  

This document defines the conventions for how security is applied across the system.

---

## Security Headers

The Api must apply Frank’s security headers middleware:

- `AddSecurityHeaders()`  
- `SecurityHeadersMiddleware`  

These headers include:

- `X-Content-Type-Options`  
- `X-Frame-Options`  
- `X-XSS-Protection`  
- `Referrer-Policy`  
- `Content-Security-Policy` (CSP)  
- `Permissions-Policy`  

### Rules

- Headers must be applied **globally**, not per‑endpoint.  
- Headers must not be overridden by slices or modules.  
- CSP must not be weakened without an ADR.  
- Middleware must be registered **before** any endpoint mapping.  

Security headers are a **non‑negotiable baseline**.

---

## Authorization

Authorization is enforced at the **Api layer**, using:

- declarative policies  
- role‑based access  
- identity resolved by the Application authentication pipeline  

### Rules

- Authorization must be declarative, not imperative.  
- Authorization must not embed business logic.  
- Authorization must not depend on Infrastructure.  
- Authorization must not read identity from request bodies.  
- Authorization must not be duplicated across endpoints.  

Authorization is a **policy**, not a conditional statement.

---

## Identity Handling

Identity is resolved exclusively by the **Application authentication pipeline**.

Api endpoints must:

- rely on the resolved identity  
- never parse tokens  
- never validate tokens  
- never extract claims manually  

Identity is a **resolved object**, not a token.

---

## Transport Security

Transport security is enforced at the hosting layer:

- HTTPS is mandatory  
- HTTP must redirect to HTTPS  
- HSTS must be enabled  
- TLS configuration must follow Frank’s hosting provider defaults  

Slices must not modify transport security.

---

## Input Validation

Validation occurs in the **Application layer**, not Api.

Api must:

- bind DTOs  
- delegate validation to Application  
- never perform business validation  
- never trust client input  

Validation is a **business concern**, not a transport concern.

---

## Enforcement

Security conventions are enforced through:

- guardrail tests  
- middleware registration tests  
- dependency analysis  
- code review  
- conventions governance  

Security is a **system‑wide contract**, not a developer preference.
