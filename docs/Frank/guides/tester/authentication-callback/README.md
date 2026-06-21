# Authentication Callback — Tester Guide

The Authentication Callback capability implements the **OIDC authorization‑code callback pipeline**.  
Testers validate that the pipeline:

- exchanges the authorization code correctly  
- validates tokens cryptographically  
- fetches userinfo reliably  
- enforces immutability rules  
- produces a correct `FrankAuthCallbackResult`  
- fails safely and predictably on protocol violations  

This guide describes how to test the capability end‑to‑end.

---

# 1. What Testers Validate

Testers must ensure:

- The pipeline executes steps in the correct order  
- Each step runs only when its `CanExecute` condition is satisfied  
- Immutable fields (`Code`, `Now`) are never modified  
- Tokens are exchanged correctly  
- ID tokens are validated cryptographically  
- UserInfo is fetched correctly  
- Claims and profile fields are extracted correctly  
- The pipeline produces a valid `FrankAuthCallbackResult`  
- The pipeline throws the correct exceptions on failure  
- No identity provider tokens are persisted  
- No domain logic leaks into the pipeline  

---

# 2. Required Test Types

## 2.1 Pipeline Ordering Tests

Validate that steps run in this order:

1. `ExchangeCodeStep`  
2. `ValidateTokensStep`  
3. `FetchUserInfoStep`  

### What to test

- Steps execute in the correct order  
- Steps skip themselves when `CanExecute` returns false  
- No step runs twice  
- No step runs out of order  

---

## 2.2 Immutability Tests

The context is immutable. Testers must verify:

- Steps return a **new** context instance  
- `Code` never changes  
- `Now` never changes  
- Any attempt to modify immutable fields results in an exception  

The builder enforces this via:

```
AssertValidTransition()
```

### What to test

- A step returning `null` → throws  
- A step modifying `Code` → throws  
- A step modifying `Now` → throws  

---

## 2.3 Exchange Code Tests

`ExchangeCodeStep` must:

- POST to `/oauth/token`  
- Include correct payload  
- Extract `access_token` and `id_token`  
- Throw `OidcProtocolException` on failure  

### Test scenarios

- Valid code → returns tokens  
- Invalid code → throws  
- Missing `access_token` → throws  
- Missing `id_token` → allowed (userinfo may still provide subject)  

---

## 2.4 Validate Tokens Tests

`ValidateTokensStep` must:

- Download JWKS  
- Validate:
  - issuer  
  - audience  
  - signature  
  - lifetime  
- Extract subject and string claims  

### Test scenarios

- Valid ID token → subject extracted  
- Invalid signature → throws  
- Wrong issuer → throws  
- Wrong audience → throws  
- Expired token → throws  
- Missing `sub` claim → throws  

---

## 2.5 Fetch UserInfo Tests

`FetchUserInfoStep` must:

- GET `/userinfo`  
- Use Bearer token  
- Extract:
  - subject  
  - claims  
  - email  
  - given/family name  
  - picture  

### Test scenarios

- Valid access token → userinfo returned  
- Invalid access token → throws  
- Missing optional fields → pipeline still succeeds  
- Claims dictionary contains all string claims  

---

## 2.6 Final Result Tests

The builder must produce a valid `FrankAuthCallbackResult`.

### What to test

- `SubjectId` is required  
- Claims dictionary is populated  
- Profile fields are mapped correctly  
- Provider is `"auth0"`  
- Missing `SubjectId` → throws  

---

# 3. Error Handling Tests

Testers must validate that:

- Protocol errors throw `OidcProtocolException`  
- Unexpected errors surface as `InvalidOperationException`  
- No step swallows exceptions  
- The pipeline stops on the first failure  
- Error messages do not leak sensitive information  

### Examples

- Token endpoint returns 500 → throws  
- JWKS endpoint unreachable → throws  
- UserInfo endpoint returns 401 → throws  
- Step returns null → throws  

---

# 4. Test Isolation Requirements

- Use fake HTTP handlers (no real network calls)  
- Use deterministic timestamps for `Now`  
- Use fake JWKS documents  
- Use fake ID tokens  
- Use fake UserInfo responses  
- Do not rely on external identity providers  
- Do not persist anything  

---

# 5. Recommended Testing Patterns

## 5.1 Fake HTTP Handlers

Simulate:

- token exchange responses  
- JWKS responses  
- userinfo responses  

## 5.2 Fake Steps

Validate:

- ordering  
- immutability  
- exception propagation  

## 5.3 Context Snapshots

Capture:

- before context  
- after context  

Ensure:

- immutable fields unchanged  
- new fields added correctly  

## 5.4 Full Pipeline Tests

Simulate:

- valid end‑to‑end flow  
- missing tokens  
- invalid tokens  
- missing userinfo  
- partial userinfo  
- protocol failures  

---

# 6. Anti‑Patterns (Tests Must Reject)

- Tests that rely on real Auth0 endpoints  
- Tests that allow mutation of immutable fields  
- Tests that allow steps to run out of order  
- Tests that swallow exceptions  
- Tests that assume domain logic belongs in the pipeline  
- Tests that persist identity provider tokens  
- Tests that assume claims always exist  

---

# 7. Summary

Testers ensure that the Authentication Callback pipeline:

- runs deterministically  
- enforces immutability  
- validates tokens correctly  
- fetches userinfo correctly  
- produces a correct `FrankAuthCallbackResult`  
- fails safely on protocol violations  
- never leaks identity provider tokens  
- remains pure and side‑effect free  

This Tester Guide covers everything needed to validate the Authentication Callback capability end‑to‑end.
