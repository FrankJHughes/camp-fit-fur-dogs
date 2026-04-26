'use client';
import type { EditDogProfileCommand } from '@/api/dogs/editDogProfile';
import { DogForm } from '@/components/dogs/DogForm';

interface EditDogProfileFormProps {
  initialData: EditDogProfileCommand;
  onSubmit: (data: EditDogProfileCommand) => void;
  errors?: Record<string, string>;
  isSubmitting?: boolean;
}

export function EditDogProfileForm({ initialData, onSubmit, errors, isSubmitting }: EditDogProfileFormProps) {
  return (
    <DogForm
      title="Edit Dog Profile"
      submitLabel="Save"
      initialValues={initialData}
      onSubmit={onSubmit}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
}
