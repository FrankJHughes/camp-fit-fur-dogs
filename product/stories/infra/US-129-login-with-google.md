---
id: US-129
title: "Login with Google"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-128
---

# US-129: Login with Google

## Intent

As an **owner**, I want to log in with my Google account so that I can access
Camp Fit Fur Dogs using the account I already use every day.

## Value

Google is the most widely held consumer identity. Adding Google login maximizes
the number of owners who can sign in without friction. This story leverages the
multi-provider infrastructure shipped with US-128.

## Acceptance Criteria

- [ ] "Login with Google" button appears on the login page alongside existing providers
- [ ] Successful Google login returns an authenticated session
- [ ] Failed Google login displays a helpful, blame-free error message
- [ ] Account linking — Google email matching an existing account links the provider, does not create a duplicate
- [ ] Google client ID and secret are stored in configuration, not source code

## Emotional Guarantees

- **EG-01 No Surprises** — Google login behaves identically to other providers
- **EG-03 Calm Protection** — No Google tokens are stored; login is a handshake, not a data grab

## Notes

- Lightweight story — infrastructure is already in place from US-128
- Uses `AddGoogle()` with Google OAuth 2.0 configuration
- **Demo:** Click "Login with Google," authenticate with Google, land on the authenticated home page
