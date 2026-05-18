> **Note:**  
> This testing workflow is built on the architecture defined in **ADR‑0031 — Form Architecture Using React Hook Form + Zod**.  
> All form tests must assert against schema‑defined validation messages and rely on the `useFormErrors` merging pattern.

## Form Validation Architecture

All form validation must be implemented using **Zod schemas** defined in the domain layer.  
Form components must not contain validation logic.

### Rules

- Every form has a schema file located at:
  `src/lib/<domain>/<feature>Schema.ts`  
  Example: `src/lib/account/createAccountSchema.ts`

- All form types are inferred from the schema:
  ```ts
  export type CreateAccountValues = z.infer<typeof CreateAccountSchema>;
  ```

- Validation is performed via:
  ```ts
  CreateAccountSchema.safeParse(values)
  ```
  and converted into a flat `{ field: message }` error map.

- Form components call a dedicated validator:
  `validateXForm(values)`, which wraps the schema.

- Validation messages must be defined **only** in the schema.  
  Components must not define or duplicate validation messages.

- Client‑side and server‑side errors are merged using:
  `useFormErrors().merge(errors)`

- Tests assert against schema‑defined messages, ensuring a single source of truth.

### Benefits

- Deterministic validation across client and server  
- Zero duplication of form types  
- Predictable error handling  
- Strong testability  
- Consistent architecture across all vertical slices
