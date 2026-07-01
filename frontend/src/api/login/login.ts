import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';
import { LoginRequest } from '@/lib/login/loginModel';

const client = createApiClient();

export async function login(loginRequest: LoginRequest = {}): Promise<CommandResult> {
  try {
    const result = await client.post<void>('/auth/login', loginRequest);
    return toCommandResult(result);
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('login error', err);
    return { success: false, error: err?.message ?? 'Network error' };
  }
}
