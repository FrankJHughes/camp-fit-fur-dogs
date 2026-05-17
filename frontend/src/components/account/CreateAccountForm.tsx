'use client';

import type { CreateAccountCommand } from '@/api/account/createAccount';
import { AccountForm } from '@/components/account/AccountForm';

interface CreateAccountFormProps {
  command: {
    submit: (data: CreateAccountCommand) => void;
    errors?: Record<string, string>;
    isSubmitting?: boolean;
  };
}

export function CreateAccountForm({ command }: CreateAccountFormProps) {
  return (
    <AccountForm
      title="Create Account"
      submitLabel="Create Account"
      onSubmit={command.submit}
      errors={command.errors}
      isSubmitting={command.isSubmitting}
    />
  );
}
