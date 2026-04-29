---
id: US-049
title: "Secure Password Hashing"
epic: ""
milestone: ""
status: shipped
domain: infra
urgency: ""
importance: ""
covey_quadrant: ""
vertical_slice: false
emotional_guarantees: ""
legal_guarantees: ""
---
# US-049 — Secure Password Hashing

## Intent

As a security-conscious team, we want to replace the placeholder base64 password encoding with a production-grade hashing algorithm so that customer credentials are protected at rest.

## Value

The current `PasswordHash` value object uses `Convert.ToBase64String` — this is encoding, not hashing. Base64 is trivially reversible. Before any authentication story ships, this must be replaced with BCrypt, Argon2, or PBKDF2 to meet minimum security standards.

## Acceptance Criteria

- [ ] `PasswordHash.Create()` uses BCrypt, Argon2, or PBKDF2 (not base64)
- [ ] Hashed values are not reversible to plaintext
- [ ] `PasswordHash.Verify(plaintext, hash)` method is added for future login use
- [ ] Existing `PasswordHash` unit tests are updated to validate the new behavior
- [ ] No plaintext or base64-encoded passwords exist in the database after migration
- [ ] The chosen algorithm and cost factor are documented in an ADR or inline comment

## Emotional Guarantees

- EG-03 Calm protection

