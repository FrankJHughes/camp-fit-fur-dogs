# Frontend Conventions (CampFitFurDogs)

The frontend mirrors backend slice structure and enforces strict separation
between UI, hooks, and API calls.

---

## Folder Structure

````text
frontend/
  src/
    api/<aggregate>/...
    components/<aggregate>/...
    lib/<aggregate>/...
    hooks/<aggregate>/...
    lib/api/...
    lib/hooks/...
    lib/components/...
    app/...
  test/...
````

---

## Rules

- One slice per aggregate  
- Hooks must not perform fetches directly  
- API layer owns all HTTP calls  
- Components must be pure and testable  
- No business logic in components  
- Tests must not hit real APIs  
- All API clients return `CommandResult` or `QueryState`  
- No direct use of `fetch` in components or hooks  

---

## Form Conventions

Forms must:

- use `FormCommand.run`  
- validate using Zod schemas  
- manage state via `useFormStateMachine`  
- display errors via `ErrorSummary`  

Form components must not contain business logic.

---

## Query Conventions

Queries must:

- use `useApiQuery`  
- be pure functions  
- expose explicit query states (`loading`, `error`, `success`)  
- never perform implicit fetches  

---

## Prohibitions

Frontend code must not:

- embed business rules  
- bypass the API layer  
- call backend endpoints directly via fetch  
