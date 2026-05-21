import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';

const client = createApiClient();

export async function removeDog(dogId: string): Promise<CommandResult> {
  try {
    const result = await client.delete<void>(`/dogs/${dogId}`);
    return toCommandResult(result);
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('removeDog error', err);
    return { success: false, error: err?.message ?? 'Network error' };
  }
}
