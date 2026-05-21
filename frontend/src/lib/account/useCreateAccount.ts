'use client';

import { useRouter } from 'next/navigation';
import { createAccount as createAccountApi } from '@/api/account/createAccount';
import { useFormCommand } from '@/lib/forms/useFormCommand';
import type { FormCommand } from '@/lib/forms/formCommand';
import type { CreateAccountCommand } from '@/api/account/types';
import type { CreateAccountValues } from './createAccountSchema';
import { mapCreateAccountValuesToCommand } from './createAccountModel';

export function useCreateAccount(): { command: FormCommand<CreateAccountValues> } {
  const router = useRouter();

  const command = useFormCommand<CreateAccountValues>({
    run: (values: CreateAccountValues) => {
      try {
        const payload: CreateAccountCommand = mapCreateAccountValuesToCommand(values);
        return createAccountApi(payload);
      } catch (err) {
        // If mapping throws, return a CommandResult-like object
        // so useFormCommand can surface the error. Adjust shape to your CommandResult.
        return Promise.resolve({ success: false, error: 'Invalid form data' } as any);
      }
    },
    onSuccess: () => {
      router.push('/create-account/success');
    },
  });

  return { command };
}
