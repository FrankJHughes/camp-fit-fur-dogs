import { getDogProfile } from '@/api/getDogProfile';

const dogId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';

const profileData = {
  id: dogId,
  ownerId: 'owner-1234',
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('getDogProfile', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('sends a GET request to /api/dogs/{id}', async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve(profileData),
    });
    global.fetch = fetchMock;

    const result = await getDogProfile(dogId);

    expect(fetchMock).toHaveBeenCalledWith(`/api/dogs/${dogId}`);
    expect(result).toEqual({ success: true, profile: profileData });
  });

  it('returns not found when the response is 404', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 404,
    });

    const result = await getDogProfile(dogId);

    expect(result).toEqual({ success: false, notFound: true });
  });

  it('returns failure with error when the response is not ok and not 404', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      status: 500,
    });

    const result = await getDogProfile(dogId);

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'An unexpected error occurred. Please try again.',
    });
  });

  it('returns failure with a network error when fetch throws', async () => {
    global.fetch = vi.fn().mockRejectedValue(new TypeError('Failed to fetch'));

    const result = await getDogProfile(dogId);

    expect(result).toEqual({
      success: false,
      notFound: false,
      error: 'A network error occurred. Please try again.',
    });
  });
});