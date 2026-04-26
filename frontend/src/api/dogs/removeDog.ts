import { createApiClient } from '@/lib/api/client';
import { toCommandResult, type CommandResult } from '@/lib/api/commandResult';

const client = createApiClient();

export async function removeDog(dogId: string): Promise<CommandResult> {
    const result = await client.delete<void>(`/dogs/${dogId}`);
    return toCommandResult(result);
}
