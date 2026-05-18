# ADR‑0031 — Form Architecture Using React Hook Form + Zod

## Status  
Accepted

## Context  
The application requires a consistent, deterministic, and testable approach to building forms across multiple domains (customer onboarding, dog registration, authentication, staff workflows, admin tools). Prior to this decision, form handling was ad‑hoc, validation was inconsistent, and TypeScript types were duplicated or manually maintained.

Upcoming stories (e.g., US‑126 Create Account, US‑027 Register Dog, US‑110 Owner Login, US‑148 Email Verification) all require:

- Strong client‑side validation  
- Deterministic error handling  
- A unified form architecture  
- Type‑safe data structures shared between frontend and backend  
- A test harness that supports TDD  
- Predictable behavior under Next.js’s `isolatedModules` constraints  

To support these needs, several options for form state management and validation were evaluated.

## Decision  
We will standardize all new forms on:

- **React Hook Form (RHF)** for form state management  
- **Zod** for schema validation and type inference  

### Architectural rules  
- All form schemas live in `src/lib/<domain>/<feature>Schema.ts`, using lowercase camelCase naming (e.g., `createAccountSchema.ts`).  
- All form types are inferred from Zod (`z.infer<typeof Schema>`).  
- No component or API client defines its own form types.  
- Form components must not contain validation logic; all validation rules and messages must be defined exclusively in the schema file.  
- Validation uses `safeParse` and is converted into a flat error map.  
- Select fields requiring an empty initial value must use `superRefine` to preserve literal unions under `isolatedModules`.  
- Form components follow the pattern established in `AccountForm` and `DogForm`.  
- Error handling flows through `useFormErrors` and merges server + client errors deterministically.  
- Tests use React Testing Library + Vitest + jsdom and follow the established form test harness.  
- Tests assert against schema‑defined messages, ensuring a single source of truth.

## Consequences  

### Positive  
- **Deterministic validation**: Zod ensures consistent rules across client and server.  
- **Type safety**: Form types always match validation schemas.  
- **Test consistency**: Validation messages originate from schemas, ensuring tests assert against a single source of truth.  
- **Testability**: RHF integrates cleanly with React Testing Library and supports TDD.  
- **Reduced duplication**: No manually maintained form types.  
- **Predictable architecture**: All forms follow the same structure.  
- **Backend alignment**: Zod schemas can be shared or mirrored on the server.  
- **Next.js compatibility**: The `superRefine` pattern avoids type narrowing issues under `isolatedModules`.

### Negative  
- **Learning curve**: Developers must understand RHF’s controlled/uncontrolled model and Zod’s schema patterns.  
- **Refactor cost**: Existing forms may need migration.  
- **Schema complexity**: Some validation rules require `superRefine` or custom issues.
