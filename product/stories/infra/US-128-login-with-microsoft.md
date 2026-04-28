---
id: US-128
title: "Login with Microsoft"
epic: Infrastructure
milestone: M1+
status: backlog
domain: infra
vertical_slice: true
dependencies:
  - US-110
  - US-111
---

# US-128: Login with Microsoft

## Intent

As an **owner**, I want to log in with my Microsoft account so that I can access
Camp Fit Fur Dogs without creating and managing a separate password.

## Value

Microsoft is the natural first provider for an ASP.NET Core application. This
story carries the multi-provider infrastructure — the login page shell, provider
abstraction, account linking logic, and first-login provisioning flow. Subsequent
provider stories (Google, Apple, Amazon) layer on incrementally.

## Acceptance Criteria

### Multi-provider infrastructure (ships with this story)
- [ ] Login page displays available identity provider options as branded buttons alongside the email/password form
- [ ] Provider abstraction supports adding new OIDC providers via configuration
- [ ] Social login with a new provider links the external identity to an existing account if the email matches
- [ ] Social login with an unrecognized email auto-provisions a customer account from OIDC claims (name, email)
- [ ] Login page is accessible (keyboard-navigable, screen-reader-friendly)

### Microsoft provider
- [ ] "Login with Microsoft" button initiates the Microsoft OIDC flow
- [ ] Successful Microsoft login returns an authenticated session (US-111)
- [ ] Failed Microsoft login displays a helpful, blame-free error message
- [ ] Microsoft client ID and secret are stored in configuration, not source code

## Emotional Guarantees

- **EG-01 No Surprises** — The login page clearly shows all available options; no unexpected redirects
- **EG-02 No Blame** — Failed login messages guide, never scold
- **EG-03 Calm Protection** — No provider tokens are stored; owners understand their data is safe

## Design Seam: Hybrid Login Model

> Camp Fit Fur Dogs uses a hybrid login model: traditional email/password
> registration (US-126) as the baseline, plus social identity providers as
> additive convenience. The login page presents both options — the email/password
> form and social provider buttons separated by a visual divider ("Or log in
> with..."). Owners who registered with email/password can later link a social
> provider; owners who first logged in via a social provider can later add a
> password.

## Notes

- This is the largest provider story — carries the multi-provider infrastructure
- Depends on US-110 (OIDC middleware) and US-111 (session management)
- Microsoft provider uses `AddMicrosoftAccount()` or `AddOpenIdConnect()` with Microsoft Entra configuration
- **Demo:** Click "Login with Microsoft" on the login page, authenticate with Microsoft, land on the authenticated home page with your name displayed
