# API Governance

This document defines the boundary rules, guarantees, and enforcement mechanisms for all API surfaces in the repository.  
It complements (but does not duplicate) architecture governance, security governance, and conventions.

API governance ensures:

- Stable, predictable API behavior  
- Clear boundary responsibilities  
- Consistent error handling  
- Safe authentication and authorization  
- Long‑term API stability  
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
- Using Frank endpoint discovery  
- Applying Frank security headers middleware  
- Integrating with Frank’s correlation ID middleware  
- Integrating with Frank’s error boundary middleware  

The API layer must not:

- Contain business logic  
- Access the database  
- Call external services directly  
- Construct domain entities  
- Perform domain mutations outside Application layer  
- Invoke handlers directly (must use dispatchers)  
- Depend on Infrastructure  
- Bypass Frank’s cross‑cutting middleware (security headers, error boundary, correlation)

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
- Never bypass Frank’s authentication seams  

Session creation must:

- Use Frank’s session helpers  
- Be auditable via `IAuditLogger`  
- Produce deterministic, secure cookies  

## 3.1 Authentication Callback Governance (NEW)

The authentication callback endpoint must:

- Act as a **thin orchestrator**  
- Extract the `code` query parameter  
- Throw a shaped `400` error when missing  
- Invoke the **Frank pipeline** (protocol logic only)  
- Invoke the **Application pipeline** (business logic only)  
- Issue the session cookie using the Application pipeline’s result  
- Redirect to the URL provided by the Application pipeline  

The callback endpoint must not:

- Contain protocol logic  
- Contain business logic  
- Construct aggregates  
- Perform persistence  
- Compute redirect URLs  
- Generate cookie values  
- Call external identity providers directly  

The callback flow must be:

- Deterministic  
- Testable  
- Fully observable  
- Fully shaped by Frank’s error boundary  

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

Authorization must integrate with:

- Frank’s dispatcher pipeline  
- Frank’s identity seam  
- Frank’s error boundary  

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

Error shaping must use:

- Frank’s error boundary helpers  
- Frank’s correlation ID middleware  
- Frank’s security headers middleware  

The API must never:

- Return raw exceptions  
- Return framework‑generated HTML error pages  
- Return inconsistent error shapes  

## 5.1 Callback Error Governance (NEW)

The callback endpoint must:

- Return `400` for missing or invalid `code`  
- Allow pipeline exceptions to flow into Frank’s error boundary  
- Never leak identity provider errors  
- Never leak token or claims data  
- Never return protocol‑level details  

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
- Updated API tests  
- Updated governance documentation  

Versioning must integrate with:

- Frank endpoint discovery  
- Frank error boundary  
- Frank security headers  

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
- Introducing new required fields without versioning  
- Changing cookie behavior without governance approval  

## 7.1 Callback Stability Guarantees (NEW)

The callback endpoint must maintain:

- Stable redirect semantics  
- Stable cookie issuance semantics  
- Stable pipeline invocation order  
- Stable error semantics  

---

# 8. Session Boundary Governance

Session cookies must:

- Be issued only by the API  
- Be validated only by the API  
- Contain opaque identifiers only  
- Use secure flags in production  
- Never contain tokens  
- Never contain PII  

Session creation must:

- Use Frank’s session helpers  
- Integrate with Frank’s correlation ID  
- Be audited via `IAuditLogger`  
- Produce deterministic, testable behavior  

Session validation must:

- Occur before endpoint execution  
- Use Frank’s identity seam  
- Never rely on frontend checks  

## 8.1 Callback Session Governance (NEW)

The callback endpoint must:

- Issue cookies **only** using the Application pipeline’s `CookieValue`  
- Never compute cookie values directly  
- Never embed identity provider tokens  
- Never embed PII  
- Never embed claims not approved by governance  

---

# 9. API Observability Governance (UPDATED)

The API must integrate with Frank’s Observability platform and must emit:

- Structured events  
- Metrics  
- Correlation IDs  
- Deterministic, testable observability output  

Observability is a required cross‑cutting concern for all API endpoints.

## 9.1 Correlation Governance

The API must:

- Use Frank’s correlation middleware  
- Accept the `IObservabilityContext` created by Frank.Hosting  
- Propagate correlation IDs through:
  - API handlers  
  - Application dispatchers  
  - Domain services  
  - Infrastructure adapters  
  - Outbox handlers  

The API must not:

- Generate correlation IDs manually  
- Mutate the observability context  
- Bypass Frank’s correlation middleware  

## 9.2 Event Governance

The API must emit structured events using `ITraceEvents` for:

- Request start  
- Request end  
- Validation failures  
- Authorization failures  
- Authentication failures  
- Dispatcher invocation  
- External call boundaries  

Event names must follow:

````  
slice.module.action  
````

Events must include:

- Correlation ID  
- Slice  
- Module  
- Action  
- Severity  
- Structured payload  

The API must not:

- Use ad‑hoc logging  
- Emit unstructured strings  
- Emit events without correlation IDs  

## 9.3 Metric Governance

The API must emit metrics using `IMetrics` for:

- Handler duration  
- Dispatcher duration  
- External call latency  
- Success/failure counters  

Metric names must follow:

````  
slice.module.metric_name  
````

Metrics must be deterministic and must not rely on real time.

The API must not:

- Use Stopwatch  
- Use vendor‑specific metric libraries  
- Emit metrics without correlation IDs  

## 9.4 Callback Observability Governance (NEW)

The authentication callback endpoint must emit:

- Callback invocation event  
- Pipeline success/failure event  
- Cookie issuance event (non‑PII)  
- Redirect target event (non‑PII)  

It must not emit:

- Authorization codes  
- Tokens  
- Claims  
- Provider error messages  
- Raw exceptions  

Callback observability must be:

- Deterministic  
- Structured  
- Fully shaped by Frank’s error boundary  
- Fully correlated  

## 9.5 Forbidden Observability Patterns

The API must never:

- Generate correlation IDs manually  
- Log secrets, tokens, or PII  
- Emit unstructured logs  
- Emit vendor‑specific logs or metrics  
- Bypass Frank’s observability abstractions  
- Use Stopwatch or real‑time timers  
- Emit inconsistent event/metric names  
- Emit observability output outside Frank’s middleware pipeline  

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
- Bypassing Frank’s middleware pipeline  
- Skipping security headers  
- Skipping correlation IDs  

## 10.1 Callback‑Specific Forbidden Patterns (NEW)

Forbidden in callback endpoints:

- Calling identity providers directly  
- Performing token exchange directly  
- Performing identity resolution directly  
- Computing redirect URLs  
- Computing cookie values  
- Returning provider errors  
- Returning raw exceptions  
- Returning unshaped errors  

---

# 11. API Enforcement

API governance is enforced through:

- Automated endpoint tests  
- Automated error‑shaping tests  
- Authentication and authorization tests  
- CI validation  
- Reviewer enforcement  
- Frank guardrails  
- Dispatcher pipeline tests  
- Security header enforcement tests  

No PR may merge if:

- It breaks API stability  
- It weakens authentication or authorization  
- It exposes internal details  
- It violates API boundary rules  
- It bypasses Frank’s cross‑cutting middleware  
- It introduces unversioned breaking changes  

API governance is non‑negotiable.
