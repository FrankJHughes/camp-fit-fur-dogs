import { createApiClient } from '@/lib/api/client';
import { toCommandResult, type CommandResult } from '@/lib/api/commandResult';

export interface DogFormData {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: string;
}

const client = createApiClient();

export async function registerDog(data: DogFormData): Promise<CommandResult> {
  const result = await client.post<void>('/dogs/register', data);
  return toCommandResult(result);
}