# Frontend Testing Guide

## Quick Start

```powershell
cd frontend
npm test              # watch mode
npm test -- --run     # single run (CI)
npx vitest --run      # skip npm overhead
```

## Test Projects

The Vitest config defines two projects with different environments:

| Project | Environment | Glob | Use for |
|---|---|---|---|
| `unit` | `node` | `test/lib/**/*.test.ts` | API client, utilities, pure logic |
| `components` | `jsdom` | `test/app/**`, `test/components/**` | React components, pages, hooks with DOM |

**Why?** jsdom initialization is expensive in Docker. Scoping it to component tests only keeps unit tests fast (~50ms vs ~300s).

## Writing a Unit Test (node environment)

```typescript
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

**Key points:**
- Use `vi.stubGlobal('fetch', ...)` to mock fetch — no jsdom needed
- `afterEach(() => vi.restoreAllMocks())` cleans up between tests
- Import from `@/lib/...` — the path alias resolves to `src/lib/...`

## Writing a Component Test (jsdom environment)

```typescript
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

**Key points:**
- `render()` and `screen` come from `@testing-library/react`
- `toBeInTheDocument()` comes from `jest-dom` matchers (loaded via `test/setup.ts`)
- Component tests are slower due to jsdom — keep assertions focused

## Where to Put Tests

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

Mirror the `src/` structure under `test/`. The glob patterns in `vitest.config.mts` determine which project (and environment) picks up each file.

## TDD Workflow

1. **Red** — Write the failing test first
2. **Green** — Write the minimum code to pass
3. **Refactor** — Clean up duplication, extract helpers
4. Run `npx vitest --run` after each step to confirm state

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Tests take 300+ seconds | All tests running in jsdom | Ensure unit tests match the `unit` project glob |
| `Cannot find module '@/lib/...'` | test/tsconfig.json path alias | Verify `paths: { "@/*": ["../src/*"] }` in `test/tsconfig.json` |
| VS Code red squiggles but tests pass | Stale TS server | `Ctrl+Shift+P` → TypeScript: Restart TS Server |
| Plugin type errors in vitest.config.mts | Vite version mismatch (Next.js vs Vitest) | Keep the `as any` cast on the plugins array |
