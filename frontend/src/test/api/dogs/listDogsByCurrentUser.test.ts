const { mockGet } = vi.hoisted(() => ({
  mockGet: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ get: mockGet }),
}));

import { listDogsByCurrentUser } from '@/api/dogs/listDogsByCurrentUser';

describe('listDogsByCurrentUser', () => {
  afterEach(() => {
    mockGet.mockReset();
  });

  it('calls GET /dogs and returns dog summaries on success', async () => {
    const dogs = [
      { id: 'id-1', name: 'Biscuit', breed: 'Golden Retriever' },
      { id: 'id-2', name: 'Maple', breed: 'Beagle' },
    ];
    mockGet.mockResolvedValue({ ok: true, data: { dogs } });

    const result = await listDogsByCurrentUser();

    expect(mockGet).toHaveBeenCalledWith('/dogs');
    expect(result).toEqual({ success: true, data: { dogs } });
  });

  it('returns error on failure', async () => {
    mockGet.mockResolvedValue({
      ok: false,
      error: { type: 'http', status: 500, message: 'Server error' },
    });

    const result = await listDogsByCurrentUser();

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'Server error',
    });
  });
});
