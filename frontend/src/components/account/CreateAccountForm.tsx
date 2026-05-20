'use client';

import { AccountForm } from '@/components/account/AccountForm';
import type { CreateAccountCommand } from '@/api/account/createAccount';
import type { CreateAccountValues } from '@/lib/account/createAccountSchema';

interface CreateAccountFormProps {
  command: {
    submit: (data: CreateAccountCommand) => void;
    errors?: Record<string, string>;
    isSubmitting?: boolean;
  };
}

export function CreateAccountForm({ command }: CreateAccountFormProps) {
  const handleSubmit = (values: CreateAccountValues) => {
    // Strip frontend-only field
    const { confirmPassword, ...rest } = values;

    // Convert form values → API command shape
    const cmd: CreateAccountCommand = {
      firstName: rest.firstName,
      lastName: rest.lastName,
      email: rest.email,
      phone: rest.phone,
      password: rest.password,
    };

    command.submit(cmd);
  };

  return (
    <AccountForm
      title="Create Account"
      submitLabel="Create Account"
      onSubmit={handleSubmit}
      errors={command.errors}
      isSubmitting={command.isSubmitting}
    />
  );
}
