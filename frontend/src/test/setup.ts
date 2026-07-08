import '@testing-library/jest-dom/vitest';
import { vi } from 'vitest';

if (typeof window !== 'undefined' && typeof window.HTMLDialogElement !== 'undefined') {
  if (!window.HTMLDialogElement.prototype.showModal) {
    window.HTMLDialogElement.prototype.showModal = function showModal() {
      this.setAttribute('open', '');
    };
  }

  if (!window.HTMLDialogElement.prototype.close) {
    window.HTMLDialogElement.prototype.close = function close() {
      this.removeAttribute('open');
    };
  }
}

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
