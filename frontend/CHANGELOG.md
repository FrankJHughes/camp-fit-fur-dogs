# Changelog

All notable changes to the Camp Fit Fur Dogs frontend will be documented in this file.

Format follows [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added
- View Dog Profile page (`/dogs/[id]`) — API client, `DogProfileCard` component, dynamic route with loading/404/error states (US-029)
- Vitest 3 configured as the frontend test runner with jsdom environment
- React Testing Library and jest-dom matchers installed and configured
- First passing component test — verifies landing page heading renders
- `npm test` runs all frontend tests
- Testing guide (`docs/testing-guide.md`) — how to write and run component tests
- `vite-tsconfig-paths` plugin for `@/*` path alias resolution in tests
- API client (`src/lib/api/client.ts`) — `createApiClient(baseUrl)` factory with `get`, `post`, `put`, `delete` and discriminated-union `ApiResult<T>` return type
- Three typed error channels: `network`, `http`, `validation` (422 with per-field errors)
- 7 unit tests for API client covering all verbs and error paths

### Changed
- Restructured project layout — moved `package.json` and configs from `frontend/src/` up to `frontend/` (project root). `src/` now contains only application source code.
- Vitest config migrated from global `environment: 'jsdom'` to `test.projects` — unit tests run in `node` (~50ms), component tests in `jsdom`
- `test/tsconfig.json` — fixed `@/*` path alias and source file inclusion for VS Code IntelliSense
