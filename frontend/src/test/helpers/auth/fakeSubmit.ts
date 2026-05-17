// test/helpers/auth/fakeSubmit.ts
import { vi } from "vitest";

export function fakeAsyncSubmit() {
  return vi.fn(() =>
    new Promise(resolve =>
      setTimeout(() => resolve({ success: true }), 10)
    )
  );
}
