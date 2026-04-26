'use client';

import { useParams, useRouter } from 'next/navigation';
import { getDogProfile, type DogProfile } from '@/api/dogs/getDogProfile';
import { DogProfileCard } from '@/components/dogs/DogProfileCard';
import DogProfileActionsCard from '@/components/dogs/DogProfileActionsCard';
import { getDogProfileActions } from '@/lib/dogs/dogProfileActions';
import { useApiQuery, type QueryState } from '@/lib/hooks/useApiQuery';

function toQueryState(result: Awaited<ReturnType<typeof getDogProfile>>): QueryState<DogProfile> {
  if (result.success) return { status: 'success', data: result.profile };
  if (result.notFound) return { status: 'not-found' };
  return { status: 'error', error: result.error };
}

export default function ViewDogProfilePage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();

  const state = useApiQuery<DogProfile>(
    () => getDogProfile(id).then(toQueryState),
    [id]
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <p>Dog not found.</p>;
  if (state.status === 'error') return <p>{state.error}</p>;

  const actions = getDogProfileActions(state.data.id, router.push);

  return (
    <>
      <DogProfileCard profile={state.data} />
      <DogProfileActionsCard actions={actions} />
    </>
  );
}