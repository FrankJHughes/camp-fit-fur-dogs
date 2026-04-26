import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';

export interface DogProfile {
  id: string;
  ownerId: string;
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: string;
}

const client = createApiClient();

export async function getDogProfile(dogId: string): Promise<QueryResult<DogProfile>> {
  const result = await client.get<DogProfile>(`/dogs/${dogId}`);
  if (result.ok) {
    return { success: true, data: result.data };
  }
  if (result.error.status === 404) {
    return { success: false, notFound: true };
  }
  return { success: false, notFound: false, error: result.error.message };
}