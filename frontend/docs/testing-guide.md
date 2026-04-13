# Frontend Testing Guide

## Stack

| Tool | Role |
|------|------|
| [Vitest 3](https://vitest.dev/) | Test runner (Vite-native, Jest-compatible API) |
| [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/) | Component rendering and DOM queries |
| [jest-dom](https://github.com/testing-library/jest-dom) | Custom matchers (`toBeInTheDocument`, etc.) |
| [jsdom](https://github.com/jsdom/jsdom) | Browser environment simulation |

## Running Tests

```bash
# Run all tests once
npm test -- --run

# Watch mode (re-runs on file changes)
npm test
```

## Writing a Component Test

Tests live in `test/`, mirroring the `src/` directory structure:

```
src/app/page.tsx        →  test/app/page.test.tsx
src/app/dogs/page.tsx   →  test/app/dogs/page.test.tsx
```

### Minimal example

```tsx
import { render, screen } from '@testing-library/react';
import { describe, it, expect } from 'vitest';
import Home from '@/app/page';

describe('Home page', () => {
  it('renders the heading', () => {
    render(<Home />);

    expect(
      screen.getByRole('heading', { level: 1, name: /camp fit fur dogs/i })
    ).toBeInTheDocument();
  });
});
```

### Key patterns

- **Query by role, not by test ID.** `screen.getByRole('heading', { name: /text/i })` tests what the user sees, not implementation details.
- **Mock fetch when components call APIs.** Use `vi.stubGlobal('fetch', ...)` to prevent real HTTP calls and avoid `act()` warnings.
- **Clean up mocks.** Call `vi.restoreAllMocks()` at the end of tests that stub globals.

## Project Structure

```
frontend/
├── vitest.config.mts        # Vitest configuration
├── test/
│   ├── env.d.ts             # Type declarations (vitest/globals, jest-dom)
│   ├── setup.ts             # Global setup — imports jest-dom matchers
│   ├── tsconfig.json        # Test-specific TypeScript config
│   └── app/
│       └── page.test.tsx    # Component tests mirror src/ layout
└── src/
    └── app/
        └── page.tsx
```

## Configuration Files

| File | Purpose |
|------|---------|
| `vitest.config.mts` | Test runner config — environment, setup files, test file globs |
| `test/setup.ts` | Runs before every test suite — loads jest-dom matchers |
| `test/env.d.ts` | Triple-slash directives for `vitest/globals` and `@testing-library/jest-dom` types |
| `test/tsconfig.json` | Extends root `tsconfig.json` for test files |
