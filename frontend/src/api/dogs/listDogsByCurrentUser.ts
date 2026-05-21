import { createApiClient } from '@/lib/api/client';
import type { QueryResult } from '@/lib/api/queryResult';
import type { ListDogsByCurrentUserResponse } from '@/lib/dogs/dogModel';

const client = createApiClient();

export async function listDogsByCurrentUser(): Promise<QueryResult<ListDogsByCurrentUserResponse>> {
  try {
    const result = await client.get<ListDogsByCurrentUserResponse>('/dogs');

    if (result.ok) {
      return { success: true, data: result.data };
    }

    if (result.error?.status === 404) {
      return { success: false, notFound: true };
    }

    return {
      success: false,
      notFound: false,
      error: result.error?.message ?? 'An unknown error occurred.',
    };
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('listDogsByCurrentUser error', err);
    return {
      success: false,
      notFound: false,
      error: err?.message ?? 'Network error',
    };
  }
}
