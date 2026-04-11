# US-084 — Register Dog Page

## Intent

As a **customer**, I want to register a new dog by filling out a form with my dog's details, so that the dog is added to my profile.

## Value

This is the first feature that lets a customer interact with the system beyond account creation. Registering a dog is the core action that makes Camp Fit Fur Dogs useful — without it, the product has no purpose. This story is also the **proving slice** for Sprint 4: the first vertical slice that includes a UI layer, exercising React form through API to database with TDD at every layer.

## Acceptance Criteria

- [ ] React form component with fields: dog name, breed, weight, notes.
- [ ] Client-side validation for required fields and data types.
- [ ] Form submits to the existing RegisterDog API endpoint via the typed API client (US-102).
- [ ] Success feedback displayed after successful registration.
- [ ] Validation errors, API errors, and network errors displayed to the user.
- [ ] `customerId` passed as route parameter or prop (design seam for future auth).
- [ ] Component tests cover render, validation, submission, and error states.
- [ ] Integration test covers form → API client → mock server → success/error.

## Emotional Guarantees

- **EG-01 Calm Confidence** — The form uses clear labels, no countdown timers, and no urgency language. The customer can take their time.
- **EG-03 Graceful Recovery** — Validation errors appear inline next to the field with a suggestion for correction. API errors display a clear message with a path forward (retry or contact support).
- **EG-05 Joyful Moments** — Successful registration includes a warm confirmation acknowledging the new dog by name.
- **EG-06 Transparent Progress** — The form clearly indicates required fields and submission state (idle, submitting, success, error).

## Edge Cases

- What happens when the API is unreachable? → Network error displayed with retry option.
- What happens when the customer submits a duplicate dog name? → API returns validation error; form displays it inline.
- What happens when the customer navigates away mid-form? → Design seam for EG-04 (Forgiveness) in a future story; not in scope for Sprint 4.
- What happens when `customerId` is missing or invalid? → Clear error message; form does not render without a valid customer context.

## Notes

- Backend RegisterDog endpoint already exists from Sprint 3.
- Uses the typed API client from US-102 (not raw fetch).
- Test infrastructure from US-103 must be in place before TDD can begin.
- `customerId` is a route parameter today; becomes session/token-derived when auth lands. The form, validation, API client call, handler, domain logic, and tests all stay the same.
- **Demo sentence:** Navigate to a customer's page, click 'Add Dog', fill in name/breed/weight, submit — see the dog in the database.
