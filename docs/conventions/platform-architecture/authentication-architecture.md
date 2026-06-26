# Authentication Architecture Conventions

CampFitFurDogs uses **exclusive OIDC authentication** implemented as a **strictly layered, immutable‑pipeline architecture**.  
Authentication is divided into **three architectural layers**, each with a single responsibility and non‑overlapping boundaries:

1. **Frank Protocol Pipeline** — OIDC protocol mechanics  
2. **Application Authentication Pipeline** — identity, onboarding, and session creation  
3. **API Endpoint** — delivery boundary  

These layers must remain **pure, isolated, and independently testable**.

---

## Authentication Flow Overview

1. The API endpoint receives the OIDC callback request.  
2. The request is passed to the **Frank Protocol Pipeline**, which:
   - exchanges the authorization code  
   - retrieves and validates tokens  
   - extracts claims  
   - normalizes provider output  
   - produces a **protocol context**  

3. The protocol context is passed to the **Application Authentication Pipeline**, which:
   - resolves the owner identity  
   - performs onboarding if required  
   - creates a session  
   - determines the redirect target  
   - produces the **final authentication result**  

4. The API endpoint returns the redirect to the client.

Each layer has a **single responsibility** and must not leak concerns into the others.

---

## Frank Protocol Pipeline (Architecture Conventions)

Frank is the **protocol layer**.  
It uses the immutable context builder pattern to ensure deterministic execution and step‑level observability.

### Responsibilities

- Authorization‑code exchange  
- Token retrieval  
- Token validation  
- Claims extraction  
- Provider normalization  
- Required‑claim enforcement  

### Prohibitions

Frank must not:

- perform business logic  
- perform persistence  
- determine redirects  
- create sessions  
- resolve owners  
- perform onboarding  

Frank produces a **pure protocol context** only.

---

## Application Authentication Pipeline (Architecture Conventions)

The Application layer handles **business‑level authentication behavior**.

It is implemented using:

`IImmutableContextBuilder<ApplicationAuthCallbackRequest, ApplicationAuthCallbackContext, ApplicationAuthCallbackContextBuilderResult>`

This pipeline uses:

- immutable context transitions  
- deterministic step execution  
- step‑level observability  

### Responsibilities

- Identity resolution  
- Owner onboarding  
- Session creation  
- Redirect selection  
- Producing the final authentication result  

### Prohibitions

The Application pipeline must not:

- perform OIDC protocol logic  
- perform HTTP calls  
- validate tokens  
- parse provider responses  
- embed business rules outside identity/session concerns  

The Application pipeline is the **business authentication layer**, not the protocol layer.

---

## API Authentication Endpoint (Architecture Conventions)

The API endpoint is intentionally thin and non‑authoritative.

### Responsibilities

- Receive the callback  
- Bind the request  
- Invoke the Application pipeline  
- Return the redirect  

### Prohibitions

The API endpoint must not:

- contain protocol logic  
- contain business logic  
- perform persistence  
- orchestrate onboarding  
- validate tokens  
- parse claims  
- construct or mutate contexts  

The API endpoint is the **delivery boundary**, not the decision‑maker.

---

## Enforcement (Architecture Conventions)

Authentication boundaries are enforced through:

- architectural guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Authentication is a **multi‑layered, immutable pipeline**, and each layer must remain pure.
