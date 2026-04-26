import type { QueryState } from '@/lib/hooks/useApiQuery';

export type QueryResult<T> =
  | { success: true; data: T }
  | { success: false; notFound: boolean; error?: string };

export function toQueryState<T>(result: QueryResult<T>): QueryState<T> {
  if (result.success) return { status: 'success', data: result.data };
  if (result.notFound) return { status: 'not-found' };
  return { status: 'error', error: result.error ?? 'An unknown error occurred.' };
}