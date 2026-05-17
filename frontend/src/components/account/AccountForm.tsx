'use client';

import { useState } from 'react';
import { FieldError } from '@/lib/components/FieldError';
import { FormField } from '@/lib/components/FormField';
import { useFormErrors } from '@/lib/hooks/useFormErrors';
import {
  validateAccountForm,
  type AccountFormValues,
} from '@/lib/account/validateAccountForm';

export type { AccountFormValues };

interface AccountFormProps {
  title: string;
  submitLabel: string;
  initialValues?: AccountFormValues;
  onSubmit: (data: AccountFormValues) => void;
  errors?: Record<string, string>;
  isSubmitting?: boolean;
}

const emptyValues: AccountFormValues = {
  email: '',
  password: '',
  confirmPassword: '',
};

export function AccountForm({
  title,
  submitLabel,
  initialValues = emptyValues,
  onSubmit,
  errors,
  isSubmitting,
}: AccountFormProps) {
  const [values, setValues] = useState<AccountFormValues>(initialValues);

  const { setClient, merge, clear } = useFormErrors();
  const displayErrors = merge(errors);

  const update =
    (field: keyof AccountFormValues) =>
      (e: React.ChangeEvent<HTMLInputElement>) =>
        setValues((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const clientErrors = validateAccountForm(values);

    if (Object.keys(clientErrors).length > 0) {
      setClient(clientErrors);
      return;
    }

    clear();
    onSubmit(values);
  };

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>{title}</h1>

      <FieldError id="error-form" message={displayErrors.form} />

      <FormField label="Email" name="email" error={displayErrors.email}>
        {(fieldProps) => (
          <input
            type="email"
            value={values.email}
            onChange={update('email')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label="Password"
        name="password"
        error={displayErrors.password}
      >
        {(fieldProps) => (
          <input
            type="password"
            value={values.password}
            onChange={update('password')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label="Confirm Password"
        name="confirmPassword"
        error={displayErrors.confirmPassword}
      >
        {(fieldProps) => (
          <input
            type="password"
            value={values.confirmPassword}
            onChange={update('confirmPassword')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <button type="submit" disabled={isSubmitting}>
        {submitLabel}
      </button>
    </form>
  );
}
