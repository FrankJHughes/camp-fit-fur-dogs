# Form Testing Workflow

This guide defines the standard workflow for testing forms built with **React Hook Form (RHF)** and **Zod**.  
All new forms must follow this workflow to ensure deterministic behavior, strong validation guarantees, and consistent developer experience across vertical slices.

---

# Goals

- Ensure all forms behave predictably under TDD  
- Validate both client‑side and server‑side error flows  
- Provide a consistent testing harness across the application  
- Support vertical‑slice development for form‑driven stories  
- Reinforce the architectural patterns defined in the RHF + Zod ADR  

---

# Test Structure

Every form must have a dedicated test file located alongside the feature or in the appropriate test directory.

Tests must cover the following phases:

---

## 1. Initial Empty State

- Form renders without crashing  
- All fields start in their expected default state  
- No validation errors are shown initially  
- Submit button is enabled unless explicitly disabled by design  

---

## 2. Client‑Side Validation (Zod)

- Required fields produce errors when empty  
- Invalid formats (email, password rules, etc.) produce errors  
- Fields using `superRefine` correctly reject invalid combinations  
- Error messages match **schema‑defined messages** (single source of truth)  

Tests must assert against the **schema**, not hardcoded strings.

---

## 3. Server Error Merging

The form state machine merges:

- Client‑side Zod errors  
- Server‑returned field errors (`command.errors`)  
- Server‑returned form‑level errors (`command.error`)  

Tests must verify:

- Server field errors appear in the correct locations  
- Form‑level errors render in the correct alert region  
- Client‑side + server‑side errors merge deterministically  
- Errors clear when the user corrects the input  

---

## 4. Successful Submission

- Valid input triggers the submit handler  
- Form disables appropriately during submission  
- Success paths are asserted (navigation, confirmation UI, etc.)  
- No stale errors remain after success  
- Submit handler receives the correct payload  

---

# Tools & Libraries

- **React Testing Library** — rendering + interaction  
- **Vitest** — test runner  
- **jsdom** — DOM simulation (component tests only)  
- **userEvent** — realistic input simulation  
- **jest‑dom matchers** — loaded via `test/setup.ts`  

Unit tests for validators should run in the **node** environment.  
Component tests must run in **jsdom**.

---

# Reference Implementations

Use the following as canonical examples:

- `AccountForm.test.tsx`  
- `DogForm.test.tsx`  

These demonstrate:

- Correct RHF setup  
- Zod validation integration  
- Error merging patterns  
- TDD‑friendly structure  
- Proper use of the form state machine  

---

# Summary

All form tests must follow this workflow to ensure:

- Consistency  
- Reliability  
- Deterministic validation  
- Correct error merging  
- Strong TDD ergonomics  

This guide supports vertical‑slice development and reinforces the architectural patterns defined in the **Form Validation Architecture** and **FormCommand** documentation.

---

# Related Documentation

- [Form Validation Architecture](ca://s?q=Show_form_validation_architecture)  
- [Frontend Testing](ca://s?q=Show_frontend_testing_guide)  
- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)
