import { describe, it, expect, vi, beforeEach } from 'vitest';
import { login } from '@/api/identity/login';

const { mockGet } = vi.hoisted(() => ({
  mockGet: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ get: mockGet }),
}));

describe('login', () => {
  const returnUrl = 'http://localhost:3000';

  beforeEach(() => {
    mockGet.mockReset();
  });

  it('GETs /api/identity/login-url and returns success', async () => {
    mockGet.mockResolvedValue({
      ok: true,
      data: { nextUrl: '/after-login' },
    });

    const result = await login(returnUrl);

    expect(mockGet).toHaveBeenCalledWith(
      `/identity/login-url?return_url=${encodeURIComponent(returnUrl)}`
    );

    expect(result).toEqual({
      success: true,
      data: { nextUrl: '/after-login' },
    });
  });

  it('returns unauthorized when the client returns a 401 error', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Unauthorized', status: 401 },
    });

    const result = await login(returnUrl);

    expect(result).toEqual({
      success: false,
      unauthorized: true,
    });
  });

  it('returns notFound when the client returns a 404 error', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Not Found', status: 404 },
    });

    const result = await login(returnUrl);

    expect(result).toEqual({
      success: false,
      notFound: true,
    });
  });

  it('returns a failure with error message on non-401/404 HTTP errors', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Internal Server Error', status: 500 },
    });

    const result = await login(returnUrl);

    expect(result).toEqual({
      success: false,
      error: 'Internal Server Error',
    });
  });

  it('returns a failure with error message on network errors', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'network', message: 'A network error occurred' },
    });

    const result = await login(returnUrl);

    expect(result).toEqual({
      success: false,
      error: 'A network error occurred',
    });
  });
});
