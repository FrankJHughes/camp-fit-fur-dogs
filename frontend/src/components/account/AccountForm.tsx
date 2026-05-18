'use client';

import { useState } from 'react';
import { FieldError } from '@/lib/components/FieldError';
import { FormField } from '@/lib/components/FormField';
import { useFormErrors } from '@/lib/hooks/useFormErrors';
import { validateAccountForm } from '@/lib/account/validateAccountForm';
import { CreateAccountValues } from '@/lib/account/createAccountSchema';

interface AccountFormProps {
  title: string;
  submitLabel: string;
  initialValues?: CreateAccountValues;
  onSubmit: (data: CreateAccountValues) => void;
  errors?: Record<string, string>;
  isSubmitting?: boolean;
}

const emptyValues: CreateAccountValues = {
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
  const [values, setValues] = useState<CreateAccountValues>(initialValues);

  const { setClient, merge, clear } = useFormErrors();
  const displayErrors = merge(errors);

  const update =
    (field: keyof CreateAccountValues) =>
      (e: React.ChangeEvent<HTMLInputElement>) =>
        setValues((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    // 1. Client-side validation via Zod
    const clientErrors = validateAccountForm(values);

    if (Object.keys(clientErrors).length > 0) {
      setClient(clientErrors);
      return;
    }

    // 2. Clear client errors before submitting
    clear();

    // 3. Submit to parent (API command)
    onSubmit(values);
  };

  return (
    <form onSubmit={handleSubmit} noValidate>
      <h1>{title}</h1>

      {/* Form-level error (e.g., server error) */}
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
