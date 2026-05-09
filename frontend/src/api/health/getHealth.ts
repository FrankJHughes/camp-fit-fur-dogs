import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

export interface GetHealthQuery {
  status: string;
}

const client = createApiClient();

export async function getHealth(): Promise<QueryResult<GetHealthQuery>> {
  const result = await client.get<GetHealthQuery>(`/health}`);
  if (result.ok) {
    return { success: true, data: result.data };
  }
  if (result.error.status === 404) {
    return { success: false, notFound: true };
  }
  return { success: false, notFound: false, error: result.error.message };
}
