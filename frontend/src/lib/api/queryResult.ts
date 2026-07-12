import type { QueryState } from '@/lib/hooks/useApiQuery';

export type QueryResult<T> =
  | { success: true; data: T }
  | { success: false; unauthorized: true }
  | { success: false; notFound: true }
  | { success: false; error: string };

export function toQueryState<T>(result: QueryResult<T>): QueryState<T> {
  // Success case
  if (result.success) {
    return { status: 'success', data: result.data };
  }

  // Unauthorized (401)
  if ((result as any).unauthorized === true) {
    return { status: 'unauthenticated' };
  }

  // Not found (404)
  if ((result as any).notFound === true) {
    return { status: 'not-found' };
  }

  // Error (500, network, etc.)
  const errorMessage =
    typeof (result as any).error === 'string' && (result as any).error.length > 0
      ? (result as any).error
      : 'An unknown error occurred.';

  return {
    status: 'error',
    error: errorMessage,
  };
}
