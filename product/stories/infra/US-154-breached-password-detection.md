---
id: US-154
title: "Breached Password Detection"
epic: Infrastructure
milestone: M2+
status: backlog
domain: infra
vertical_slice: false
dependencies:
  - US-049
  - US-126
---

# US-154: Breached Password Detection

## Intent

As a **system operator**, I need the system to silently check passwords against
known breach databases so that owners are warned before using a compromised
password.

## Value

Credential stuffing is the most common attack vector. Owners reuse passwords
across sites — when one site is breached, attackers try the same credentials
everywhere. Checking passwords against the Have I Been Pwned (HIBP) database
during registration and password changes catches compromised passwords before
they become a liability. This is invisible security — no friction for owners
with strong passwords, gentle guidance for those at risk.

## Acceptance Criteria

- [ ] During registration (US-126) and password change, the entered password is checked against the HIBP Pwned Passwords API
- [ ] HIBP check uses the k-anonymity model (only the first 5 characters of the SHA-1 hash are sent — the full password never leaves the server)
- [ ] If the password appears in the breach database, a non-blocking warning is displayed: "This password has appeared in a data breach on another site. We recommend choosing a different password."
- [ ] The warning is advisory, not blocking — the owner can proceed if they choose (reduces friction)
- [ ] HIBP check failure (API timeout, network error) does not block registration — fail open with a log entry
- [ ] Password is never logged, cached, or stored in plaintext at any point in the check
- [ ] Unit test verifies k-anonymity hash prefix logic
- [ ] Integration test verifies HIBP API call and response parsing

## Emotional Guarantees

- **EG-02 No Blame** — The message says "this password appeared in a breach elsewhere" — never implies the owner did something wrong
- **EG-03 Calm Protection** — The check is silent for strong passwords; only surfaces when there's a genuine risk

## Notes

- HIBP Pwned Passwords API is free, open, and designed for exactly this use case
- k-anonymity: hash the password with SHA-1, send the first 5 hex chars, receive all matching suffixes, check locally — the full hash never leaves the server
- Consider: periodic batch check of existing password hashes against HIBP (requires stored hash comparison)
- Consider: should the warning be stronger ("We strongly recommend...") or softer ("You might want to...")?
- **Demo:** During registration, enter "password123" — see the breach warning. Enter a unique password — no warning.
