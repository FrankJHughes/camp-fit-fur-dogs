# CORS Architecture  
*How Cross‑Origin Resource Sharing is enforced in CampFitFurDogs and Frank.*

CORS (Cross‑Origin Resource Sharing) defines which origins are allowed to make browser‑initiated requests to the API.  
It is a **security boundary**, not a convenience feature.  
This guide explains how CORS is configured, enforced, and validated across the system.

For governance rules, see **Security Governance → CORS Policy**.  
This document focuses on **developer understanding**, not policy enforcement.

---

# 1. Purpose of CORS in This Architecture

CORS protects the API from:

- Unauthorized browser‑initiated requests  
- Malicious third‑party websites  
- Accidental cross‑origin leaks  
- Unsafe credential sharing  
- Unintended exposure of authenticated endpoints  

CORS is enforced **only at the API boundary** and is implemented using **Frank’s CORS seams**.

The frontend and API are hosted on different domains in production, so CORS is required for:

- Local development  
- PR preview environments  
- Production hosting  
- API → Frontend redirects during authentication  

---

# 2. CORS Responsibilities

CORS enforcement is split across two layers:

## **2.1 Frank (Cross‑Cutting Layer)**  
Frank provides:

- The CORS policy seam  
- The middleware that applies the policy  
- The configuration abstraction  
- The hosting‑provider integration  
- Startup validation to ensure CORS is not misconfigured  

Frank does **not** define the allowed origins — that is product‑specific.

## **2.2 CampFitFurDogs API (Product Layer)**  
The API defines:

- Which origins are allowed  
- Whether credentials are allowed  
- Which headers are allowed  
- Which methods are allowed  

The API must **not** bypass Frank’s CORS middleware.

---

# 3. CORS in Local Development

Local development uses:

```
http://localhost:3000   (frontend)
http://localhost:5000   (api)
```

CORS must allow:

- `localhost:3000` → API  
- Credentialed requests (cookies)  
- Standard headers (`Content-Type`, `Accept`)  

Local development CORS is intentionally permissive **only for localhost**.

---

# 4. CORS in PR Preview Environments

PR previews use dynamic URLs:

- Vercel frontend: `https://pr-###.campfitdogs.dev`  
- Render API: `https://pr-###-api.campfitdogs.dev`  

CORS must allow:

- The PR’s frontend origin  
- Only that origin  
- Credentialed requests  
- No wildcard origins  

Frank’s hosting provider infrastructure injects the allowed origin at startup using:

- Render PR metadata  
- GitHub artifact URLs  
- Environment variables  

If the origin cannot be determined, **startup must fail**.

---

# 5. CORS in Production

Production uses:

- Frontend: `https://app.campfitdogs.com`  
- API: `https://api.campfitdogs.com`

CORS must allow:

- Only the production frontend origin  
- Credentialed requests  
- No wildcard origins  
- No dynamic origins  
- No fallback behavior  

Production CORS must be **strict**.

---

# 6. CORS Configuration Flow

CORS configuration flows through three layers:

```
Hosting Provider → Frank CORS Seam → API Startup Module
```

### **6.1 Hosting Provider**
Determines the allowed origin based on:

- Environment variables  
- PR preview metadata  
- Deployment environment  

### **6.2 Frank CORS Seam**
Applies:

- Allowed origins  
- Allowed methods  
- Allowed headers  
- Credential rules  

### **6.3 API Startup Module**
Registers:

- The CORS policy name  
- The middleware ordering  
- The integration with security headers  

---

# 7. Middleware Ordering

CORS must run:

1. **Before** authentication  
2. **Before** session validation  
3. **Before** endpoint routing  
4. **After** security headers  

Correct ordering ensures:

- Cookies are not blocked  
- Preflight requests succeed  
- Redirects behave correctly  
- Authentication flows work  

---

# 8. Credentialed Requests

CampFitFurDogs uses **session cookies**, so CORS must allow:

- `AllowCredentials = true`  
- No wildcard origins  
- No wildcard headers  
- No wildcard methods  

Browsers block credentialed requests unless:

- The origin is explicit  
- The headers are explicit  
- The methods are explicit  

---

# 9. Preflight Requests

Browsers send `OPTIONS` preflight requests for:

- Credentialed requests  
- Non‑simple headers  
- Non‑simple methods  

The API must:

- Respond with `200`  
- Include the correct CORS headers  
- Not require authentication for OPTIONS  

Frank’s CORS middleware handles this automatically.

---

# 10. Testing CORS

CORS must be tested in:

## **10.1 Integration Tests**
Validate:

- Allowed origins succeed  
- Disallowed origins fail  
- Credentialed requests include cookies  
- Preflight requests succeed  

## **10.2 PR Preview Tests**
Validate:

- Dynamic origins are correctly injected  
- No wildcard origins exist  
- Startup fails if origin is missing  

## **10.3 Production Tests**
Validate:

- Only the production frontend origin is allowed  
- No fallback origins exist  

---

# 11. Common Failure Modes

CORS failures usually appear as:

- Missing cookies  
- Failed preflight requests  
- 401s caused by blocked credentials  
- Redirect loops during authentication  
- “CORS error” in browser console  

Most failures are caused by:

- Incorrect origin  
- Missing credentials flag  
- Incorrect middleware ordering  
- Missing preflight handling  

---

# 12. Summary

CORS is a **security boundary**, not a convenience feature.  
It ensures that only trusted browser origins can communicate with the API.

Frank provides:

- The CORS seam  
- The middleware  
- The hosting provider integration  
- Startup validation  

CampFitFurDogs provides:

- The allowed origins  
- The product‑specific configuration  

CORS must be:

- Strict in production  
- Dynamic in PR previews  
- Safe in local development  
- Deterministic everywhere  

For enforceable rules, see:

```
docs/governance/technical/security-governance.md
```
