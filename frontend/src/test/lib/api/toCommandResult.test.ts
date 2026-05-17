import { toCommandResult } from '@/lib/api/commandResult';
import type { ApiResult } from '@/lib/api/client';

describe('toCommandResult', () => {
  it('returns success when result is ok', () => {
    const result: ApiResult<void> = { ok: true, data: undefined };
    expect(toCommandResult(result)).toEqual({ success: true });
  });

  it('flattens validation errors to first message per field', () => {
    const result: ApiResult<void> = {
      ok: false,
      error: {
        type: 'validation',
        message: 'Validation failed',
        status: 422,
        errors: {
          name: ['Name is required', 'Name too short'],
          breed: ['Breed is required'],
        },
      },
    };

    expect(toCommandResult(result)).toEqual({
      success: false,
      errors: { name: 'Name is required', breed: 'Breed is required' },
    });
  });

  it('returns form-level error for HTTP errors', () => {
    const result: ApiResult<void> = {
      ok: false,
      error: { type: 'http', message: 'Internal Server Error', status: 500 },
    };

    expect(toCommandResult(result)).toEqual({
      success: false,
      errors: { form: 'Internal Server Error' },
    });
  });

  it('returns network error message for network errors', () => {
    const result: ApiResult<void> = {
      ok: false,
      error: { type: 'network', message: 'Failed to fetch' },
    };

    expect(toCommandResult(result)).toEqual({
      success: false,
      errors: { form: 'A network error occurred. Please try again.' },
    });
  });
});