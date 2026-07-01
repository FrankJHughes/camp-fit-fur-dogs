import { describe, it, expect, vi, beforeEach } from 'vitest';
import { login } from '@/api/authentication/login';

const { mockGet } = vi.hoisted(() => ({
  mockGet: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ get: mockGet }),
}));

describe('login', () => {
  beforeEach(() => {
    mockGet.mockReset();
  });

  it('GETs /auth/login and returns success', async () => {
    mockGet.mockResolvedValue({ ok: true, data: {} });

    const result = await login('http://localhost:3000');

    expect(mockGet).toHaveBeenCalledWith(`/auth/login?return_url=${encodeURIComponent('http://localhost:3000')}`);
    expect(result).toEqual({ success: true, data: {} });
  });

  it('returns error on network or server failure', async () => {
    mockGet.mockRejectedValue(new Error('boom'));

    const result = await login('http://localhost:3000');

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'boom',
    });
  });
});
