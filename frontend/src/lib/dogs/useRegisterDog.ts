'use client';

import { useRouter } from 'next/navigation';
import { registerDog } from '@/api/dogs/registerDog';
import type RegisterDogCommand from '@/lib/dogs/dogModel';
import { useFormCommand } from '@/lib/forms/useFormCommand';

export function useRegisterDog() {
  const router = useRouter();

  const command = useFormCommand<RegisterDogCommand>({
    run: (values) => {
      return registerDog(values);
    },
    onSuccess: () => router.push('/dogs/register/success'),
  });

  return { command };
}
