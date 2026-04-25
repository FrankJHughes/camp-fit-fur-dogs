'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { getDogProfile, type GetDogProfileResult } from '@/api/getDogProfile';
import { DogProfileCard } from '@/components/DogProfileCard';
import DogProfileActionsCard from '@/components/dogs/DogProfileActionsCard';
import { getDogProfileActions } from '@/lib/dogs/dogProfileActions';

export default function ViewDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [result, setResult] = useState<GetDogProfileResult | null>(null);

  useEffect(() => {
    getDogProfile(id).then(setResult);
  }, [id]);

  if (result === null) {
    return <p>Loading…</p>;
  }

  if (result.success) {
    const actions = getDogProfileActions(result.profile.id, router.push);

    return (
      <>
        <DogProfileCard
          profile={result.profile}
        />
        <DogProfileActionsCard actions={actions} />
      </>
    );
  }

  if (result.notFound) {
    return <p>Dog not found.</p>;
  }

  return <p>{result.error}</p>;
}
