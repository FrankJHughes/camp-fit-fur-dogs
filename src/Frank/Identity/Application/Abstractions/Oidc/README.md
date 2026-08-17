# Identity Application — OIDC Protocol Abstractions

The **Oidc** folder contains all protocol‑layer abstractions required to interact
with an upstream OpenID Connect (OIDC) provider.  
These abstractions define a clean, deterministic, and testable interface for:

- Exchanging authorization codes for tokens  
- Validating ID tokens  
- Retrieving UserInfo profile data  
- Normalizing provider‑specific claims  

This folder contains **contracts and immutable data models only**.  
All HTTP, cryptographic, and provider‑specific behavior is implemented in higher
layers.

---

## Folder Structure

```
Oidc/
├── IOidcTokenClient.cs
├── OidcTokenResponse.cs
├── IOidcTokenValidator.cs
├── OidcTokenValidationResult.cs
├── IOidcUserInfoClient.cs
└── OidcUserInfo.cs
```

---

# IOidcTokenClient

Defines the contract for performing an authorization‑code exchange.

### Responsibilities

- Call the provider’s token endpoint  
- Submit the authorization code  
- Return access and ID tokens  
- Handle provider‑specific error responses  

### Output

`OidcTokenResponse` — a structured model containing:

- `AccessToken`  
- `IdToken` (optional)

---

# OidcTokenResponse

Represents the tokens returned by the provider’s token endpoint.

### Contains

- **AccessToken** — always present  
- **IdToken** — optional, depending on provider configuration  

### Purpose

Used by the OIDC callback pipeline to:

- Call the UserInfo endpoint  
- Validate identity claims  
- Construct the immutable OIDC context  

---

# IOidcTokenValidator

Defines the contract for validating an ID token.

### Responsibilities

- Verify signature  
- Validate issuer and audience  
- Check expiration and “not‑before”  
- Validate nonce  
- Extract and normalize claims  

### Output

`OidcTokenValidationResult` — containing:

- `SubjectId`  
- `Claims` (normalized dictionary)

---

# OidcTokenValidationResult

Represents the identity information extracted from a validated ID token.

### Contains

- **SubjectId** — the upstream identity’s unique identifier  
- **Claims** — normalized claim dictionary  

### Purpose

Used by the OIDC callback pipeline to:

- Build the immutable OIDC context  
- Map upstream identity into local application identity  

---

# IOidcUserInfoClient

Defines the contract for retrieving profile information from the UserInfo endpoint.

### Responsibilities

- Call the provider’s UserInfo endpoint  
- Use the access token for authorization  
- Return structured profile data  
- Handle provider‑specific errors  

### Output

`OidcUserInfo` — containing:

- Subject  
- Email  
- GivenName  
- FamilyName  
- Picture  
- Additional claims  

---

# OidcUserInfo

Represents the profile information returned by the UserInfo endpoint.

### Contains

- **Subject** — upstream identity’s unique identifier  
- **Email** — optional  
- **GivenName** — optional  
- **FamilyName** — optional  
- **Picture** — optional  
- **Claims** — additional provider‑specific attributes  

### Purpose

Used by the OIDC callback pipeline to enrich the normalized identity context.

---

# Protocol Pipeline Overview

```
[ Authorization Code ]
        ↓
IOidcTokenClient
        ↓
[ Access Token + ID Token ]
        ↓
IOidcTokenValidator
        ↓
[ Subject + Claims ]
        ↓
IOidcUserInfoClient
        ↓
[ Profile Information ]
        ↓
(Immutable OIDC Context Builder)
```

This structure ensures:

- Deterministic behavior  
- Provider‑agnostic abstractions  
- Full testability  
- Clean separation of protocol vs application concerns  

---

# Summary

The OIDC folder provides a complete protocol‑layer abstraction for interacting
with upstream identity providers:

### **IOidcTokenClient / OidcTokenResponse**
Token exchange.

### **IOidcTokenValidator / OidcTokenValidationResult**
ID token validation and claim extraction.

### **IOidcUserInfoClient / OidcUserInfo**
UserInfo retrieval and profile normalization.

Together, these abstractions form a clean, deterministic, and testable OIDC
protocol pipeline within the Identity subsystem.

---
