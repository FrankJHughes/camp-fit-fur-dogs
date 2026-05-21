import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';
import type { DogProfile } from '@/lib/dogs/dogModel';

const client = createApiClient();

export async function getDogProfile(
  dogId: string
): Promise<QueryResult<DogProfile>> {
  try {
    const result = await client.get<DogProfile>(`/dogs/${dogId}`);

    if (result.ok) {
      return { success: true, data: result.data };
    }

    // If the client provides an error object with status, handle 404 explicitly
    if (result.error?.status === 404) {
      return { success: false, notFound: true };
    }

    return {
      success: false,
      notFound: false,
      error: result.error?.message ?? 'Unexpected server error',
    };
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('getDogProfile error', err);
    return {
      success: false,
      notFound: false,
      error: err?.message ?? 'Network error',
    };
  }
}
