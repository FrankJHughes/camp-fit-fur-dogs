'use client';

import { useRouter } from 'next/navigation';
import { RegisterDogForm } from '@/components/dogs/RegisterDogForm';
import { registerDog, type DogFormData } from '@/api/dogs/registerDog';
import { useCommand } from '@/lib/hooks/useCommand';

export default function RegisterDogPage() {
  const router = useRouter();

  const { errors, isSubmitting, handleSubmit } = useCommand<DogFormData>(
    (data) => registerDog(data),
    () => router.push('/dogs/register/success')
  );

  return (
    <RegisterDogForm
      onSubmit={handleSubmit}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
}