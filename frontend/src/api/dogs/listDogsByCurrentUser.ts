import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

interface DogListItem {
  id: string;
  name: string;
  breed: string;
}

export interface ListDogsByCurrentUserResponse {
  dogs: DogListItem[];
}

const client = createApiClient();

export async function listDogsByCurrentUser(): Promise<QueryResult<ListDogsByCurrentUserResponse>> {
  const result = await client.get<ListDogsByCurrentUserResponse>('/dogs');
  if (result.ok) {
    return { success: true, data: result.data };
  }
  return {
    success: false,
    notFound: false,
    error: result.error.message,
  };
}
