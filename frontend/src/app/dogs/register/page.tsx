'use client';

import { RegisterDogForm } from '@/components/dogs/RegisterDogForm';
import { useRegisterDog } from '@/lib/dogs/useRegisterDog';

export default function RegisterDogPage() {
  const { command } = useRegisterDog();
  return <RegisterDogForm command={command} />;
}
