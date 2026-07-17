# Frontend Testing Guide

This guide explains how frontend tests are structured, how Vitest projects are organized, and how to write fast, predictable tests for React components, forms, hooks, and API clients.

---

# Quick Start

```powershell
cd frontend
npm test              # watch mode
npm test -- --run     # single run (CI)
npx vitest --run      # skip npm overhead
```

Use `npx vitest --run` for the fastest local feedback loop.

---

# Test Projects

Vitest defines **two projects**, each with its own environment and glob patterns:

| Project | Environment | Glob | Use for |
|--------|-------------|------|---------|
| `unit` | `node` | `test/lib/**/*.test.ts` | API clients, utilities, pure logic |
| `components` | `jsdom` | `test/app/**`, `test/components/**` | React components, pages, hooks requiring DOM |

### Why two projects

- **jsdom is slow** — especially in Docker  
- Unit tests stay extremely fast (~50ms)  
- Component tests run only where needed (~300–600ms)  

This separation keeps CI and local dev fast and predictable.

---

# Writing a Unit Test (node environment)

```ts
// test/lib/api/client.test.ts
import { createApiClient } from '@/lib/api/client';

describe('ApiClient', () => {
  afterEach(() => vi.restoreAllMocks());

  it('returns ok with data on success', async () => {
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({
      ok: true,
      status: 200,
      json: () => Promise.resolve({ id: 1 }),
    }));

    const client = createApiClient('http://localhost');
    const result = await client.get('/dogs');

    expect(result).toEqual({ ok: true, data: { id: 1 } });
  });
});
```

### Key points

- Use `vi.stubGlobal('fetch', ...)` — no jsdom required  
- Always clean up with `vi.restoreAllMocks()`  
- Import using the `@` alias (resolved via `vite-tsconfig-paths`)  

Unit tests must **never** import React or DOM utilities.

---

# Writing a Component Test (jsdom environment)

```ts
// test/app/page.test.tsx
import { render, screen } from '@testing-library/react';
import Home from '@/app/page';

describe('Home page', () => {
  it('renders the heading', () => {
    render(<Home />);
    expect(
      screen.getByRole('heading', { name: /camp fit fur dogs/i }),
    ).toBeInTheDocument();
  });
});
```

### Key points

- Use `@testing-library/react` for rendering  
- `jest-dom` matchers (e.g., `toBeInTheDocument`) are loaded via `test/setup.ts`  
- Keep component tests focused — jsdom is slower  

Component tests should validate **behavior**, not implementation details.

---

# Where to Put Tests

```
test/
├── setup.ts                    # jest-dom matchers (jsdom project only)
├── app/
│   └── page.test.tsx           # page component tests
├── components/
│   └── *.test.tsx              # shared component tests
└── lib/
    └── api/
        └── client.test.ts      # API client unit tests
```

Mirror the `src/` structure under `test/`.  
Vitest project globs determine which environment runs each test.

---

# TDD Workflow

1. **Red** — Write the failing test  
2. **Green** — Implement the minimum passing code  
3. **Refactor** — Clean up duplication  
4. Run `npx vitest --run` to confirm stability  

This workflow applies to:

- API clients  
- Hooks  
- Components  
- FormCommand flows  
- Pages  

---

# Troubleshooting

| Symptom | Cause | Fix |
|--------|--------|------|
| Tests take 300+ seconds | All tests running in jsdom | Ensure unit tests match the `unit` project glob |
| `Cannot find module '@/lib/...'` | Path alias not resolving | Verify `paths` in `test/tsconfig.json` |
| VS Code shows red squiggles but tests pass | Stale TS server | Restart TS Server |
| Plugin type errors in `vitest.config.mts` | Next.js + Vite plugin mismatch | Keep the `as any` cast on the plugins array |

---

# Related Documentation

- [Test Architecture](ca://s?q=Show_test_architecture_guide)  
- [Form Testing](ca://s?q=Show_form_testing_guide)  
- [Authentication Testing](ca://s?q=Show_authentication_testing_guide)  
- [Form Validation Architecture](ca://s?q=Show_form_validation_architecture)  
- [Feature Slice Walkthrough](ca://s?q=Show_feature_slice_walkthrough)
