'use client';

import type { RegisterDogCommand } from '@/api/dogs/registerDog';
import { DogForm } from '@/components/dogs/DogForm';

interface RegisterDogFormProps {
  command: {
    submit: (data: RegisterDogCommand) => void;
    errors?: Record<string, string>;
    isSubmitting?: boolean;
  };
}

export function RegisterDogForm({ command }: RegisterDogFormProps) {
  return (
    <DogForm
      title="Register Dog"
      submitLabel="Register"
      onSubmit={command.submit}
      errors={command.errors}
      isSubmitting={command.isSubmitting}
    />
  );
}
