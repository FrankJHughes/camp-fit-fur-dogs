import { createApiClient } from '@/lib/api/client';
import { QueryResult } from '@/lib/api/queryResult';

const client = createApiClient();

export interface LogoutResponse { }

export async function logout(returnUrl: string): Promise<QueryResult<LogoutResponse>> {
  try {
    const urlSearchParams = new URLSearchParams();
    urlSearchParams.set('return_url', returnUrl);

    const url = '/auth/logout?' + urlSearchParams.toString();

    const result = await client.get<LogoutResponse>(url);

    if (result.ok) {
      return { success: true, data: result.data };
    }

    // 404 → not found
    if (result.error.status === 404) {
      return { success: false, notFound: true };
    }

    // 401 → unauthorized
    if (result.error.status === 401) {
      return { success: false, unauthorized: true };
    }

    // All other errors → error
    return {
      success: false,
      error: result.error.message ?? 'An unknown error occurred.',
    };
  } catch (err: any) {
    return {
      success: false,
      error: err?.message ?? 'An unknown error occurred.',
    };
  }
}
