// /api/account/createAccount.ts

import { z } from 'zod';

// This mirrors the Zod schema in AccountForm
export const CreateAccountCommandSchema = z.object({
  email: z.string(),
  password: z.string(),
  confirmPassword: z.string(),
});

export type CreateAccountCommand = z.infer<typeof CreateAccountCommandSchema>;

export async function createAccount(cmd: CreateAccountCommand) {
  const res = await fetch('/owners/create-account', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(cmd),
  });

  const json = await res.json();

  if (!res.ok) {
    return {
      ok: false as const,
      error: json,
    };
  }

  return {
    ok: true as const,
    data: json,
  };
}
