import { describe, it, expect, vi, beforeEach } from 'vitest';
import { getDogProfile } from '@/api/dogs/getDogProfile';

const { mockGet } = vi.hoisted(() => ({
  mockGet: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ get: mockGet }),
}));

describe('getDogProfile', () => {
  const dogId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';
  const profileData = {
    id: dogId,
    ownerId: 'owner-1234',
    name: 'Buddy',
    breed: 'Golden Retriever',
    dateOfBirth: '2023-06-15',
    sex: 'Male',
  };

  beforeEach(() => {
    mockGet.mockReset();
  });

  it('sends GET to /dogs/{id} and returns the profile on success', async () => {
    mockGet.mockResolvedValue({ ok: true, data: profileData });

    const result = await getDogProfile(dogId);

    expect(mockGet).toHaveBeenCalledWith(`/dogs/${dogId}`);
    expect(result).toEqual({ success: true, data: profileData });
  });

  it('returns notFound when the client returns a 404 error', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Not Found', status: 404 },
    });

    const result = await getDogProfile(dogId);

    expect(result).toEqual({ success: false, notFound: true });
  });

  it('returns a failure with error message on non-404 HTTP errors', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Internal Server Error', status: 500 },
    });

    const result = await getDogProfile(dogId);

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'Internal Server Error',
    });
  });

  it('returns a failure with error message on network errors', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'network', message: 'A network error occurred' },
    });

    const result = await getDogProfile(dogId);

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'A network error occurred',
    });
  });
});