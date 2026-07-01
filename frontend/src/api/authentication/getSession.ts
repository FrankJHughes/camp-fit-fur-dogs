import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';
import { GetSessionResponse } from '@/lib/authentication/getSessionResponse';

const client = createApiClient();

export async function getSession(): Promise<QueryResult<GetSessionResponse>> {
  const result = await client.get<GetSessionResponse>(`/api/auth/session`);
  if (result.ok) {
    return { success: true, data: result.data };
  }
  if (result.error.status === 404) {
    return { success: false, notFound: true };
  }
  return { success: false, notFound: false, error: result.error.message };
}
