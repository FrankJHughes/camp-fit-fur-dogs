# Security Header Conventions (Frank)

Frank provides middleware that applies standard HTTP security headers to all
responses. Products must not implement their own security header middleware.

---

## Purpose

Security headers protect against:

- clickjacking  
- MIME sniffing  
- cross‑site scripting  
- insecure transport  

Frank ensures consistent, product‑agnostic security posture.

---

## Required Headers

Frank must apply:

- `X-Content-Type-Options: nosniff`  
- `X-Frame-Options: DENY`  
- `Referrer-Policy: no-referrer`  
- `X-XSS-Protection: 0`  
- `Cross-Origin-Opener-Policy: same-origin`  
- `Cross-Origin-Embedder-Policy: require-corp`  
- `Cross-Origin-Resource-Policy: same-origin`  

Products must not override or remove these headers.

---

## Prohibitions

Frank must not:

- apply environment‑specific variations  
- expose configuration knobs for disabling headers  
- allow products to modify header values  

Products must not:

- add duplicate security headers  
- disable Frank’s middleware  
