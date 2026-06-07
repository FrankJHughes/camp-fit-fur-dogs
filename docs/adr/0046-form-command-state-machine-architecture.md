# ADR‑0046 — FormCommand State Machine Architecture

## Status  
Accepted

## Context  
Frontend forms originally relied on ad‑hoc validation and submission logic:

- Validation rules were duplicated across components.  
- Error handling was inconsistent.  
- Forms behaved differently across slices.  
- Client‑side and server‑side errors were merged inconsistently.  
- UX patterns (loading, success, error) varied widely.

As the number of forms grew, a unified architecture was required to ensure consistency, predictability, and accessibility.

## Decision  
The system now uses a **FormCommand‑driven state machine architecture** for all frontend forms.

### Architecture Characteristics

1. **Single source of truth**  
   - Each form defines a `FormCommand` that encapsulates validation and submission.

2. **Deterministic state machine**  
   - States include: `Idle`, `Validating`, `Submitting`, `Success`, `Error`.  
   - Transitions are explicit and predictable.

3. **Unified validation model**  
   - Zod schemas define validation rules.  
   - Client‑side and server‑side errors are merged into a single error model.

4. **Consistent UX patterns**  
   - Loading indicators  
   - Inline error messages  
   - Success transitions  
   - Accessibility attributes

5. **Slice‑level isolation**  
   - Each form lives within its slice.  
   - No global form logic or shared mutable state.

6. **Testability**  
   - State machine transitions can be tested independently.  
   - FormCommand logic is fully deterministic.

## Consequences  

### Positive  
- Predictable, consistent form behavior across the entire app.  
- Strong accessibility guarantees.  
- Reduced duplication of validation logic.  
- Easier onboarding for developers.  
- Highly testable form flows.

### Negative  
- Slightly more boilerplate when creating new forms.  
- Requires developers to understand the state machine model.  
