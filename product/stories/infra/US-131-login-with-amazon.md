---
id: US-131
title: "Login with Amazon"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-128
---

# US-131: Login with Amazon

## Intent

As an **owner**, I want to log in with my Amazon account so that I can access
Camp Fit Fur Dogs using an identity I already trust with purchases and personal
information.

## Value

Amazon is a widely held consumer identity with high trust. Login with Amazon
expands the provider options for owners who may not use Microsoft, Google, or
Apple ecosystems.

## Acceptance Criteria

- [ ] "Login with Amazon" button appears on the login page alongside existing providers
- [ ] Successful Amazon login returns an authenticated session
- [ ] Failed Amazon login displays a helpful, blame-free error message
- [ ] Account linking — Amazon email matching an existing account links the provider, does not create a duplicate
- [ ] Amazon client ID and secret are stored in configuration, not source code

## Emotional Guarantees

- **EG-01 No Surprises** — Amazon login behaves identically to other providers
- **EG-03 Calm Protection** — No Amazon tokens are stored; login is a handshake, not a data grab

## Notes

- Lightweight story — infrastructure is already in place from US-128
- Login with Amazon uses OAuth 2.0 (not strict OIDC) — may need a custom handler or token endpoint call for profile data
- **Demo:** Click "Login with Amazon," authenticate with Amazon, land on the authenticated home page
