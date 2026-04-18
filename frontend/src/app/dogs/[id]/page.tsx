'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import { getDogProfile, type GetDogProfileResult } from '@/api/getDogProfile';
import { DogProfileCard } from '@/components/DogProfileCard';

export default function ViewDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const [result, setResult] = useState<GetDogProfileResult | null>(null);

  useEffect(() => {
    getDogProfile(id).then(setResult);
  }, [id]);

  if (result === null) {
    return <p>Loading…</p>;
  }

  if (result.success) {
    return <DogProfileCard profile={result.profile} />;
  }

  if (result.notFound) {
    return <p>Dog not found.</p>;
  }

  return <p>{result.error}</p>;
}