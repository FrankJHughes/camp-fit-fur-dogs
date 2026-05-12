'use client';

import { useRouter } from 'next/navigation';
import { RegisterDogForm } from '@/components/dogs/RegisterDogForm';
import type { RegisterDogCommand } from '@/api/dogs/registerDog';
import { useApiCommand } from '@/lib/hooks/useApiCommand';

export default function RegisterDogPage() {
  const router = useRouter();

  const command = useApiCommand<RegisterDogCommand>(
    '/dogs/register',
    () => router.push('/dogs/register/success')
  );

  return <RegisterDogForm command={command} />;
}
