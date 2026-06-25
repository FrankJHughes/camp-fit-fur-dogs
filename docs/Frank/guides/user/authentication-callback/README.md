# Authentication Callback — User Guide

The Authentication Callback capability handles the **OIDC authorization‑code callback** after an owner logs in with an external identity provider (Auth0).  
It exchanges the authorization code for tokens, validates them, retrieves the user’s profile, and returns a normalized identity object that the rest of the system can trust.

This guide explains how to **use** the capability in your application.

---

## 1. What This Capability Does

When an owner completes login with Auth0, the identity provider redirects back to your application with a `code` parameter.

The Authentication Callback capability:

1. Accepts the authorization code  
2. Exchanges it for tokens  
3. Validates the ID token  
4. Fetches user profile information  
5. Produces a `FrankAuthCallbackResult`  
6. Hands that result to your identity resolution logic  

You do **not** need to handle any OIDC protocol details yourself — the capability does all of that for you.

---

## 2. When You Use This Capability

You use this capability when implementing the **OIDC login callback endpoint**, typically something like:

````text
GET /auth/callback?code=123
````

Your endpoint:

1. Extracts the `code`  
2. Passes it to the callback builder  
3. Receives a normalized identity result  
4. Resolves or creates the internal user identity  
5. Issues a session token  

---

## 3. How to Call the Authentication Callback Pipeline

Inject the builder:

````csharp
public class AuthCallbackHandler
{
    private readonly IImmutableContextBuilder<
        FrankAuthCallbackRequest,
        OidcAuthCallbackContext,
        FrankAuthCallbackResult> _builder;

    public AuthCallbackHandler(
        IImmutableContextBuilder<
            FrankAuthCallbackRequest,
            OidcAuthCallbackContext,
            FrankAuthCallbackResult> builder)
    {
        _builder = builder;
    }
}
````

Then call it:

````csharp
var result = await _builder.BuildAsync(
    new FrankAuthCallbackRequest { Code = code },
    cancellationToken);
````

If the callback succeeds, you receive a `FrankAuthCallbackResult`.

---

## 4. What You Get Back

A successful callback returns:

````csharp
public sealed record FrankAuthCallbackResult
{
    public string SubjectId { get; init; }
    public IReadOnlyDictionary<string, string> Claims { get; init; }

    public string? Email { get; init; }
    public string? GivenName { get; init; }
    public string? FamilyName { get; init; }
    public string? Picture { get; init; }

    public string Provider { get; init; } = "unknown";
}
````

### Key fields

- **SubjectId** — the stable provider‑assigned user identifier  
- **Claims** — all string claims extracted from ID token or UserInfo  
- **Email / GivenName / FamilyName / Picture** — common profile fields  
- **Provider** — `"auth0"` for this implementation  

This object is safe to use for identity resolution and session creation.

---

## 5. What You Do Next

Once you have a `FrankAuthCallbackResult`, you typically:

1. Pass it to your **identity resolver**:

   ````csharp
   var userId = await _identityResolver.ResolveAsync(result, ct);
   ````

2. Issue a session token for the resolved user  
3. Redirect the owner to the application  

The Authentication Callback capability does **not**:

- create users  
- issue tokens  
- manage sessions  

Those responsibilities belong to other capabilities.

---

## 6. Configuration Requirements

The capability reads its settings from:

````text
Authentication:Callback:Oidc
````

You must configure:

- `Authority`  
- `ClientId`  
- `ClientSecret`  
- `CallbackUrl`  

These must match your Auth0 application settings.

---

## 7. What You Don’t Need to Worry About

The capability handles:

- OIDC protocol correctness  
- Token exchange  
- Token validation  
- JWKS retrieval  
- UserInfo retrieval  
- Claim extraction  
- Immutable context safety  
- Error handling  
- Diagnostic events  

You do **not** need to:

- Parse JWTs  
- Validate signatures  
- Handle JWKS rotation  
- Call `/userinfo`  
- Manage protocol errors  

All of that is done for you.

---

## 8. Common Failure Cases

The pipeline will throw an exception if:

- The authorization code is invalid  
- The token endpoint rejects the request  
- The ID token is invalid or expired  
- JWKS cannot be retrieved  
- UserInfo cannot be fetched  
- The pipeline completes without a `SubjectId`  

Your callback endpoint should catch these and return an appropriate HTTP error.

---

## 9. Summary

The Authentication Callback capability gives you:

- A complete OIDC callback pipeline  
- A normalized identity result  
- Strong validation and safety guarantees  
- A simple API surface  
- No need to handle OIDC protocol details  

As a user of this capability:

- You provide the authorization code  
- You receive a validated identity  
- You resolve the internal user  
- You issue a session token  

Everything else is handled for you.
