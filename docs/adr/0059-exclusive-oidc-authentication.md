# ADR‑XXX — Removal of Local Identity & Exclusive OIDC Authentication

## Status
Accepted

## Context
The system previously supported two authentication paths:

1. Local identity (username/password, local registration, password reset)
2. External identity via OIDC (OpenID Connect)

Maintaining both paths introduced significant architectural and operational issues:

- Ambiguous identity authority
- Increased attack surface (password storage, reset flows, brute‑force protection)
- Duplicated logic for registration, login, lockout, and onboarding
- Divergent identity lifecycles between local and OIDC users
- Additional compliance and security burden
- Higher operational cost and maintenance complexity

The product direction requires a single, authoritative identity source with a deterministic onboarding flow. OIDC already fulfills this role. Local identity does not provide business value and introduces unnecessary risk.

Additionally, the OIDC callback pipeline required hardening to ensure strict validation of tokens, issuers, audiences, and claims.

## Decision

### 1. Remove all local identity functionality
The following features are removed:

- Local registration
- Local login
- Local password reset
- Local credential storage
- Local password policies
- Local identity lifecycle management
- Local authentication UI

All related endpoints, configuration keys, and UI elements are removed.

### 2. Enforce OIDC as the exclusive authentication mechanism
The system now:

- Requires a configured OIDC provider
- Rejects all non‑OIDC authentication attempts
- Treats OIDC claims as the authoritative identity source
- Uses OIDC login as the only entry point into the system

### 3. Harden the OIDC callback pipeline
The callback engine now performs strict validation:

- `state`
- `nonce`
- issuer
- audience
- signature
- required claims
- tenant trust
- token completeness

The callback engine is now a pure protocol component, isolated from domain logic.

### 4. Treat Owner creation as an onboarding step
The new flow:

1. User authenticates via OIDC
2. Token is validated
3. System checks for an existing Owner
4. If none exists, an Owner is created using OIDC claims
5. No passwords, no local verification, no local identity lifecycle

### 5. Simplify configuration
Removed:

- All local identity configuration keys
- Password policy configuration
- Local login UI configuration

Added:

- `Oidc:Disabled` flag for development scenarios
- Strict required OIDC configuration keys

## Consequences

### Positive
- Reduced attack surface
- Unified identity model
- Deterministic onboarding
- Cleaner architecture
- Lower operational cost
- Improved compliance posture
- Stronger authentication guarantees

### Negative
- No local login fallback
- Development environments must configure OIDC or explicitly disable it
- Migration required for any legacy local accounts

### Neutral
- Email flows related to local identity (verification, password reset) are removed
- Documentation and story files must be updated accordingly
