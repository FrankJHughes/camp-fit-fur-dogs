'use client';

import { useRouter } from 'next/navigation';
import { RegisterDogForm } from '@/components/dogs/RegisterDogForm';
import { registerDog, type RegisterDogCommand } from '@/api/dogs/registerDog';
import { useCommand } from '@/lib/hooks/useCommand';

export default function RegisterDogPage() {
  const router = useRouter();

  const { errors, isSubmitting, handleSubmit } = useCommand<RegisterDogCommand>(
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
