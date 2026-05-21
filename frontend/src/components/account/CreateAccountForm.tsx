'use client';

import React from 'react';
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

    try {
      await command.run(payload as any);
      // command.run is expected to set command.errors/command.error when appropriate
      // and useFormCommand / useApiCommand will call onSuccess when appropriate.
    } catch (err) {
      // Log and surface a generic form-level error via the command state machine if needed.
      // eslint-disable-next-line no-console
      console.error(err);
      // Returning here lets the parent form state machine show a generic error if desired.
      return;
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

export default React.memo(CreateAccountForm);
