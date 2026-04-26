'use client';
import type { RegisterDogCommand } from '@/api/dogs/registerDog';
import { DogForm } from '@/components/dogs/DogForm';

interface RegisterDogFormProps {
  onSubmit: (data: RegisterDogCommand) => void;
  errors?: Record<string, string>;
  isSubmitting?: boolean;
}

export function RegisterDogForm({ onSubmit, errors, isSubmitting }: RegisterDogFormProps) {
  return (
    <DogForm
      title="Register Dog"
      submitLabel="Register"
      onSubmit={onSubmit}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
}
