// src/api/dogs/editDogProfile.ts
import type { EditDogProfileCommand } from '@/lib/dogs/dogModel';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';

const client = createApiClient();

export async function editDogProfile(
  dogId: string,
  data: EditDogProfileCommand
): Promise<CommandResult> {
  // Tests expect the client to be called with /dogs/:id
  const result = await client.put<void>(`/dogs/${dogId}`, data);
  return toCommandResult(result);
}
