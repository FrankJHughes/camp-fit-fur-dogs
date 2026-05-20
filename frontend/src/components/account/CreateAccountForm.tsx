// src/components/account/CreateAccountForm.tsx
'use client';

import { AccountForm } from '@/components/account/AccountForm';
import type { CreateAccountValues } from '@/lib/account/createAccountSchema';
import type { FormCommand } from '@/lib/forms/formCommand';

interface CreateAccountFormProps {
  command: FormCommand<CreateAccountValues>;
}

export function CreateAccountForm({ command }: CreateAccountFormProps) {
  // Adapt CreateAccountValues into the command payload (exclude confirmPassword)
  const handleSubmit = async (values: CreateAccountValues) => {
    const payload = {
      firstName: values.firstName,
      lastName: values.lastName,
      email: values.email,
      phone: values.phone,
      password: values.password,
    };

    const anyCmd = command as any;

    if (typeof anyCmd.run === 'function') {
      await anyCmd.run(payload);
    } else if (typeof anyCmd.submit === 'function') {
      await anyCmd.submit(payload);
    } else if (typeof anyCmd.execute === 'function') {
      await anyCmd.execute(payload);
    } else {
      throw new Error('Form command is missing run/submit/execute function');
    }
  };

  // Merge form-level error into the external errors object under the "form" key
  const externalErrors = {
    ...(command.errors ?? {}),
    ...(command.error ? { form: command.error } : {}),
  };

  return (
    <AccountForm
      title="Create Account"
      submitLabel="Create Account"
      onSubmit={handleSubmit}
      errors={externalErrors}
      isSubmitting={command.isSubmitting}
    />
  );
}

export default CreateAccountForm;
