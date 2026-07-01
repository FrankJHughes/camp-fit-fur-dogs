import { describe, it, expect, vi, beforeEach } from 'vitest';
import { logout } from '@/api/authentication/logout';

const { mockGet } = vi.hoisted(() => ({
  mockGet: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ get: mockGet }),
}));

describe('logout', () => {
  beforeEach(() => {
    mockGet.mockReset();
  });

  it('GETs /auth/logout and returns success', async () => {
    mockGet.mockResolvedValue({ ok: true, data: {} });

    const result = await logout('http://localhost:3000');

    expect(mockGet).toHaveBeenCalledWith(`/auth/logout?return_url=${encodeURIComponent('http://localhost:3000')}`);
    expect(result).toEqual({ success: true, data: {} });
  });

  it('returns error on failure', async () => {
    mockGet.mockRejectedValue(new Error('boom'));

    const result = await logout('http://localhost:3000');

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'boom',
    });
  });
});
