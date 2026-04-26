import { createApiClient } from '@/lib/api/client';
import { toCommandResult, type CommandResult } from '@/lib/api/commandResult';

export interface EditDogProfileCommand {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: string;
}

const client = createApiClient();

export async function editDogProfile(
  dogId: string,
  data: EditDogProfileCommand
): Promise<CommandResult> {
  const result = await client.put<void>(`/dogs/${dogId}`, data);
  return toCommandResult(result);
}
