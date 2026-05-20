// src/api/account/createAccount.ts
import type { CreateAccountCommand } from '@/api/account/types';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';

const client = createApiClient();

export async function createAccount(payload: CreateAccountCommand) {
  // Tests expect this endpoint
  const res = await client.post('/api/customers', payload);
  return toCommandResult(res);
}
