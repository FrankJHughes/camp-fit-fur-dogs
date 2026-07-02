'use client';

import React from 'react';
import RegisterDogForm from '@/components/dogs/RegisterDogForm';
import { useRegisterDog } from '@/lib/dogs/useRegisterDog';

export default function RegisterDogPage() {
  const { command } = useRegisterDog();

  return (
    <main className="page-container">
      <h1 className="page-title">Register a Dog</h1>
      <RegisterDogForm command={command} />
    </main>
  );
}
