---
id: US-126
title: "Create Account Page"
epic: Customer
milestone: M1+
status: backlog
domain: customer
vertical_slice: true
dependencies:
  - US-027
  - US-049
  - US-056
  - US-102
  - US-103
---

# US-126: Create Account Page

## Intent

As an **owner**, I must be able to create an account by filling out a form with my
personal information and choosing a password, so that I can manage my dogs and
book services.

## Value

This is the front door to Camp Fit Fur Dogs. Without a registration page, the
Create Customer Account capability (US-027) exists only as an API endpoint with
no user-facing entry point. This story also unblocks downstream stories that
assume a registration flow exists (US-033 Duplicate Account, US-034 Incomplete
Registration) and is a natural predecessor to the authentication chain
(US-110 through US-113).

## Acceptance Criteria

- [ ] Registration page accessible at a public route
- [ ] Form collects required personal information (fields match the CreateCustomer API contract)
- [ ] Form includes email and password fields for traditional account creation
- [ ] Password field enforces minimum strength requirements with real-time feedback
- [ ] Client-side validation with clear, accessible inline error messages (reuses US-035 validation patterns)
- [ ] Form submits to the existing CreateCustomer API endpoint via the typed API client (US-102)
- [ ] Password is hashed server-side using BCrypt (US-049)
- [ ] Success feedback confirms account creation and guides the owner to the next step
- [ ] Duplicate account attempt displays a helpful, blame-free message (seeds US-033)
- [ ] Validation errors and API errors are surfaced without exposing internal system concepts
- [ ] Component tests: render, validate, submit success, submit error states
- [ ] Integration test: form to API client to mock server for success and error paths

## Emotional Guarantees

- **EG-01 No Surprises** — Every field is clearly labeled; nothing unexpected happens on submit
- **EG-02 No Blame** — Error messages guide, never scold ("That email is already registered — would you like to sign in instead?")
- **EG-03 Calm Protection** — Personal information is handled respectfully; no data is exposed in the URL or browser history

## Design Seam: Social Login

> Camp Fit Fur Dogs uses a hybrid login model: traditional email/password
> registration (this story) as the baseline, plus social identity providers
> (Microsoft, Google, Apple, Amazon) as additive convenience (US-128 through
> US-131). The registration page should include a visual divider with social
> login buttons ("Or sign up with...") once social providers ship. Until then,
> email/password is the only path.

## Notes

- Follows the same vertical-slice pattern established by US-084 (Register Dog Page)
- Form fields must match the `CreateCustomerCommand` backend contract (US-027)
- Reuse the `FormField`, `FieldError`, and validation patterns from US-035
- US-049 (BCrypt) is a dependency — password hashing must be in place
- This story ships the happy path; edge-case stories (US-033, US-034) remain separate
- **Demo:** Navigate to the registration page, enter name/email/password, submit — see the account in the database
