import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';

export default defineConfig({
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  plugins: [react()] as any,
  resolve: {
    tsconfigPaths: true,
  },
  test: {
    testTimeout: 15000,
    projects: [
      {
        extends: true,
        test: {
          name: 'unit',
          environment: 'node',
          globals: true,
          include: ['./test/lib/**/*.test.ts', './test/api/**/*.test.ts'],
          exclude: ['./test/lib/hooks/**', './test/lib/components/**']
        },
      },
      {
        extends: true,
        test: {
          name: 'components',
          environment: 'jsdom',
          globals: true,
          setupFiles: ['./test/setup.ts'],
          include: ['./test/app/**/*.test.{ts,tsx}', './test/components/**/*.test.{ts,tsx}', './test/hooks/**/*.test.{ts,tsx}',
            './test/lib/components/**/*.test.{ts,tsx}', './test/lib/hooks/**/*.test.{ts,tsx}'],
        },
      },
      {
        extends: true,
        test: {
          name: 'integration',
          environment: 'jsdom',
          globals: true,
          setupFiles: ['./test/setup.ts'],
          include: ['./test/integration/**/*.test.{ts,tsx}'],
        },
      },
    ],
  },
});

