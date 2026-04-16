import { registerDog } from '@/api/registerDog';

const validData = {
  name: 'Buddy',
  breed: 'Golden Retriever',
  dateOfBirth: '2023-06-15',
  sex: 'Male',
};

describe('registerDog', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('sends a POST request to /api/dogs/register', async () => {
    const fetchMock = vi.fn().mockResolvedValue({
      ok: true,
      json: () => Promise.resolve({ success: true }),
    });
    global.fetch = fetchMock;

    const result = await registerDog(validData);

    expect(fetchMock).toHaveBeenCalledWith('/api/dogs/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(validData),
    });
    expect(result).toEqual({ success: true });
  });

  it('returns failure with errors when the response is not ok', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      json: () => Promise.resolve({
        errors: { name: 'Name is required' },
      }),
    });

    const result = await registerDog(validData);

    expect(result).toEqual({
      success: false,
      errors: { name: 'Name is required' },
    });
  });

  it('returns failure with a network error when fetch throws', async () => {
    global.fetch = vi.fn().mockRejectedValue(new TypeError('Failed to fetch'));

    const result = await registerDog(validData);

    expect(result).toEqual({
      success: false,
      errors: { form: 'A network error occurred. Please try again.' },
    });
  });

  it('returns failure with a generic error when the error response is not JSON', async () => {
    global.fetch = vi.fn().mockResolvedValue({
      ok: false,
      json: () => Promise.reject(new SyntaxError('Unexpected token <')),
    });

    const result = await registerDog(validData);

    expect(result).toEqual({
      success: false,
      errors: { form: 'An unexpected error occurred. Please try again.' },
    });
  });
});
