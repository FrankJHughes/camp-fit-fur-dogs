# Session Management Guide  
**Aligned With Exclusive OIDC Authentication & Auth Callback Refactor**

This guide explains how session management works **today**, based on the implementation completed for:

- US‑110 — Authentication: Owner Login  
- US‑111 — Session Management  

It documents the *runtime behavior* and *developer workflow* for issuing and persisting session cookies after a successful OIDC login, aligned with the **new authentication callback architecture**.

This guide does **not** define rules or governance — those live in:

- Architecture Governance  
- Security Governance  
- Operations Governance  
- Conventions  
- ADRs  

This guide focuses solely on **how the current session implementation behaves**.

---

# What Session Management Covers Today

The current system implements the **first half** of the session lifecycle:

- Session token **generation**  
- Token **hashing**  
- Cookie **value computation** (Application pipeline)  
- Cookie **issuance** (API layer)  
- Session **persistence** in the database  
- Session **association** with an Owner  
- Session **lookup** (internal only — middleware not yet implemented)  

Not yet implemented (future stories):

- Session **validation middleware**  
- Session **rotation**  
- Session **revocation**  
- Idle timeout enforcement  
- Absolute expiration enforcement  

This guide documents only what exists today.

---

# Session Flow Overview (Post‑Refactor)

1. User completes OIDC login with Auth0  
2. Auth0 redirects to `/api/identity/callback?code=XYZ&state=<encoded>`  
3. API decodes `state` → extracts `return_url`  
4. API extracts the authorization code  
5. **Frank Auth Callback Pipeline** runs (protocol):  
   - Validates configuration  
   - Exchanges the authorization code  
   - Normalizes userinfo  
   - Produces a stable external identity payload  

6. **Application Auth Callback Pipeline** runs (business):  
   - Validates identity claims  
   - Resolves or creates the Owner  
   - Logs the login event  
   - **Creates the session**  
   - **Generates the session token + hash**  
   - **Computes the cookie value**  

7. API layer:  
   - Issues the `session` cookie using `CookieValue`  
   - Redirects the user to `return_url` from decoded state  

8. Browser stores the cookie  
9. Future API requests include the cookie  
10. Session validation middleware (future) will consume it  

All of this behavior follows **Session Management Governance** and **API Endpoint Purity**.

---

# Session Cookie Format

The session cookie is an **opaque, server‑managed token**.  
It contains **no user data**, is **not a JWT**, and is **not readable** by the client.

Example (conceptual):

````text
Set-Cookie: session=2f9c3e2a...; HttpOnly; Secure; SameSite=Strict; Path=/;
````

### Cookie Properties (Actual Implementation)

| Property | Value | Purpose |
|---------|--------|---------|
| Name | `session` | Single authentication cookie |
| Value | 256‑bit random token | Prevents guessing/bruteforce |
| HttpOnly | `true` | Prevents JS access |
| Secure | `true` in preview/prod | Required for HTTPS |
| SameSite | `Strict` | Prevents CSRF; compatible with OIDC callback |
| Path | `/` | Sent to all API routes |

The cookie value is computed in the **Application pipeline** and written by the **API layer**.

Local development uses `Secure=false`.

---

# Session Token Generation

Session tokens are generated inside the **Application Auth Callback Pipeline** using a cryptographically secure token generator.

Each session token consists of:

- A **256‑bit random plaintext token**  
- A **SHA‑256 hash** of that token  

The pipeline returns:

````text
CookieValue  (plaintext token)
TokenHash    (SHA‑256 hash)
````

The **plaintext token** is sent to the browser as a cookie.  
The **hash** is stored in the database.

The plaintext token is **never persisted**.

This aligns with **Security Governance** and **Session Token Governance**.

---

# Session Database Record

Sessions are stored in the database using the domain `Session` entity.

