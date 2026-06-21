# Authentication Callback Conventions (CampFitFurDogs)

CampFitFurDogs owns the **business‑layer authentication pipeline**, which
consumes Frank’s OIDC protocol pipeline and produces:

- user identity resolution  
- session creation  
- redirect computation  
- cookie value computation  

The Api callback endpoint orchestrates these steps.

---

## Responsibilities

The Application callback builder must:

- resolve identity from validated OIDC claims  
- create or load Owners  
- create sessions  
- compute redirect URLs  
- compute opaque cookie values  

It must not:

- perform protocol logic  
- validate tokens  
- call external identity providers  
- issue cookies (Api does this)  
- write to HttpContext  

---

## Required Output

The callback builder must produce:

- `CustomerId`  
- `SessionId`  
- `TokenHash`  
- `CookieValue` (opaque session token)  
- `RedirectUrl`  

All fields must be set before returning.

---

## Endpoint Behavior

The callback endpoint must:

- extract `code` and `state`  
- throw BadRequest if missing  
- invoke Frank’s OIDC protocol builder  
- invoke Application callback builder  
- issue the session cookie  
- redirect to the computed URL  

The endpoint must not:

- perform protocol logic  
- perform business logic  
- compute redirect URLs  
- generate cookie values  
- construct aggregates  
- perform persistence  

---

## Cookie Rules

The callback endpoint must issue a cookie that is:

- opaque (no dots, no user data, not a JWT)  
- HttpOnly  
- Secure (preview/prod)  
- SameSite=Lax  
- Path=/  
- Max‑Age set  

Cookie name: `cfd.session`

---

## Prohibitions

The callback flow must not:

- leak token contents  
- expose IdP error payloads  
- depend on Infrastructure directly  
- depend on HttpContext inside builders  
