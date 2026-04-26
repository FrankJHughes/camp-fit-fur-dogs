'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { DogProfileCard } from '@/components/dogs/DogProfileCard';
import DogProfileActionsCard from '@/components/dogs/DogProfileActionsCard';
import { getDogProfileActions } from '@/lib/dogs/dogProfileActions';
import { useApiQuery } from '@/lib/hooks/useApiQuery';

export default function ViewDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const state = useApiQuery(
    () => getDogProfile(id).then(toQueryState),
    [id]
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <DogNotFound />;
  if (state.status === 'error') return <p>{state.error}</p>;

  const actions = getDogProfileActions(state.data.id, router.push);

  return (
    <>
      <DogProfileCard profile={state.data} />
      <DogProfileActionsCard actions={actions} />
    </>
  );
}