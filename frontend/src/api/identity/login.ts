import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

const client = createApiClient();

export interface LoginResponse {
  nextUrl: string;
}

export async function login(returnUrl: string): Promise<QueryResult<LoginResponse>> {
  const urlSearchParams = new URLSearchParams();
  urlSearchParams.set('return_url', returnUrl);

  const url = '/identity/login-url?' + urlSearchParams.toString();

  const result = await client.get<LoginResponse>(url);

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
