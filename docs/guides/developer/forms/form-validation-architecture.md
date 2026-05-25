## Form Validation Architecture

All form validation is implemented using **Zod schemas** defined in the frontend `src/lib/<domain>/` directory.  
Form components must not contain validation logic — validation is performed by the schema and the form state machine.

### Rules

- Every form has a schema file located at:  
  `src/lib/<domain>/<feature>Schema.ts`  
  Example:  
  `````ts
  src/lib/account/createAccountSchema.ts
  `````

- All form types are inferred from the schema:  
  `````ts
  export type CreateAccountValues = z.infer<typeof CreateAccountSchema>;
  `````

- Validation is performed via:  
  `````ts
  const result = CreateAccountSchema.safeParse(values);
  `````  
  and converted into a flat `{ field: message }` error map.

- Form components **never** call the schema directly.  
  They call a dedicated validator function:  
  `````ts
  validateXForm(values)
  `````  
  which wraps the schema and returns `{ field: message }` maps.

- Validation messages must be defined **only** in the schema.  
  Components must not define or duplicate validation messages.

- Client‑side and server‑side errors are merged by the **form state machine**, not manually.  
  The state machine produces a unified `displayErrors` object containing:  
  - field‑level errors  
  - form‑level errors  
  - merged backend errors (`command.errors`, `command.error`)

- Tests must assert against **schema‑defined messages**, ensuring a single source of truth.

### Benefits

- Deterministic validation across client and server  
- Zero duplication of form types  
- Predictable error handling  
- Strong testability  
- Consistent architecture across all vertical slices  
- Unified error model via the form state machine  
