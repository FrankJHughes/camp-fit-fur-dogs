'use client';
import { useState } from 'react';

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

    const newErrors: Record<string, string> = {};
    if (!name.trim()) newErrors.name = 'Name is required';
    if (!breed.trim()) newErrors.breed = 'Breed is required';
    if (!dateOfBirth.trim()) newErrors.dateOfBirth = 'Date of birth is required';
    if (!sex) newErrors.sex = 'Sex is required';

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

      {displayErrors.form && <p>{displayErrors.form}</p>}

      <label>
        Name
        <input type="text" value={name} onChange={(e) => setName(e.target.value)} />
      </label>
      {displayErrors.name && <p>{displayErrors.name}</p>}

      <label>
        Breed
        <input type="text" value={breed} onChange={(e) => setBreed(e.target.value)} />
      </label>
      {displayErrors.breed && <p>{displayErrors.breed}</p>}

      <label>
        Date of Birth
        <input type="text" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} />
      </label>
      {displayErrors.dateOfBirth && <p>{displayErrors.dateOfBirth}</p>}

      <label>
        Sex
        <select value={sex} onChange={(e) => setSex(e.target.value)}>
          <option value="">Select</option>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </select>
      </label>
      {displayErrors.sex && <p>{displayErrors.sex}</p>}

      <button type="submit" disabled={isSubmitting}>{submitLabel}</button>
    </form>
  );
}