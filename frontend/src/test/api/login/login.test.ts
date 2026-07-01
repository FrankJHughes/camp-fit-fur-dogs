import { describe, it, expect, vi, beforeEach } from 'vitest';
import { login } from '@/api/login/login';

const { mockPost } = vi.hoisted(() => ({
  mockPost: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ post: mockPost }),
}));

describe('login', () => {
  beforeEach(() => {
    mockPost.mockReset();
  });

  it('POSTs to /auth/login and returns success', async () => {
    mockPost.mockResolvedValue({ ok: true });

    const result = await login();

    expect(mockPost).toHaveBeenCalledWith('/auth/login', {});
    expect(result).toEqual({ success: true });
  });

  it('returns error on network or server failure', async () => {
    mockPost.mockRejectedValue(new Error('boom'));

    const result = await login();

    expect(result).toEqual({
      success: false,
      error: 'boom',
    });
  });
});
