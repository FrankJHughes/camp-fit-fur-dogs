# Immutable Context Builder Conventions (Frank)

Frank uses ImmutableContextBuilder for protocol‑level transformations such as
OIDC authentication flows. These builders must remain pure and side‑effect‑free.

---

## Structure

Each builder defines:

- `TRequest` — immutable input  
- `TContext` — immutable working context  
- `TResult` — immutable output  

All types must:

- be immutable  
- use required members  
- contain no behavior  
- contain no side effects  
- contain no Infrastructure or Api dependencies  

---

## Responsibilities

Frank builders must:

- handle protocol logic only  
- validate authorization code  
- validate token issuer, audience, signature  
- validate nonce and state  
- enforce required claims  
- normalize provider output  

Frank builders must not:

- perform business logic  
- access persistence  
- construct aggregates  
- issue cookies  
- compute redirect URLs  

They produce a **protocol‑normalized result** consumed by Application.
