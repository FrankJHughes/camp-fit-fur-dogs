import { describe, it, expect, vi, beforeEach } from 'vitest';
import { registerDog } from '@/api/dogs/registerDog';

const { mockPost } = vi.hoisted(() => ({
  mockPost: vi.fn(),
}));

vi.mock('@/lib/api/client', () => ({
  createApiClient: () => ({ post: mockPost }),
}));

describe('registerDog', () => {
  const formData = {
    name: 'Buddy',
    breed: 'Golden Retriever',
    dateOfBirth: '2023-06-15',
    sex: 'Male',
  };

  beforeEach(() => {
    mockPost.mockReset();
  });

  it('sends POST to /dogs/register with the form data and returns success', async () => {
    mockPost.mockResolvedValue({ ok: true, data: undefined });

    const result = await registerDog(formData);

    expect(mockPost).toHaveBeenCalledWith('/dogs/register', formData);
    expect(result).toEqual({ success: true });
  });

  it('returns validation errors when the client returns a validation error', async () => {
    mockPost.mockResolvedValue({
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        errors: { name: ['Name is required'], breed: ['Breed is required'] },
      },
    });

    const result = await registerDog(formData);

    expect(result).toEqual({
      success: false,
      errors: { name: 'Name is required', breed: 'Breed is required' },
    });
  });

  it('returns a form-level error on non-validation errors', async () => {
    mockPost.mockResolvedValue({
      ok: false,
      error: { type: 'http', message: 'Internal Server Error', status: 500 },
    });

    const result = await registerDog(formData);

    expect(result).toEqual({
      success: false,
      errors: { form: 'Internal Server Error' },
    });
  });

  it('returns a form-level error on network errors', async () => {
    mockPost.mockResolvedValue({
      ok: false,
      error: { type: 'network', message: 'A network error occurred' },
    });

    const result = await registerDog(formData);

    expect(result).toEqual({
      success: false,
      errors: { form: 'A network error occurred. Please try again.' },
    });
  });
});