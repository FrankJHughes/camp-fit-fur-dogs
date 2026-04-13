# Changelog

All notable changes to the Camp Fit Fur Dogs frontend will be documented in this file.

Format follows [Keep a Changelog](https://keepachangelog.com/).

## [Unreleased]

### Added
- Vitest 3 configured as the frontend test runner with jsdom environment
- React Testing Library and jest-dom matchers installed and configured
- First passing component test — verifies landing page heading renders
- `npm test` runs all frontend tests
- Testing guide (`docs/testing-guide.md`) — how to write and run component tests
- `vite-tsconfig-paths` plugin for `@/*` path alias resolution in tests

### Changed
- Restructured project layout — moved `package.json` and configs from `frontend/src/` up to `frontend/` (project root). `src/` now contains only application source code.
