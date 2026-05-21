// src/api/account/createAccount.ts
import type { CreateAccountCommand } from '@/api/account/types';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';

const client = createApiClient();

export async function createAccount(payload: CreateAccountCommand): Promise<CommandResult> {
  try {
    // Tests expect this endpoint
    const res = await client.post('/api/customers', payload);
    return toCommandResult(res);
  } catch (err: any) {
    // eslint-disable-next-line no-console
    console.error('createAccount error', err);
    return { success: false, error: err?.message ?? 'Network error' };
  }
}
