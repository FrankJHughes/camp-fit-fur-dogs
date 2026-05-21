import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';
import type RegisterDogCommand from '@/lib/dogs/dogModel';

const client = createApiClient();

export async function registerDog(
  data: RegisterDogCommand
): Promise<CommandResult> {
  try {
    const result = await client.post<void>('/dogs/register', data);
    return toCommandResult(result);
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('registerDog error', err);
    return { success: false, error: err?.message ?? 'Network error' };
  }
}
