# authentication-architecture.md

# Authentication Architecture

CampFitFurDogs uses **exclusive OIDC authentication**.  
Authentication is split into **three layers**, each with strict boundaries:

1. **Frank Protocol Pipeline** — handles OIDC protocol mechanics  
2. **Application Authentication Pipeline** — resolves identity, onboarding, and session creation  
3. **Api Endpoint** — thin orchestration boundary  

This separation ensures clarity, testability, and security.

---

## Authentication Flow Overview

1. The Api endpoint receives the OIDC callback.  
2. The request is passed to the **Frank protocol pipeline**, which:
   - exchanges the authorization code  
   - retrieves tokens  
   - validates tokens  
   - extracts claims  
   - normalizes provider output  

3. The resulting protocol context is passed to the **Application pipeline**, which:
   - resolves the owner identity  
   - performs onboarding if needed  
   - creates a session  
   - determines the redirect target  

4. The Api endpoint returns the final redirect.

Each layer has a **single responsibility** and must not leak concerns into the others.

---

## Frank Protocol Pipeline (Summary)

Frank handles:

- authorization‑code exchange  
- token retrieval  
- token validation  
- claims extraction  
- provider normalization  
- required‑claim enforcement  

Frank must not:

- perform business logic  
- perform persistence  
- determine redirects  
- create sessions  

Frank is the **protocol layer only**.

---

## Application Authentication Pipeline

The Application layer handles **business‑level authentication behavior**.

It uses an immutable context builder:

````markdown
IImmutableContextBuilder<
    ApplicationAuthCallbackRequest,
    ApplicationAuthCallbackContext,
    ApplicationAuthCallbackContextBuilderResult>
````
  
### Responsibilities

- identity resolution  
- owner onboarding  
- session creation  
- redirect selection  

### Prohibitions

The Application pipeline must not:

- perform OIDC protocol logic  
- perform HTTP calls  
- validate tokens  
- parse provider responses  
- embed business rules outside identity/session concerns  

The Application pipeline is the **business authentication layer**, not the protocol layer.

---

## Api Authentication Endpoint

The Api endpoint is intentionally thin.

### Responsibilities

- receive the callback  
- bind the request  
- invoke the Application pipeline  
- return the redirect  

### Prohibitions

The Api endpoint must not:

- contain protocol logic  
- contain business logic  
- perform persistence  
- orchestrate onboarding  
- validate tokens  
- parse claims  

The Api endpoint is the **delivery boundary**, not the decision‑maker.

---

## Enforcement

Authentication boundaries are enforced through:

- guardrail tests  
- dependency analysis  
- code review  
- conventions governance  

Authentication is a **multi‑layered pipeline**, and each layer must remain pure.
