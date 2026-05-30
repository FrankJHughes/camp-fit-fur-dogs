# Form Validation Architecture

This guide defines the **frontend form validation architecture** used across all vertical slices.  
Validation is implemented using **Zod schemas**, a dedicated validator function per form, and the unified **form state machine** that merges client‑side and server‑side errors.

This document describes **how validation works today**, the rules that govern it, and how developers should implement validation in new slices.

---

# Purpose

- Provide a **single source of truth** for all form validation rules  
- Ensure **consistent UX** across all forms  
- Prevent validation logic from leaking into components  
- Support **deterministic, testable** validation  
- Integrate cleanly with the **FormCommand** and **form state machine** architecture  

---

# Folder Location

This guide lives in:

```
docs/guides/developer/forms/form-validation-architecture.md
```

It belongs here because it is a **frontend slice‑level convention**, not a global system architecture rule.

---

# Core Principles

1. **Schemas define all validation rules**  
2. **Components never validate directly**  
3. **Validation messages live only in schemas**  
4. **A dedicated validator function wraps the schema**  
5. **The form state machine merges all errors**  
6. **Tests assert against schema messages**  
7. **Types are inferred from schemas — no duplication**

---

# Schema Location

Every form has a schema file located at:

```
src/lib/<domain>/<feature>Schema.ts
```

Example:

```
src/lib/account/createAccountSchema.ts
```

Schemas define:

- Field types  
- Field constraints  
- Validation messages  
- Transformations (if needed)  

Schemas must not import React, components, or API clients.

---

# Type Inference

All form types are inferred from the schema:

```ts
export type CreateAccountValues = z.infer<typeof CreateAccountSchema>;
```

This ensures:

- No duplicated types  
- No drift between schema and form  
- Strong compile‑time guarantees  

---

# Validation Execution

Validation is performed using:

```ts
const result = CreateAccountSchema.safeParse(values);
```

The validator converts the Zod error tree into a flat map:

```ts
{ fieldName: "Message" }
```

This map is returned to the form state machine.

---

# Validator Function (Required)

Form components **never** call the schema directly.

Each form has a dedicated validator:

```ts
validateXForm(values)
```

This function:

- Calls the schema  
- Converts errors into `{ field: message }`  
- Returns a deterministic error map  
- Contains no UI logic  

Example:

```ts
export function validateCreateAccountForm(values: CreateAccountValues) {
  const result = CreateAccountSchema.safeParse(values);
  return result.success
    ? {}
    : flattenZodErrors(result.error);
}
```

---

# Validation Messages

Validation messages must be defined **only** in the schema.

Components must not:

- Define messages  
- Duplicate messages  
- Override messages  
- Hardcode messages  

Tests must assert against schema messages to ensure consistency.

---

# Error Merging (Form State Machine)

The **form state machine** produces a unified `displayErrors` object that merges:

- Client‑side validation errors  
- Server‑side field errors (`command.errors`)  
- Server‑side form‑level errors (`command.error`)  

Components read only:

```ts
state.displayErrors
```

They never merge errors manually.

---

# Form Component Rules

Form components:

- Do not import schemas  
- Do not perform validation  
- Do not merge errors  
- Do not define messages  
- Do not transform values (except UI‑only formatting)  

They rely entirely on:

- `validateXForm`  
- `useFormStateMachine`  
- `FormField` for accessibility  

---

# Testing Requirements

Tests must verify:

### 1. Schema Tests  
- Field constraints  
- Required fields  
- Message correctness  
- Edge cases  

### 2. Validator Tests  
- Correct flattening  
- Correct field mapping  
- Deterministic output  

### 3. Component Tests  
- `aria-invalid` toggles  
- `aria-describedby` links  
- Error messages appear in the DOM  
- Errors clear on correction  
- Form‑level errors render via `role="alert"`  

### 4. Integration Tests  
- Backend errors merge correctly  
- Successful submit clears errors  

---

# Benefits

- **Deterministic validation** across client and server  
- **Zero duplication** of form types  
- **Predictable error handling**  
- **Unified error model**  
- **High testability**  
- **Consistent UX**  
- **Clean separation of concerns**  
- **Stable architecture across all slices**

---

# Related Documentation

- [FormCommand Architecture](ca://s?q=Show_form_command_architecture)  
- [Form State Machine](ca://s?q=Show_form_state_machine_guide)  
- [Frontend Testing](ca://s?q=Show_frontend_testing_guide)  
- [Feature Slice Walkthrough](ca://s?q=Show_feature_slice_walkthrough)  
- [Create Account Feature Slice](ca://s?q=Show_create_account_feature_slice)  

