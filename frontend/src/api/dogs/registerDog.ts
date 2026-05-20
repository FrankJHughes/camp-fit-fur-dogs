import { createApiClient } from '@/lib/api/client';
import { type CommandResult } from '@/lib/api/commandResult';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { RegisterDogCommand } from '@/lib/dogs/dogModel';

const client = createApiClient();

export async function registerDog(
  data: RegisterDogCommand
): Promise<CommandResult> {
  const result = await client.post<void>('/dogs/register', data);
  return toCommandResult(result);
}
