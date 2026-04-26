'use client';
import { useState } from 'react';
import { FieldError } from '../shared/FieldError';
import { FormField } from '../shared/FormField';
import { validateDogForm } from '../../lib/validateDogForm';

export interface DogFormValues {
  name: string;
  breed: string;
  dateOfBirth: string;
  sex: string;
}

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
  const [name, setName] = useState(initialValues.name);
  const [breed, setBreed] = useState(initialValues.breed);
  const [dateOfBirth, setDateOfBirth] = useState(initialValues.dateOfBirth);
  const [sex, setSex] = useState(initialValues.sex);
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

  const displayErrors = { ...validationErrors, ...errors };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const newErrors = validateDogForm({ name, breed, dateOfBirth, sex });

    if (Object.keys(newErrors).length > 0) {
      setValidationErrors(newErrors);
      return;
    }

    setValidationErrors({});
    onSubmit({ name, breed, dateOfBirth, sex });
  };

  return (
    <form onSubmit={handleSubmit}>
      <h1>{title}</h1>

      <FieldError id="error-form" message={displayErrors.form} />

      <FormField label="Name" name="name" error={displayErrors.name}>
        {(fieldProps) => (
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            {...fieldProps}
          />
        )}
      </FormField>

      <FormField label="Breed" name="breed" error={displayErrors.breed}>
        {(fieldProps) => (
          <input
            type="text"
            value={breed}
            onChange={(e) => setBreed(e.target.value)}
            {...fieldProps}
          />
        )}
      </FormField>

      <FormField label="Date of Birth" name="dateOfBirth" error={displayErrors.dateOfBirth}>
        {(fieldProps) => (
          <input
            type="text"
            value={dateOfBirth}
            onChange={(e) => setDateOfBirth(e.target.value)}
            {...fieldProps}
          />
        )}
      </FormField>

      <FormField label="Sex" name="sex" error={displayErrors.sex}>
        {(fieldProps) => (
          <select
            value={sex}
            onChange={(e) => setSex(e.target.value)}
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