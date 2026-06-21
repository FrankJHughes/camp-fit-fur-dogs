# Frontend Architecture Guide  
*A developer‑facing overview of the Camp Fit Fur Dogs frontend architecture.*

The Camp Fit Fur Dogs frontend is built on **Next.js (App Router)** and follows a predictable, slice‑oriented structure.  
This guide explains the folder layout, form architecture, API client conventions, page orchestration, query patterns, shared UI components, and testing approach.

The goal is to keep the frontend:

- Predictable  
- Testable  
- Declarative  
- Aligned with backend slice architecture  
- Easy to extend  

---

# 1. Folder Structure

The frontend uses a clear, layered folder structure:

```
frontend/src/
    api/          ← API clients (one file per slice)
    app/          ← Next.js App Router pages
    components/   ← Reusable UI components
    hooks/        ← Custom React hooks
    lib/          ← Utilities (non‑domain)
    state/        ← Client‑side state machines
    styles/       ← Global styles and CSS modules
```

### Folder Intent Summary

- **api/** — typed API clients returning `CommandResult` or `QueryState<T>`  
- **app/** — pages, routing, and page‑level orchestration  
- **components/** — reusable UI building blocks  
- **hooks/** — composable logic (queries, commands, state machines)  
- **lib/** — helpers, adapters, and non‑domain utilities  
- **state/** — form state machines and client‑side reducers  
- **styles/** — global and modular styles  

This structure mirrors backend slice boundaries while remaining idiomatic to Next.js.

---

# 2. Form Architecture

Forms follow a consistent pattern built on:

- `FormCommand.run`  
- `useFormStateMachine`  
- `FormField`  
- Zod schemas  
- API clients returning `CommandResult`  

### Form Flow

```
User Input
    ↓
FormField components
    ↓
useFormStateMachine (validation + state)
    ↓
FormCommand.run (submit)
    ↓
API client (returns CommandResult)
    ↓
Success or error state
```

### Key Principles

- Validation is handled by **Zod** and the **state machine**, not inside components  
- Submit logic is centralized in **FormCommand.run**  
- Errors are normalized into a predictable shape  
- Forms remain declarative and predictable  

---

# 3. API Client Conventions

API clients live in:

```
src/api/<useCase>.ts
```

### Rules

- **One file per slice**  
- Always return:
  - `CommandResult` for commands  
  - `QueryState<T>` for queries  
- **Never throw** — errors are normalized  
- Use `fetch` with:
  - JSON body  
  - Correct HTTP method  
  - Error normalization  

### Example Pattern

```ts
export async function registerDog(data: RegisterDogRequest): Promise<CommandResult<RegisterDogResponse>> {
    const res = await fetch('/api/dogs/register', { method: 'POST', body: JSON.stringify(data) });
    return normalizeCommandResult(res);
}
```

API clients are intentionally thin and predictable.

---

# 4. Page Orchestration

Pages are **thin orchestration layers**.

### Page Responsibilities

- `'use client'`  
- Import the form component  
- Use `useCommand(apiFn, onSuccess)`  
- Redirect via `router.push()`  
- Pass errors + loading state to the form  

### Example Flow

```
Page
  ↓
useCommand(apiFn)
  ↓
Form component
  ↓
FormCommand.run
  ↓
API client
```

Pages do not contain business logic — they orchestrate UI and navigation.

---

# 5. Query Architecture

Queries use:

```
useApiQuery(() => queryFn(id).then(toQueryState), [id])
```

### Query Flow

```
useApiQuery
    ↓
QueryState<T>
    ↓
Branch on state.status
    ↓
Render components
```

### Status Branching

- `loading`  
- `error`  
- `notFound`  
- `success`  

### Example

```ts
const state = useApiQuery(() => getDog(id).then(toQueryState), [id]);

if (state.status === 'loading') return <Loading />;
if (state.status === 'error') return <ErrorSummary errors={state.errors} />;
if (state.status === 'notFound') return <NotFound />;

return <DogDetails dog={state.data} />;
```

Queries remain declarative and predictable.

---

# 6. Shared UI Components

Common UI building blocks live in `components/`:

- `FormField`  
- `Button`  
- `Card`  
- `ErrorSummary`  

These components:

- Are stateless  
- Are reusable  
- Accept props only  
- Do not contain business logic  

They form the foundation of the frontend UI.

---

# 7. Testing

The frontend uses:

- **Vitest**  
- **React Testing Library (RTL)**  

### Test Types

- **Component tests** — rendering, props, UI behavior  
- **API client tests** — fetch mocking, error normalization  
- **Page tests** — orchestration, routing, integration behavior  
- **Form tests** — validation + state machine behavior  

Testing follows the same principles as the backend:

- Predictable  
- Deterministic  
- Slice‑aligned  

---

# Summary

The Camp Fit Fur Dogs frontend architecture is:

- Declarative  
- Predictable  
- Slice‑aligned  
- Easy to test  
- Easy to extend  

It mirrors backend conventions while remaining idiomatic to Next.js.

