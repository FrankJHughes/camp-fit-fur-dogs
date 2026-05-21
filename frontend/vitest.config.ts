// vite.config.ts
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import tsconfigPaths from 'vite-tsconfig-paths';
import path from 'node:path';

export default defineConfig({
  plugins: [react(), tsconfigPaths()],

  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
    },
  },

  // Ensure esbuild treats TS/TSX correctly for transforms (dev, SSR)
  esbuild: {
    loader: 'tsx',
    jsx: 'automatic',
  },

  // Ensure dependency prebundling uses TypeScript loaders where needed
  optimizeDeps: {
    esbuildOptions: {
      loader: {
        '.ts': 'ts',
        '.tsx': 'tsx',
        '.js': 'js',
        '.jsx': 'jsx',
      },
    },
  },

  // If some packages use modern syntax that breaks SSR parsing, add them here
  // e.g. noExternal: ['some-esm-only-package']
  ssr: {
    noExternal: [],
  },

  test: {
    environment: 'jsdom',
    globals: true,
    css: false,
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/test/**/*.test.{ts,tsx}'],

    // Vitest sometimes needs certain deps to be inlined for SSR-like transforms.
    // Add packages here if you see parse errors coming from node_modules.
    deps: {
      inline: [],
    },
  },
});
