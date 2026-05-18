# Form Testing Workflow

This guide defines the standard workflow for testing forms built with **React Hook Form (RHF)** and **Zod**. All new forms must follow this testing approach to ensure deterministic behavior, strong validation guarantees, and consistent developer experience.

## Goals
- Ensure all forms behave predictably under TDD.
- Validate both client‑side and server‑side error flows.
- Provide a consistent testing harness across the application.
- Support vertical‑slice development for form‑driven stories.

## Test Structure
Every form must have a dedicated test file located alongside the feature or in the appropriate test directory.

Tests must cover:

### 1. Initial Empty State
- Form renders without crashing.
- All fields start in their expected default state.
- No validation errors are shown initially.

### 2. Client‑Side Validation (Zod)
- Required fields produce errors when empty.
- Invalid formats (email, password rules, etc.) produce errors.
- Select fields using `superRefine` correctly reject empty values.

### 3. Server Error Merging
- Server‑returned field errors appear in the correct locations.
- Form‑level errors (e.g., “Something went wrong”) are displayed.
- Client‑side and server‑side errors merge deterministically.

### 4. Successful Submission
- Valid input triggers the submit handler.
- The form disables appropriately during submission.
- Success paths (navigation, confirmation UI, etc.) are asserted.

## Tools & Libraries
- **React Testing Library** for rendering and interaction.
- **Vitest** as the test runner.
- **jsdom** for DOM simulation.
- **userEvent** for realistic input simulation.

## Reference Implementations
Use the following as canonical examples:

- `AccountForm.test.tsx`
- `DogForm.test.tsx`

These tests demonstrate:
- The correct RHF setup
- Zod validation integration
- Error merging patterns
- TDD‑friendly structure

## Summary
All form tests must follow this workflow to ensure consistency, reliability, and maintainability across the application. This guide supports vertical‑slice development and reinforces the architectural patterns defined in the RHF + Zod ADR.
