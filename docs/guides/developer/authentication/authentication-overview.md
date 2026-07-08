# Authentication Overview

The application uses **OIDC‑based authentication** via an external identity provider (Auth0).  
Owners authenticate using Auth0’s hosted login page; **no passwords are stored locally**, **no identity provider tokens are persisted**, and **the backend manages all session state via a domain session cookie**.

Authentication is implemented as:

- A **pure login initiation endpoint** that returns the OIDC authorization URL (no domain logic, no persistence)  
- A **three‑layer authentication callback architecture** consisting of:  
  1. **Frank Auth Callback Pipeline** — OIDC protocol logic  
  2. **Application Auth Callback Pipeline** — identity + session  
  3. **API Callback Endpoint** — cookie issuance + redirect orchestration  
- A **logout endpoint** that deletes the session cookie and returns a post‑logout redirect target  
- An **identity endpoint** that exposes the current authenticated user via `ICurrentUser`

All authentication behavior follows **Architecture Governance**, Security Governance, and API Endpoint Purity.

---

# Flow Summary

## Login

1. Client calls **GET `/api/identity/login`** (optionally with `return_url`)  
2. API validates OIDC and frontend configuration (`Authority`, `ClientId`, `Frontend:BaseUrl`)  
3. API constructs the callback URL (from configuration or current request)  
4. API encodes `return_url` into OIDC `state` using `OidcStateEncoder`  
5. API builds the Auth0 authorization URL (`nextUrl`)  
6. API returns **`200 OK`** with `LoginResponse(nextUrl)`  
7. **Frontend performs the actual redirect** to Auth0 using `nextUrl`

## Callback

8. Auth0 redirects back to **GET `/api/identity/callback`** with `code` and `state`  
9. API callback endpoint:
   - extracts and decodes `state`  
   - validates and reads `return_url` from state  
   - extracts `code` (if missing, redirects to `return_url` without a session)  
10. API constructs `FrankAuthCallbackRequest` and runs the **Frank pipeline**  
11. API constructs `ApplicationAuthCallbackRequest` and runs the **Application pipeline**  
12. Application pipeline returns a result containing `CookieValue` (session)  
13. API issues the **real CFFD session cookie**:

    ```csharp
    http.Response.Cookies.Append(
        "session",
        appAuthCallbackResult.CookieValue,
        new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
    ```

14. API redirects the user to `return_url` (from decoded state)

If `CookieValue` is empty, the callback endpoint redirects to `return_url` **without** issuing a session cookie.

## Identity

15. Client calls **GET `/api/identity`**  
16. Endpoint requires authorization and uses `ICurrentUser`  
17. API returns `GetIdentityResponse` (currently `Name` only)

## Logout

18. Client calls **GET `/api/identity/logout`** (optionally with `return_url`)  
19. API deletes the **`session`** cookie:

    ```csharp
    http.Response.Cookies.Delete("session");
    ```

20. API determines `returnUrl`:
    - from query `return_url`, or  
    - from `Frontend:BaseUrl` configuration (validated)  
21. API returns **`200 OK`** with `LogoutResponse(returnUrl)`  
22. **Frontend performs the actual post‑logout redirect** using `returnUrl`

---

# Principles (Aligned With Exclusive OIDC Authentication)

- Authentication is **external** — Auth0 performs identity proof  
- Login initiation endpoint is **pure** — no domain logic, no persistence, no identity resolution; it only validates configuration and builds `nextUrl`  
- Callback endpoint is a **thin orchestrator**:
  - validates `state` and `return_url` shape  
  - runs Frank and Application pipelines  
  - issues the `session` cookie  
  - performs the final redirect  
- Protocol logic lives **only** in the Frank pipeline  
- Business logic (identity + session) lives **only** in the Application pipeline  
- Cookie issuance and redirect live **only** in the API callback endpoint  
- No identity provider tokens are stored or logged  
- Session cookie (`session`) is the only client‑side authentication state  
- Session cookie is:
  - `HttpOnly=true`  
  - `Secure=true` (non‑local)  
  - `SameSite=Strict`  
- Logout deletes the `session` cookie and returns a redirect target; frontend navigates  
- Identity is resolved exclusively through Frank’s `ICurrentUser` abstraction, populated by the authentication pipeline  
- No endpoint performs token exchange or protocol logic directly  
- Local identity and ASP.NET cookie authentication (`cffd.session`) are fully removed — **OIDC + `session` cookie is the only authentication mechanism**

These principles align with Architecture Governance, Security Governance, and Session Management Governance.

---

# See Also

- **Login Endpoint** — `/api/identity/login`  
- **Callback Endpoint** — `/api/identity/callback`  
- **Identity Endpoint** — `/api/identity`  
- **Logout Endpoint** — `/api/identity/logout`  
- **Authentication Architecture Guide**  
- **Authentication Callback Architecture**  
- **Identity Mapping Guide**  
- **Session Management Guide**
