'use client';
import type { EditDogProfileData } from '@/api/dogs/editDogProfile';
import { DogForm } from '@/components/dogs/DogForm';

interface EditDogProfileFormProps {
  initialData: EditDogProfileData;
  onSubmit: (data: EditDogProfileData) => void;
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