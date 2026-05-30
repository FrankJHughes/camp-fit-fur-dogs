# API Governance

This document defines the boundary rules, guarantees, and enforcement mechanisms for all API surfaces in the repository.  
It complements (but does not duplicate) architecture governance, security governance, and conventions.

API governance ensures:

- Stable, predictable API behavior  
- Clear boundary responsibilities  
- Consistent error handling  
- Safe authentication and authorization  
- Long-term API stability  
- Zero accidental breaking changes  

The API is a contract. Breaking it is a product failure.

---

# 1. API Boundary Principles

The API must:

- Be stable  
- Be explicit  
- Be predictable  
- Be secure  
- Be versionable  
- Be observable  
- Be testable  

The API boundary is the system’s public surface and must be treated as such.

---

# 2. API Responsibilities

The API layer is responsible for:

- HTTP endpoint exposure  
- Authentication  
- Authorization  
- Request validation  
- Response shaping  
- Error shaping  
- Session cookie issuance  
- Mapping to/from Application layer  
- Using dispatchers (`ICommandDispatcher`, `IQueryDispatcher`)  
- Using SharedKernel endpoint discovery  

The API layer must not:

- Contain business logic  
- Access the database  
- Call external services directly  
- Construct domain entities  
- Perform domain mutations outside Application layer  
- Invoke handlers directly (must use dispatchers)  
- Depend on Infrastructure  

---

# 3. Authentication Governance

Authentication must:

- Occur at the API boundary  
- Use secure OIDC flows (US‑110)  
- Issue opaque session cookies only  
- Never expose tokens to the frontend  
- Never store tokens in localStorage  
- Fail fast on misconfiguration  

Authentication failures must:

- Never leak provider details  
- Never expose raw error messages  
- Always return a safe, generic error  

Identity resolution must:

- Use `ICurrentUserService`  
- Never accept identity from request bodies  
- Never rely on frontend checks  

---

# 4. Authorization Governance

Authorization must:

- Be explicit  
- Be enforced at the API boundary  
- Use Application layer abstractions  
- Never rely on frontend checks  
- Never be implemented in the UI  

Endpoints must declare:

- Required roles  
- Required permissions  
- Required identity context  

Endpoints without explicit authorization are prohibited.

---

# 5. Error Boundary Governance

Errors must:

- Be shaped consistently  
- Never expose stack traces  
- Never expose internal exceptions  
- Never expose infrastructure details  
- Never expose domain internals  

API errors must be:

- Deterministic  
- Structured  
- Documented  
- Stable  

Error shaping must use SharedKernel error boundary helpers.

---

# 6. Versioning Governance

The API must:

- Support versioning  
- Avoid breaking changes  
- Deprecate endpoints explicitly  
- Provide migration paths  
- Document version changes  

Breaking changes require:

- Product Owner approval  
- Migration documentation  
- Changelog updates  

---

# 7. Stability & Compatibility Governance

The API must guarantee:

- Backward compatibility within a major version  
- Stable response shapes  
- Stable error shapes  
- Stable authentication behavior  
- Stable session behavior  

Forbidden:

- Changing response shapes without versioning  
- Changing error shapes without versioning  
- Changing authentication flows without approval  
- Removing fields without deprecation  

---

# 8. Session Boundary Governance

Session cookies must:

- Be issued only by the API  
- Be validated only by the API  
- Contain opaque identifiers only  
- Use secure flags in production  
- Never contain tokens  
- Never contain PII  

Session creation must be audited via `IAuditLogger`.

---

# 9. API Observability Governance

The API must provide:

- Structured logs  
- Request correlation  
- Authentication event logs  
- Hosting provider selection logs  
- Startup validation logs  

Logs must never contain:

- Secrets  
- Tokens  
- PII  

---

# 10. Forbidden API Patterns

The following patterns are prohibited:

- Returning domain entities  
- Returning infrastructure types  
- Accepting domain entities in requests  
- Accepting unvalidated JSON  
- Using exceptions for control flow  
- Leaking internal exception messages  
- Using HTTP 200 for error states  
- Using HTTP 500 for validation errors  
- Invoking handlers directly (must use dispatchers)  
- Depending on Infrastructure  

---

# 11. API Enforcement

API governance is enforced through:

- Automated endpoint tests  
- Automated error-shaping tests  
- CI validation  
- Reviewer enforcement  
- SharedKernel guardrails  
- Dispatcher pipeline tests  

No PR may merge if:

- It breaks API stability  
- It weakens authentication or authorization  
- It exposes internal details  
- It violates API boundary rules  

API governance is non-negotiable.
