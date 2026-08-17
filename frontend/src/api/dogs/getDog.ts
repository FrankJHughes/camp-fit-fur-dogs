import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';
import type { Dog } from '@/lib/dogs/dogModel';

const client = createApiClient();

export async function getDog(
  dogId: string
): Promise<QueryResult<Dog>> {
  try {
    const result = await client.get<Dog>(`/dogs/${dogId}`);

    if (result.ok) {
      return { success: true, data: result.data };
    }

    // If the client provides an error object with status, handle 404 explicitly
    if (result.error?.status === 404) {
      return { success: false, notFound: true };
    }

    return {
      success: false,
      error: result.error?.message ?? 'Unexpected server error',
    };
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('getDog error', err);
    return {
      success: false,
      error: err?.message ?? 'Network error',
    };
  }
}
