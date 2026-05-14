# Setup Node + pnpm

A reusable GitHub Action that provides a consistent, reliable setup for:

- Node.js 22  
- Global pnpm installation  
- pnpm store caching  

This action centralizes the Node/pnpm setup logic used across all workflows
(`ci.yaml`, `preview.yaml`, and any future pipelines).  
It eliminates duplication, prevents drift, and ensures every workflow uses the
same package manager configuration.

---

## Why this exists

GitHub runners do not include pnpm by default, and Corepack shims are unreliable
in CI environments. Installing pnpm globally via npm is the most stable
approach, and caching the pnpm store significantly speeds up dependency
installation.

By extracting this into a reusable action, we ensure:

- One place to update Node versions  
- One place to update pnpm versions  
- One place to adjust caching strategy  
- Cleaner workflows with less repeated YAML  
- Fully deterministic behavior across all pipelines  

---

## What this action does

1. Installs **Node.js 22** using `actions/setup-node`.
2. Installs **pnpm globally** using `npm install -g pnpm`.
3. Restores and saves the **pnpm store cache** for faster installs.

This matches the unified strategy used across the repository.

---

## Usage

In any workflow:

```yaml
- uses: ./.github/actions/setup-node-pnpm
```

After this step, you can safely run:

```yaml
run: pnpm install --frozen-lockfile
```

or any other pnpm command.

---

## Requirements

- A `pnpm-lock.yaml` file must exist in the repository root or workspace.
- Workflows must check out the repository before using this action.

---

## Notes

- This action intentionally does **not** install project dependencies.
  Workflows remain responsible for running `pnpm install` in the appropriate
  working directory.
- This action does **not** install Vercel CLI globally.  
  Use `npx vercel` instead for deterministic behavior.

---

## Example

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: ./.github/actions/setup-node-pnpm

      - name: Install dependencies
        working-directory: frontend
        run: pnpm install --frozen-lockfile

      - name: Run tests
        working-directory: frontend
        run: pnpm test --run
```

---

## License

This action is part of the Camp Fit Fur Dogs repository and follows the
repository’s licensing terms.
