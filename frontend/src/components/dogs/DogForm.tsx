'use client';

import { useState } from 'react';
import { FormField } from '@/lib/components/FormField';
import { FieldError } from '@/lib/components/FieldError';
import { useFormErrors } from '@/lib/hooks/useFormErrors';
import { validateDogForm } from '@/lib/dogs/validateDogForm';
import { DogFormValues } from '@/lib/dogs/DogFormSchema';

interface DogFormProps {
  title?: string;
  submitLabel?: string;
  initialValues?: DogFormValues;
  onSubmit: (data: DogFormValues) => void;
  errors?: Record<string, string>;
  isSubmitting?: boolean;
}

const emptyValues: DogFormValues = {
  name: '',
  breed: '',
  dateOfBirth: '',
  sex: '',
};

export function DogForm({
  title = 'Register Dog',
  submitLabel = 'Register Dog',
  initialValues = emptyValues,
  onSubmit,
  errors,
  isSubmitting,
}: DogFormProps) {
  const [values, setValues] = useState<DogFormValues>(initialValues);

  const { setClient, merge, clear } = useFormErrors();
  const displayErrors = merge(errors);

  const update =
    (field: keyof DogFormValues) =>
      (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
        setValues((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const clientErrors = validateDogForm(values);

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

      <FormField label="Name" name="name" error={displayErrors.name}>
        {(fieldProps) => (
          <input
            type="text"
            value={values.name}
            onChange={update('name')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField label="Breed" name="breed" error={displayErrors.breed}>
        {(fieldProps) => (
          <input
            type="text"
            value={values.breed}
            onChange={update('breed')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField
        label="Date of Birth"
        name="dateOfBirth"
        error={displayErrors.dateOfBirth}
      >
        {(fieldProps) => (
          <input
            type="date"
            value={values.dateOfBirth}
            onChange={update('dateOfBirth')}
            {...fieldProps}
            disabled={isSubmitting}
          />
        )}
      </FormField>

      <FormField label="Sex" name="sex" error={displayErrors.sex}>
        {(fieldProps) => (
          <select
            value={values.sex}
            onChange={update('sex')}
            {...fieldProps}
            disabled={isSubmitting}
          >
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
        )}
      </FormField>

      <button type="submit" disabled={isSubmitting}>
        {submitLabel}
      </button>
    </form>
  );
}
