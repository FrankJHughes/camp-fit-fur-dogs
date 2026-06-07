# Frontend Architecture Guide

This guide documents the architecture of the Camp Fit Fur Dogs frontend, including folder structure, form architecture, API client conventions, and page orchestration.

---

# Folder Structure

```
frontend/src/
    api/
    app/
    components/
    hooks/
    lib/
    state/
    styles/
```

---

# Form Architecture

Forms use:

- `FormCommand.run`
- `useFormStateMachine`
- `FormField`
- Zod schemas
- API clients returning `CommandResult`

---

# API Client Conventions

- One file per slice: `api/<useCase>.ts`
- Always return `CommandResult` or `QueryState<T>`
- Never throw
- Normalize errors

---

# Page Orchestration

Pages are thin:

- `'use client'`
- Import form component
- Use `useCommand(apiFn, onSuccess)`
- Redirect via `router.push()`

---

# Query Architecture

- `useApiQuery(() => queryFn(id).then(toQueryState), [id])`
- Branch on `state.status`
- Pass resolved data to components

---

# Shared UI Components

- `FormField`
- `Button`
- `Card`
- `ErrorSummary`

---

# Testing

- Vitest + RTL
- Component tests
- API client tests
- Page tests
