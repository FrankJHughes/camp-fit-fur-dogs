'use client';

import React from 'react';
import { DogForm } from '@/components/dogs/DogForm';
import type { DogFormValues } from '@/lib/dogs/dogModel';
import type { FormCommand } from '@/lib/forms/formCommand';

interface EditDogFormProps {
  command: FormCommand<DogFormValues>;
  initialValues: DogFormValues;
}

function EditDogForm({ command, initialValues }: EditDogFormProps) {
  return (
    <DogForm
      title="Edit Dog Profile"
      submitLabel="Save Changes"
      command={command}
      initialValues={initialValues}
    />
  );
}

export default React.memo(EditDogForm);
