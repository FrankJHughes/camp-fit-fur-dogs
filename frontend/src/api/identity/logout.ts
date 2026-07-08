import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

const client = createApiClient();

export interface LogoutResponse {
  nextUrl: string;
}

export async function logout(returnUrl: string): Promise<QueryResult<LogoutResponse>> {
  const urlSearchParams = new URLSearchParams();
  urlSearchParams.set('return_url', returnUrl);

  const url = '/api/identity/logout?' + urlSearchParams.toString();

  const result = await client.get<LogoutResponse>(url);

  if (result.ok) {
    return { success: true, data: result.data };
  }

  if (result.error.status === 401) {
    return { success: false, unauthorized: true };
  }

  if (result.error.status === 404) {
    return { success: false, notFound: true };
  }

  return { success: false, error: result.error.message };
}
