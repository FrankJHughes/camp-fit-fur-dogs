import type { EditDogProfileCommand } from '@/lib/dogs/dogModel';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';

const client = createApiClient();

export async function editDogProfile(
  dogId: string,
  data: EditDogProfileCommand
): Promise<CommandResult> {
  try {
    // Tests expect the client to be called with /dogs/:id
    const result = await client.put<void>(`/dogs/${dogId}`, data);
    return toCommandResult(result);
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('editDogProfile error', err);
    // Return a CommandResult-shaped error so callers can handle it uniformly
    return { success: false, error: err?.message ?? 'Network error' };
  }
}
