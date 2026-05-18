import type { MockInstance } from 'vitest';

export function asMock<T extends (...args: any) => any>(fn: T) {
  return fn as unknown as MockInstance;
}
