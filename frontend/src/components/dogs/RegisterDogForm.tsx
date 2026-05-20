'use client';

import { DogForm } from '@/components/dogs/DogForm';
import type { DogFormValues } from '@/lib/dogs/dogModel';
import type { FormCommand } from '@/lib/forms/formCommand';

interface RegisterDogFormProps {
  command: FormCommand<DogFormValues>;
}

export function RegisterDogForm({ command }: RegisterDogFormProps) {
  return (
    <DogForm
      title="Register Dog"
      submitLabel="Register Dog"
      command={command}
    />
  );
}
