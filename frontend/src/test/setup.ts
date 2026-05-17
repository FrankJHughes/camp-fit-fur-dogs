import '@testing-library/jest-dom/vitest';
import { vi } from 'vitest';

export const apiClientMock = {
  get: vi.fn().mockResolvedValue({
    ok: true,
    json: () => Promise.resolve({}),
  }),
  post: vi.fn().mockResolvedValue({
    ok: true,
    json: () => Promise.resolve({ success: true }),
  }),
  put: vi.fn().mockResolvedValue({
    ok: true,
    json: () => Promise.resolve({}),
  }),
  delete: vi.fn().mockResolvedValue({
    ok: true,
    json: () => Promise.resolve({}),
  }),
};

vi.mock('@/lib/api/client', () => {
  return {
    createApiClient: () => apiClientMock,
  };
});
