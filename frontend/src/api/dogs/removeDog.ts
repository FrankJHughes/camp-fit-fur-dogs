import { createApiClient } from '@/lib/api/client';
import { type CommandResult } from '@/lib/api/commandResult';
import { toCommandResult } from '@/lib/api/toCommandResult';

const client = createApiClient();

export async function removeDog(dogId: string): Promise<CommandResult> {
  const result = await client.delete<void>(`/dogs/${dogId}`);
  return toCommandResult(result);
}
