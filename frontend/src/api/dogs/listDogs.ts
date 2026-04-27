import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

export interface DogSummary {
  id: string;
  name: string;
  breed: string;
}

export interface ListDogsResponse {
  dogs: DogSummary[];
}

const client = createApiClient();

export async function listDogs(): Promise<QueryResult<ListDogsResponse>> {
  const result = await client.get<ListDogsResponse>('/dogs');
  if (result.ok) {
    return { success: true, data: result.data };
  }
  return {
    success: false,
    notFound: false,
    error: result.error.message,
  };
}
