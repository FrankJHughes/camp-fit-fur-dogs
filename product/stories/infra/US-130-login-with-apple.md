---
id: US-130
title: "Login with Apple"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-128
---

# US-130: Login with Apple

## Intent

As an **owner**, I want to log in with my Apple account so that I can access
Camp Fit Fur Dogs using the identity built into my iPhone, iPad, or Mac.

## Value

Apple Sign In is expected by iOS and macOS users and may be required for App
Store distribution in the future. Apple's privacy-forward identity model (email
relay, "Hide My Email") aligns with Camp Fit Fur Dogs' emotional guarantees.

## Acceptance Criteria

- [ ] "Login with Apple" button appears on the login page alongside existing providers
- [ ] Successful Apple login returns an authenticated session
- [ ] Failed Apple login displays a helpful, blame-free error message
- [ ] Handles Apple's private relay email — account linking works even when Apple hides the owner's real email
- [ ] Apple client ID, team ID, key ID, and private key are stored in configuration, not source code

## Emotional Guarantees

- **EG-01 No Surprises** — Apple login behaves identically to other providers
- **EG-03 Calm Protection** — Apple's privacy relay is respected; owners are never asked to reveal their real email

## Notes

- Lightweight story — infrastructure is already in place from US-128
- Apple Sign In requires server-side JWT client secret generation (unlike Microsoft/Google)
- Private relay email complicates account linking — design decision needed for how to match relay emails to accounts
- Apple Developer Program membership ($99/year) required
- **Demo:** Click "Login with Apple," authenticate with Apple, land on the authenticated home page
