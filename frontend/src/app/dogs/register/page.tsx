'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { RegisterDogForm, type RegisterDogFormData } from '@/components/dogs/RegisterDogForm';
import { registerDog } from '@/api/dogs/registerDog';

export default function RegisterDogPage() {
  const router = useRouter();
  const [errors, setErrors] = useState<Record<string, string> | undefined>();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (data: RegisterDogFormData) => {
    setIsSubmitting(true);
    setErrors(undefined);

    const result = await registerDog(data);

    if (result.success) {
      router.push('/dogs/register/success');
    } else {
      setErrors(result.errors);
      setIsSubmitting(false);
    }
  };

  return (
    <RegisterDogForm
      onSubmit={handleSubmit}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
}
