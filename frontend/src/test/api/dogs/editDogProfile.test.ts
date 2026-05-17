import { editDogProfile } from '@/api/dogs/editDogProfile';

const { mockPut } = vi.hoisted(() => ({ mockPut: vi.fn() }));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({
    get: vi.fn(),
    post: vi.fn(),
    put: mockPut,
    delete: vi.fn(),
  }),
}));

const dogId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890';

const validData = {
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('editDogProfile', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('returns success when the API call succeeds', async () => {
    mockPut.mockResolvedValue({ ok: true, data: undefined });

    const result = await editDogProfile(dogId, validData);

    expect(mockPut).toHaveBeenCalledWith(`/dogs/${dogId}`, validData);
    expect(result).toEqual({ success: true });
  });

  it('returns field errors when the API returns validation errors', async () => {
    mockPut.mockResolvedValue({
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        errors: { name: ['Name is required'] },
      },
    });

    const result = await editDogProfile(dogId, validData);

    expect(result).toEqual({
      success: false,
      errors: { name: 'Name is required' },
    });
  });

  it('returns a form-level error on HTTP errors', async () => {
    mockPut.mockResolvedValue({
      ok: false,
      error: {
        type: 'http',
        message: 'Internal Server Error',
        status: 500,
      },
    });

    const result = await editDogProfile(dogId, validData);

    expect(result).toEqual({
      success: false,
      errors: { form: 'Internal Server Error' },
    });
  });

  it('returns a form-level error on network errors', async () => {
    mockPut.mockResolvedValue({
      ok: false,
      error: {
        type: 'network',
        message: 'Failed to fetch',
      },
    });

    const result = await editDogProfile(dogId, validData);

    expect(result).toEqual({
      success: false,
      errors: { form: 'A network error occurred. Please try again.' },
    });
  });
});