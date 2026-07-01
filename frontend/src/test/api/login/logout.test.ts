import { describe, it, expect, vi, beforeEach } from 'vitest';
import { logout } from '@/api/logout/logout';

const { mockPost } = vi.hoisted(() => ({
  mockPost: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ post: mockPost }),
}));

describe('logout', () => {
  beforeEach(() => {
    mockPost.mockReset();
  });

  it('POSTs to /auth/logout and returns success', async () => {
    mockPost.mockResolvedValue({ ok: true });

    const result = await logout();

    expect(mockPost).toHaveBeenCalledWith('/auth/logout', {});
    expect(result).toEqual({ success: true });
  });

  it('returns error on failure', async () => {
    mockPost.mockRejectedValue(new Error('boom'));

    const result = await logout();

    expect(result).toEqual({
      success: false,
      error: 'boom',
    });
  });
});
