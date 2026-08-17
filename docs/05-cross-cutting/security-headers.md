# Frank.CrossCutting — Security Headers

Security headers harden all HTTP responses against common web attacks. They are applied globally through platform middleware and require **no per‑endpoint configuration**. This ensures consistent protection across all vertical slices.

This document describes the security header subsystem under:

```
/docs/05-cross-cutting
```

and maps it back to its implementation under:

```
/src/Frank/Core.Api/Middleware/SecurityHeaders
```

Security headers integrate with:

- [CORS](ca://s?q=Explain_crosscutting_cors)  
- [Rate Limiting](ca://s?q=Explain_crosscutting_rate_limiting)  
- [Observability](ca://s?q=Generate_crosscutting_observability_doc)  
- [Authentication](ca://s?q=Explain_crosscutting_authentication)

---

## Headers Applied

The `SecurityHeadersMiddleware` adds hardened defaults recommended by OWASP and modern browser security standards:

- **`X-Content-Type-Options: nosniff`** — prevents MIME type sniffing  
- **`X-Frame-Options: DENY`** — blocks clickjacking  
- **`X-XSS-Protection: 0`** — disables legacy, unsafe XSS filters  
- **`Referrer-Policy: strict-origin-when-cross-origin`** — limits referrer leakage  
- **`Permissions-Policy`** — disables sensitive browser APIs (geolocation, camera, microphone, etc.)  
- **`Cross-Origin-Opener-Policy: same-origin`** — isolates browsing context  
- **`Cross-Origin-Embedder-Policy: require-corp`** — enforces CORP for embedded resources  
- **`Cross-Origin-Resource-Policy: same-origin`** — restricts cross-origin resource loading  
- **`Content-Security-Policy`** — strict modern baseline preventing inline scripts and unauthorized external resources  

These headers significantly reduce attack surface for:

- XSS  
- clickjacking  
- cross-origin attacks  
- resource embedding attacks  
- browser API misuse  

See:  
- [Security Middleware](ca://s?q=Explain_identity_security_middleware_tests)

---

## Content Security Policy (CSP)

The platform applies a strict CSP baseline:

```
default-src 'self';
script-src 'self';
style-src 'self';
img-src 'self' data:;
font-src 'self';
```

### What this prevents

- **inline scripts**  
- **external script loading**  
- **unauthorized cross-origin requests**  
- **third‑party resource injection**  
- **malicious browser extensions exploiting unsafe defaults**

CSP is one of the strongest protections against XSS and script injection.

See:  
- [CSP Details](ca://s?q=Explain_content_security_policy_baseline)

---

## When Is This Applied?

Security headers are applied during platform startup:

```csharp
app.UseSecurityHeaders();
```

This ensures:

- **all responses** include hardened headers  
- **no endpoint‑level configuration** is required  
- **middleware ordering** guarantees headers are applied after routing but before response writing  
- **testing environments** receive identical protections unless explicitly disabled  

Security headers are part of the platform’s default middleware stack.

See:  
- [Pipeline Behaviors](ca://s?q=Generate_crosscutting_pipeline_behaviors_doc)

---

## Runtime Collaboration Points

Security headers interact with:

- **CORS** — combined protection against cross-origin attacks  
- **Authentication** — secure handling of session cookies  
- **Rate Limiting** — protection against brute-force and abuse  
- **Logging** — structured logging of security events  
- **Observability** — correlation IDs for debugging security issues  
- **Testing** — validated through endpoint tests and mutated contexts  

Security headers form the foundation of the platform’s security posture.

---

## Notes

Keep this document grounded in the actual middleware implementation.  
Whenever new headers, CSP rules, or browser security standards evolve, update this section to reflect the current architecture.
