---
id: US-084
title: "Register Dog Page"
epic: Customer
milestone: M1+
status: in-progress
sprint: S4
sprint_order: 6
domain: customer
vertical_slice: true
dependencies:
  - US-056
  - US-102
  - US-103
replaces: "Frontend Testing Setup (completed, moved to US-103)"
---

# US-084: Register Dog Page

## User Story

As a **customer**, I can register a new dog by filling out a form with my dog's details, so that the dog is added to my profile.

## Acceptance Criteria

- [ ] React form component with fields: dog name, breed, date of birth, sex
- [ ] Client-side validation (required fields, data types)
- [ ] Form submits to the existing RegisterDog API endpoint via the typed API client (US-102)
- [ ] Success feedback displayed after successful registration
- [ ] Error handling: validation errors, API errors, network errors displayed to user
- [x] ~~customerId passed as route parameter or prop~~ → **Dropped (April 15).** See *Design Seam* below.
- [ ] Component tests: render, validate, submit, error states
- [ ] Integration test: form → API client → mock server → success/error
- [ ] **Design Doc:** This slice is the living example — the code IS the documentation

## TDD Requirements

**Heavy — first full-stack frontend TDD.** Red-green-refactor at every layer:

- [ ] Component renders correctly
- [ ] Validation rejects invalid input
- [ ] Successful submission calls API and shows feedback
- [ ] Error states render correctly

## Design Seam: Customer Identity

> **Original AC (dropped):** "customerId passed as route parameter or prop (design seam for future auth)"
>
> **Why it was dropped (April 15):** A customer registers *their own* dog — their identity comes from their authenticated session, not from the client. Exposing an owner ID in the route or request body is a security risk (path/body manipulation → registering dogs under another customer). A staff-managed registration flow would be a separate story with its own authorization model.

### ICurrentUserService

The backend introduces an `ICurrentUserService` abstraction to decouple identity resolution from the endpoint:

- **Interface (`ICurrentUserService`):** Application layer. Single method returning the current user's identity.
- **`DummyCurrentUserService`:** Infrastructure layer. Returns a hardcoded placeholder identity for Sprint 4 (pre-auth).
- **When auth lands:** Swap `DummyCurrentUserService` for a real implementation that reads identity from the session/token. No endpoint or handler changes required.

### Contract Changes

- **Backend:** Create a `RegisterDogRequest` DTO with only `Name`, `Breed`, `DateOfBirth`, `Sex` — no `OwnerId`. The endpoint injects `ICurrentUserService`, reads the current user's identity, and constructs the full `RegisterDogCommand`.
- **Frontend:** Remove `customerId` from the form data, API client payload, and all tests. The client never sends identity — the server owns it.

## Notes

- This is the **proving slice** for Sprint 4 — the entire sprint builds toward this story
- Original US-084 content (Frontend Testing Setup) has been completed and moved to US-103
- Form fields match the RegisterDogCommand backend contract (US-028): Name, Breed, DateOfBirth, Sex
- **Demo:** Navigate to the register dog page, fill in name/breed/date of birth/sex, submit — see the dog in the database

## AC Revision History

- **April 14:** Form fields updated from name, breed, weight, notes to name, breed, dateOfBirth, sex to match the RegisterDogCommand backend contract (US-028)
- **April 15:** AC 6 (customerId as route param) dropped. Customer identity must not come from the client. Introduced `ICurrentUserService` abstraction — `DummyCurrentUserService` pre-auth, real implementation post-auth. Backend needs `RegisterDogRequest` DTO excluding `OwnerId`. Frontend removes `customerId` from the entire chain.
