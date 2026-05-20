// src/lib/api/toCommandResult.ts
import type { ApiResult } from './client';
import type { CommandResult } from './commandResult';

function normalizeErrorValue(v: unknown): string {
  if (Array.isArray(v)) {
    return String(v[0] ?? '');
  }
  if (v && typeof v === 'object') {
    // If nested object, stringify first-level values
    try {
      return JSON.stringify(v);
    } catch {
      return String(v);
    }
  }
  return String(v ?? '');
}

export function toCommandResult<T>(result: ApiResult<T>): CommandResult {
  if (result.ok) return { success: true };

  // Validation errors from server: map to field-level errors (strings)
  if (result.error?.errors && typeof result.error.errors === 'object') {
    const errors = Object.fromEntries(
      Object.entries(result.error.errors).map(([k, v]) => [k, normalizeErrorValue(v)])
    );
    return { success: false, errors };
  }

  // HTTP errors with a message -> form-level error
  if (result.error?.type === 'http' && result.error?.message) {
    return { success: false, errors: { form: normalizeErrorValue(result.error.message) } };
  }

  // Network errors -> generic form-level message
  if (result.error?.type === 'network') {
    return { success: false, errors: { form: 'A network error occurred. Please try again.' } };
  }

  // Fallback: if server returned a message field, use it as form-level error
  if (result.error?.message) {
    return { success: false, errors: { form: normalizeErrorValue(result.error.message) } };
  }

  // Final fallback
  return { success: false, errors: { form: 'A network error occurred. Please try again.' } };
}
