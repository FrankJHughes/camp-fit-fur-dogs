import { vi } from 'vitest';

export function fakeSubmit<T = unknown>() {
  const submit = vi.fn<(data: T) => void>();
  return { submit };
}
