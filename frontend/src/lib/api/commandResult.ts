import type { ApiResult } from '@/lib/api/client';

export interface CommandResult {
  success: boolean;
  errors?: Record<string, string>;
}

export function toCommandResult<T>(result: ApiResult<T>): CommandResult {
  if (result.ok) {
    return { success: true };
  }

  if (result.error.errors) {
    const errors = Object.fromEntries(
      Object.entries(result.error.errors).map(([k, v]) =>
        [k, Array.isArray(v) ? v[0] : v]
      )
    );
    return { success: false, errors };
  }

  const message = result.error.type === 'network'
    ? 'A network error occurred. Please try again.'
    : result.error.message;

  return { success: false, errors: { form: message } };
}