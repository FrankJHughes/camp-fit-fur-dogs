// src/lib/account/useCreateAccount.ts
'use client';

import { useRouter } from 'next/navigation';
import { createAccount as createAccountApi } from '@/api/account/createAccount';
import { useFormCommand } from '@/lib/forms/useFormCommand';
import type { FormCommand } from '@/lib/forms/formCommand';
import type { CreateAccountCommand } from '@/api/account/types'; // <- updated import
import type { CreateAccountValues } from './createAccountSchema';
import { mapCreateAccountValuesToCommand } from './createAccountModel';

export function useCreateAccount(): { command: FormCommand<CreateAccountValues> } {
  const router = useRouter();

  const command = useFormCommand<CreateAccountValues>({
    submit: async (values: CreateAccountValues) => {
      const payload: CreateAccountCommand = mapCreateAccountValuesToCommand(values);
      return await createAccountApi(payload);
    },
    onSuccess: () => {
      router.push('/create-account/success');
    },
  });

  return { command };
}
