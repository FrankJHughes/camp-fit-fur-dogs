# Frank.CrossCutting — Authentication

Authentication validates user identity and establishes session state on each request. It is a **cross‑cutting concern**: every vertical slice (Dogs, Scheduling, Identity, Billing, etc.) relies on a consistent authentication pipeline, unified configuration, and predictable session behavior.

This document describes the authentication subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Identity
/src/Frank/Core.Api
/src/Frank/Core.Infrastructure
```

Authentication is composed of:

- **OIDC login flow**  
- **session creation and persistence**  
- **session validation on each request**  
- **current user resolution**  
- **cross‑cutting configuration and middleware**  

---

## Authentication Flow

The platform uses an external OIDC provider (Auth0) for identity verification. The flow is:

1. **Initial Login** — User is redirected to the Auth0 consent screen  
2. **Callback** — Auth0 redirects back with an authorization code  
3. **Session Creation** — Application exchanges the code for tokens and creates a session  
4. **Session Validation** — Subsequent requests validate the session token  
5. **Identity Resolution** — `ICurrentUser` resolves the authenticated user from the session  

Each step is handled by a different layer:

- OIDC redirect/callback → **API layer**  
- token exchange → **Application layer**  
- session persistence → **EF Core layer**  
- session validation → **Infrastructure + Middleware**  
- current user resolution → **Cross‑cutting runtime**  

See also:  
- [OIDC Flow](ca://s?q=Explain_identity_oidc_flow)  
- [Session Management](ca://s?q=Explain_identity_session_management)  
- [Current User Resolution](ca://s?q=Explain_identity_context_access)

---

## Configuration

Authentication is configured through `appsettings.json`:

```json
{
  "Identity": {
    "Oidc": {
      "Authority": "https://dev-f73yf4vyecgf51qh.us.auth0.com",
      "ClientId": "8BxEHo6DLRpG7Bo8sNlMjunQYKlippPH",
      "ClientSecret": "YOUR_CLIENT_SECRET",
      "CallbackUrl": "https://app.example.com/identity/callback",
      "Disabled": "false"
    },
    "Session": {
      "Ttl": "7.00:00:00"
    }
  }
}
```

### OIDC Settings

- **Authority** — Auth0 tenant  
- **ClientId / ClientSecret** — credentials for token exchange  
- **CallbackUrl** — endpoint that receives the authorization code  
- **Disabled** — allows bypassing OIDC in development/testing  

See: [OIDC Settings](ca://s?q=Explain_identity_oidc_settings)

### Session Settings

- **TTL** — session lifetime  
- **Storage** — EF Core persistence  
- **Revocation** — logout invalidates session  

See: [Session Management](ca://s?q=Explain_identity_session_management)

---

## Session Management

Sessions are:

- stored in the database  
- associated with a user ID  
- timestamped with creation time  
- validated on every request  
- revocable (logout)  
- expired automatically based on TTL  

Session management is implemented in:

- **Application layer** — creation, revocation  
- **EF Core layer** — persistence  
- **Infrastructure layer** — validation  
- **Cross‑cutting middleware** — request‑level enforcement  

---

## Request Authentication

Every request is authenticated through the cross‑cutting `ICurrentUser` abstraction:

```csharp
[FromServices] ICurrentUser currentUser
```

`ICurrentUser` resolves:

- session token from headers/cookies  
- session validity  
- authenticated user ID  
- session metadata (creation time, expiration)  

This ensures:

- API endpoints never parse tokens directly  
- domain logic never touches authentication concerns  
- application logic receives a consistent identity model  

See: [Current User](ca://s?q=Explain_identity_context_access)

---

## Runtime Collaboration Points

Authentication interacts with:

- **Cross‑cutting middleware** — session validation, identity resolution  
- **Identity Infrastructure** — OIDC provider integration  
- **Identity Application** — token exchange, session creation  
- **Identity Domain** — user/session aggregates  
- **EF Core** — persistence  
- **Testing** — mutated contexts, fake providers, test sessions  

Authentication is a foundational cross‑cutting concern that shapes the entire request pipeline.

---

## Notes

Keep this document grounded in the actual authentication implementation.  
Whenever OIDC behavior, session rules, or identity resolution changes, update this section to reflect the current architecture.
