# Frank – Conventions – Code – OIDC Callback Conventions

Frank provides a **callback‑focused OIDC handler** implemented using the `ImmutableContextBuilder` pattern.  
This handler performs the minimum protocol work required to support an OIDC authorization‑code callback.

It is **not** a general OIDC engine.  
It is **not** a full protocol pipeline.  
It is a **deterministic, step‑based callback processor**.

Products (e.g., CampFitFurDogs) consume the result and perform all product‑specific behavior.

---

# Purpose

The OIDC callback handler exists to:

- exchange the authorization code  
- retrieve user information (if configured)  
- validate tokens and protocol parameters  
- extract required claims  
- produce a **callback result** for the product layer  

It does **not** perform:

- user creation  
- session creation  
- redirect computation  
- cookie issuance  
- persistence  
- product‑specific logic  

Frank handles only the **protocol mechanics** required to complete the callback.

---

# Architecture

The callback handler is implemented using:

- `IImmutableContextBuilder<TRequest, TContext, TResult>`
- `IImmutableContextBuildStep<TContext>`
- Three concrete steps:
  - `ExchangeCodeStep`
  - `FetchUserInfoStep`
  - `ValidateTokensStep`
- One concrete builder:
  - `OidcAuthCallbackContextBuilder`

Each step receives an immutable context and returns a new immutable context.

---

# DI Registration

The callback system is registered via:

```csharp
services.AddFrankAuthCallback();
```

This registers:

- `AuthCallbackOidcSettings` (bound from configuration)  
- All build steps (`ExchangeCodeStep`, `FetchUserInfoStep`, `ValidateTokensStep`)  
- The context builder (`OidcAuthCallbackContextBuilder`)  

No other DI registration is required.  
Products must not manually register these components.

---

# Settings

The callback handler binds:

```
Authentication:Callback:Oidc
```

into:

```csharp
AuthCallbackOidcSettings
```

Settings must be:

- validated with data annotations  
- validated on startup  

---

# Responsibilities

The callback handler must:

- read the authorization code and state  
- call the IdP token endpoint  
- optionally call the user‑info endpoint  
- validate:
  - issuer  
  - audience  
  - expiry  
  - nonce  
  - state  
- extract required claims  
- produce a `FrankAuthCallbackResult`  

The handler must **not**:

- create users  
- load users  
- create sessions  
- compute redirects  
- issue cookies  
- persist anything  
- depend on product types  

Products own all post‑callback behavior.

---

# HttpClient Usage

Outbound calls must:

- use named HttpClients  
- use Frank’s HttpClient seam  
- never use `new HttpClient()`  

Tests must use:

- `FakeHttpMessageHandler`  
- `WithServiceOverride`  

No real network calls are permitted in tests.

---

# Error Behavior

The callback handler must:

- throw mapped exceptions for protocol failures  
- never expose raw IdP payloads  
- never leak sensitive token data  

Products shape these exceptions into HTTP responses.

---

# Testability Requirements

The callback handler must be fully testable using:

- fake HttpClient handlers  
- fake configuration  
- fake environment seams  

Tests must assert:

- correct token exchange behavior  
- correct user‑info behavior  
- correct validation behavior  
- correct claim extraction  
- correct result mapping  

Tests must not:

- hit real IdPs  
- require real secrets  
- require real network calls  

---

# Prohibitions

The callback handler must not:

- depend on product code  
- depend on product configuration formats  
- depend on product domain models  
- perform persistence  
- perform business logic  
- compute redirect URLs  
- issue cookies  
- create sessions  
- load users  

It is strictly a **callback‑level protocol component**, not a full OIDC engine.
