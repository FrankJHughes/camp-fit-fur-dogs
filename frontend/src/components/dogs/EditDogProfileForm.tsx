'use client';
import type { EditDogProfileCommand } from '@/api/dogs/editDogProfile';
import { DogForm } from '@/components/dogs/DogForm';
import { DogFormValues } from '@/lib/dogs/DogFormSchema';

interface EditDogProfileFormProps {
  initialData: EditDogProfileCommand;
  onSubmit: (data: EditDogProfileCommand) => void;
  errors?: Record<string, string>;
  isSubmitting?: boolean;
}

export function EditDogProfileForm({ initialData, onSubmit, errors, isSubmitting }: EditDogProfileFormProps) {
  const normalizedInitialValues: DogFormValues = {
    name: initialData.name,
    breed: initialData.breed,
    dateOfBirth: initialData.dateOfBirth,
    sex:
      initialData.sex === 'Male' || initialData.sex === 'Female'
        ? initialData.sex
        : '',
  };

  return (
    <DogForm
      title="Edit Dog Profile"
      submitLabel="Save"
      initialValues={normalizedInitialValues}
      onSubmit={onSubmit}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
}