| Column | Purpose |
|--------|---------|
| `Id` | Primary key |
| `TokenHash` | SHA‑256 hash of the session token |
| `CustomerId` | Internal owner identifier |
| `CreatedAt` | Audit timestamp |
| `ExpiresAt` | Absolute expiration |
| `LastUsedAt` | For idle timeout (future) |

Only the **hash** of the token is stored — never the raw token.

---

# Where Session Creation Happens (Post‑Refactor)

Session creation occurs inside the **Application Auth Callback Pipeline**.

### Inputs:

- Normalized identity from Frank  
- `CustomerId`  
- `Now`  
- `TokenHash`  

### Behavior:

1. Creates a new domain `Session`  
2. Persists it via `ISessionRepository`  
3. Commits via `IUnitOfWork`  
4. Returns:
   - `SessionId`
   - `TokenHash`
   - `CookieValue`

The Application pipeline does **not** issue cookies — that is the API layer’s responsibility.

---

# Where Cookie Issuance Happens

Cookie issuance happens in the **API callback endpoint**, not in the pipeline.

The API endpoint:

1. Receives `CookieValue` from the Application pipeline  
2. Applies cookie security flags  
3. Writes the cookie to the HTTP response  
4. Redirects the user to `return_url`  

This separation enforces:

- **API Endpoint Purity**  
- **Session Management Governance**  
- **Security Governance**  

---

# Session Validation (Future Work)

Session validation middleware is **not yet implemented**.  
It will be added in **US‑111**.

The middleware will:

- Read the `session` cookie  
- Hash the token  
- Look up the session by hash  
- Validate expiration  
- Attach the authenticated Owner to the request  

Until then:

- Sessions are created  
- Cookies are issued  
- No middleware consumes them  

---

# Local Development Notes

### Cookies in HTTP vs HTTPS

- Local dev uses `http://localhost:3000` (frontend) and `http://localhost:5000` (API)  
- Cookie is issued with `Secure=false` in local dev  
- Preview/prod enforce `Secure=true`  

### Auth0 Callback URLs

````text
http://localhost:5000/api/identity/callback
https://<preview>.onrender.com/api/identity/callback
https://campfitfurdogsapi.onrender.com/api/identity/callback
````

---

# Testing Session Behavior

Session behavior is tested in three layers:

---

## 1. Unit Tests  
- Token generation  
- Token hashing  
- Cookie value computation  
- Session creation  

---

## 2. Application Pipeline Tests  
- Identity resolution → session creation  
- Cookie value computation  

---

## 3. Integration Tests  
- Full callback flow  
- Cookie issuance  
- Session persistence  
- Redirect behavior  

---

## 4. Guardrail Tests  
- Cookie flags  
- No sensitive data in cookies  
- Token opacity  
- No JWTs  
- No Infrastructure leakage into Application  

Tests live in:

````text
tests/Api.Tests/Authentication
tests/Api.Tests/Guardrails
````

---

# Troubleshooting

### Cookie not appearing in browser  
- Check SameSite=Strict  
- Check domain mismatch  
- Check HTTPS requirement in preview/prod  

### Session not created  
- Ensure identity resolution succeeded  
- Check database connection  
- Check session repository DI registration  

### Callback returning 500  
- Usually caused by missing Auth0 secrets  
- Check `Authentication__Callback__Oidc__ClientId`  
- Check `Authentication__Callback__Oidc__ClientSecret`  
- Check `Authentication__Callback__Oidc__Authority`  

---

# Related Documents

- **[Identity Mapping Guide](ca://s?q=Show_identity_mapping_guide)**  
- **[Authentication Architecture Guide](ca://s?q=Show_authentication_architecture_doc)**  
- **[Authentication Testing Guide](ca://s?q=Show_authentication_testing_guide)**  
- **[Authentication Operations Guide](ca://s?q=Show_authentication_operations_guide)**  
- **[Create Account Form Guide](ca://s?q=Show_create_account_form_guide)**  
- **[Create Account Feature Slice Guide](ca://s?q=Show_create_account_feature_slice_guide)**
