'use client';
import { useState } from 'react';
import { FieldError } from '../shared/FieldError';
import { FormField } from '../shared/FormField';
import { validateDogForm } from '../../lib/dogs/validateDogForm';
import type { DogFormValues } from '../../types/dog';

export type { DogFormValues };

interface DogFormProps {
  title: string;
  submitLabel: string;
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
  title,
  submitLabel,
  initialValues = emptyValues,
  onSubmit,
  errors,
  isSubmitting,
}: DogFormProps) {
  const [values, setValues] = useState<DogFormValues>(initialValues);
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

  const displayErrors = { ...validationErrors, ...errors };

  const update = (field: keyof DogFormValues) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
      setValues((prev) => ({ ...prev, [field]: e.target.value }));

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const newErrors = validateDogForm(values);

    if (Object.keys(newErrors).length > 0) {
      setValidationErrors(newErrors);
      return;
    }

    setValidationErrors({});
    onSubmit(values);
  };

  return (
    <form onSubmit={handleSubmit}>
      <h1>{title}</h1>

      <FieldError id="error-form" message={displayErrors.form} />

      <FormField label="Name" name="name" error={displayErrors.name}>
        {(fieldProps) => (
          <input
            type="text"
            value={values.name}
            onChange={update('name')}
            {...fieldProps}
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
          />
        )}
      </FormField>

      <FormField label="Date of Birth" name="dateOfBirth" error={displayErrors.dateOfBirth}>
        {(fieldProps) => (
          <input
            type="text"
            value={values.dateOfBirth}
            onChange={update('dateOfBirth')}
            {...fieldProps}
          />
        )}
      </FormField>

      <FormField label="Sex" name="sex" error={displayErrors.sex}>
        {(fieldProps) => (
          <select
            value={values.sex}
            onChange={update('sex')}
            {...fieldProps}
          >
            <option value="">Select</option>
            <option value="Male">Male</option>
            <option value="Female">Female</option>
          </select>
        )}
      </FormField>

      <button type="submit" disabled={isSubmitting}>{submitLabel}</button>
    </form>
  );
}
