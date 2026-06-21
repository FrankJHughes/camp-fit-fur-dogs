# Security Headers Architecture  
*How HTTP security headers are applied, validated, and enforced across CampFitFurDogs and Frank.*

Security headers are a mandatory part of the API boundary.  
They protect customers from browser‑based attacks, enforce safe defaults, and ensure consistent security behavior across all environments.

This guide explains **how security headers work**, **how Frank enforces them**, and **how the API integrates with them**.

For enforceable rules, see:  
`docs/governance/technical/security-governance.md`

---

# 1. Purpose of Security Headers

Security headers protect against:

- Cross‑site scripting (XSS)  
- Clickjacking  
- MIME sniffing  
- Insecure transport  
- Unsafe framing  
- Mixed‑content attacks  
- Accidental exposure of sensitive resources  

They ensure:

- Browsers behave safely  
- Attack surfaces are minimized  
- The API cannot be embedded or misused by third‑party sites  
- HTTPS is enforced  
- Cookies are protected  

Security headers are **non‑negotiable** and must be applied to **every API response**.

---

# 2. Security Headers Provided by Frank

Frank provides a **single, centralized middleware** that applies all required headers.

Headers include:

## **2.1 X‑Content‑Type‑Options**
```
X-Content-Type-Options: nosniff
```
Prevents MIME‑type sniffing.

## **2.2 X‑Frame‑Options**
```
X-Frame-Options: DENY
```
Prevents clickjacking.

## **2.3 X‑XSS‑Protection**
```
X-XSS-Protection: 0
```
Disables legacy, unsafe browser filters.

## **2.4 Referrer‑Policy**
```
Referrer-Policy: strict-origin-when-cross-origin
```
Prevents leaking sensitive URLs.

## **2.5 Permissions‑Policy**
Restricts access to browser APIs (camera, geolocation, etc.).

## **2.6 Strict‑Transport‑Security (HSTS)**
```
Strict-Transport-Security: max-age=31536000; includeSubDomains; preload
```
Enforces HTTPS.

Applied only in production.

## **2.7 Content‑Security‑Policy (CSP)**
CSP is optional but recommended for future hardening.

Frank provides a seam for CSP injection.

---

# 3. Middleware Ordering

Security headers must run:

1. **Before** CORS  
2. **Before** authentication  
3. **Before** session validation  
4. **Before** endpoint routing  
5. **Before** error boundary shaping  

This ensures:

- All responses (including errors) are protected  
- Preflight requests receive headers  
- Authentication redirects are safe  
- Cookies are protected  

Frank’s middleware enforces correct ordering automatically.

---

# 4. Environment‑Specific Behavior

Security headers behave differently depending on environment.

## **4.1 Local Development**
- HSTS disabled  
- CSP disabled  
- All other headers enabled  

Local development must remain functional without HTTPS.

## **4.2 PR Preview Environments**
- HSTS enabled  
- CSP optional  
- All headers applied  

Preview environments must behave like production.

## **4.3 Production**
- All headers enabled  
- HSTS required  
- CSP recommended  
- No fallback behavior allowed  

Production must be fully hardened.

---

# 5. Integration with Hosting Providers

Security headers integrate with:

- **HostingEngine**  
- **StartupEngine**  
- **Render hosting provider**  
- **Vercel frontend hosting**  

Hosting providers must:

- Not override headers  
- Not disable headers  
- Not inject conflicting headers  
- Fail startup if security headers cannot be applied  

Frank enforces:

- Header presence  
- Header correctness  
- Header ordering  

---

# 6. Integration with API Endpoints

API endpoints must:

- Not modify security headers  
- Not remove security headers  
- Not override Frank’s middleware  
- Not add duplicate headers  

All endpoint responses — including:

- Success responses  
- Validation errors  
- Authentication redirects  
- Callback errors  
- Preflight responses  
- 404s  
- 500s  

must include the full security header set.

Frank’s middleware ensures this automatically.

---

# 7. Testing Security Headers

Security headers must be tested in:

## **7.1 Integration Tests**
Validate:

- Headers exist  
- Headers have correct values  
- Headers apply to all endpoints  
- Headers apply to error responses  

## **7.2 PR Preview Tests**
Validate:

- HSTS is enabled  
- No missing headers  
- No wildcard CSP (if CSP is enabled)  

## **7.3 Production Tests**
Validate:

- All headers are present  
- No fallback behavior  
- No environment‑specific weakening  

---

# 8. Common Failure Modes

Security header issues typically appear as:

- Browser refusing to load resources  
- Cookies not being sent  
- Mixed‑content warnings  
- Redirect loops during authentication  
- Missing headers in error responses  

Most failures are caused by:

- Incorrect middleware ordering  
- Custom middleware overriding headers  
- Hosting provider misconfiguration  
- Missing HTTPS in production  

Frank’s guardrails prevent most of these.

---

# 9. Extending Security Headers

Developers may extend headers by:

- Adding CSP directives  
- Adding new Permissions‑Policy rules  
- Adding additional hardening headers  

Extensions must:

- Be additive  
- Not weaken existing headers  
- Not conflict with hosting provider behavior  
- Not break authentication redirects  

All changes must be tested in:

- Local development  
- PR previews  
- Production  

---

# 10. Summary

Security headers are a critical part of the API boundary.  
They protect customers, enforce safe browser behavior, and ensure consistent security across all environments.

Frank provides:

- The middleware  
- The ordering  
- The validation  
- The hosting provider integration  

CampFitFurDogs provides:

- Environment‑specific configuration  
- Optional CSP extensions  

Security headers must be:

- Always enabled  
- Never overridden  
- Never removed  
- Never weakened  

For enforceable rules, see:

```
docs/governance/technical/security-governance.md
```
