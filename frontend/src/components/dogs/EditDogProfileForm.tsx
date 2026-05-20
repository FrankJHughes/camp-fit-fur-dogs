'use client';

import { DogForm } from '@/components/dogs/DogForm';
import type { DogFormValues } from '@/lib/dogs/dogModel';
import type { FormCommand } from '@/lib/forms/formCommand';

interface EditDogProfileFormProps {
  command: FormCommand<DogFormValues>;
  initialValues: DogFormValues;
}

export function EditDogProfileForm({ command, initialValues }: EditDogProfileFormProps) {
  return (
    <DogForm
      title="Edit Dog Profile"
      submitLabel="Save Changes"
      command={command}
      initialValues={initialValues}
    />
  );
}
