import { describe, it, expect, vi, beforeEach } from 'vitest';
import { identify } from '@/api/identity/identify';

const { mockGet } = vi.hoisted(() => ({
  mockGet: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ get: mockGet }),
}));

describe('identify', () => {
  beforeEach(() => {
    mockGet.mockReset();
  });

  it('GETs /api/identity and returns success', async () => {
    mockGet.mockResolvedValue({
      ok: true,
      data: { name: 'Harry Styles' },
    });

    const result = await identify();

    expect(mockGet).toHaveBeenCalledWith('/identity');

    expect(result).toEqual({
      success: true,
      data: { name: 'Harry Styles' },
    });
  });

  it('returns unauthorized when the client returns a 401 error', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Unauthorized', status: 401 },
    });

    const result = await identify();

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

    const result = await identify();

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

    const result = await identify();

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

    const result = await identify();

    expect(result).toEqual({
      success: false,
      error: 'A network error occurred',
    });
  });
});
