import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

const client = createApiClient();

export interface GetIdentityResponse {
  name: string;
}

export async function getIdentity(): Promise<QueryResult<GetIdentityResponse>> {
  const result = await client.get<GetIdentityResponse>(`/api/identity`);

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
