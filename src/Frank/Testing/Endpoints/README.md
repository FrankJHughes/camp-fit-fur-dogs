# Testing — Endpoints

The **Endpoints** folder contains lightweight, test‑only API endpoints used by the
Frank testing harness.  
These endpoints are intentionally simple and deterministic, providing a stable
surface for verifying authentication behavior, error handling, and basic host
availability.

They are **not** intended for production use.  
Their purpose is to support integration tests, diagnostics, and controlled
failure scenarios.

This folder currently contains:

- `CurrentUserIdEndpoint` — exposes the authenticated user ID  
- `HealthCheckEndpoint` — verifies host availability  
- `ThrowEndpoint` — intentionally triggers an exception  

---

## Purpose

The Endpoints subsystem provides:

- A predictable way to inspect authentication state  
- A simple health probe for test hosts  
- A controlled exception path for testing error middleware  
- Anonymous access for all test endpoints  
- A stable API surface that does not change across environments  

These endpoints are automatically registered when included in the test host’s
endpoint assemblies.

---

## Files

### **CurrentUserIdEndpoint**

Returns the current authenticated user’s `NameIdentifier` claim (OIDC `sub`).

Responsibilities:

- Reads the user ID from `HttpContext.User`  
- Returns `{ userId }` as JSON  
- Supports authenticated and unauthenticated scenarios  
- Helps verify:
  - Claim injection  
  - Authentication simulation  
  - Test client mutation  

Example response:

```json
{ "userId": "auth0|123" }
```

---

### **HealthCheckEndpoint**

A simple health probe for test hosts.

Responsibilities:

- Returns `{ status = "ok" }`  
- Always anonymous  
- Used to verify:
  - Host startup  
  - Routing  
  - Basic request handling  

Example response:

```json
{ "status": "ok" }
```

---

### **ThrowEndpoint**

An endpoint that intentionally throws an exception.

Responsibilities:

- Throws `InvalidOperationException("Test exception")`  
- Always anonymous  
- Used to verify:
  - Exception middleware  
  - Logging pipelines  
  - Error response formatting  
  - Client resilience  

Example behavior:

Calling `/__test__/throw` results in a 500 response.

---

## Design Principles

The Endpoints subsystem follows these principles:

- **Test‑only behavior**  
  These endpoints exist solely for diagnostics and integration tests.

- **Minimal logic**  
  Each endpoint performs exactly one simple, predictable action.

- **Anonymous access**  
  Tests should not require authentication to verify host behavior.

- **Deterministic responses**  
  No randomness, no environment‑dependent behavior.

- **Separation from production code**  
  These endpoints live in the testing layer, not the application layer.

---

## Example Usage

```csharp
var client = factory.CreateClient(clientCtx);

var health = await client.GetStringAsync("/__test__/health");
var userId = await client.GetStringAsync("/__test__/current-user-id");

try
{
    await client.GetAsync("/__test__/throw");
}
catch
{
    // Expected: test exception path
}
```

---

## Summary

The **Endpoints** folder provides a small set of deterministic, test‑focused API
endpoints:

- Current user identity inspection  
- Health probing  
- Controlled exception triggering  

These endpoints are essential tools for verifying authentication flows,
middleware behavior, and host stability within the Frank testing harness.

