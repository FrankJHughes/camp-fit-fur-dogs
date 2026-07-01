import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';
import { LogoutRequest } from '@/lib/logout/logoutModel';

const client = createApiClient();

export async function logout(logoutRequest: LogoutRequest = {}): Promise<CommandResult> {
  try {
    const result = await client.post<void>(`/auth/logout`, logoutRequest);
    return toCommandResult(result);
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('logout error', err);
    return { success: false, error: err?.message ?? 'Network error' };
  }
}
