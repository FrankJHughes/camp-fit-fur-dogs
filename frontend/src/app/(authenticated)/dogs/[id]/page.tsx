'use client';

import React from 'react';
import { useParams, useRouter } from 'next/navigation';
import { getDogProfile } from '@/api/dogs/getDogProfile';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { DogProfileCard } from '@/components/dogs/DogProfileCard';
import { ActionsCard } from '@/lib/components/ActionsCard';
import { ConfirmDialog } from '@/lib/components/ConfirmDialog';
import { useRemoveDog } from '@/hooks/dogs/useRemoveDog';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import type { Action } from '@/lib/action';

export default function GetDogProfilePage() {
  const params = useParams<{ id?: string }>();
  const dogId = params?.id;
  const router = useRouter();

  // Guard early for missing id (keeps hooks stable)
  if (!dogId) {
    return <p>Invalid dog id</p>;
  }

  // Hooks must be called unconditionally and in the same order on every render
  const state = useApiQuery(() => getDogProfile(dogId).then(toQueryState), [dogId]);

  // Provide a stable name argument (empty string until we have data)
  const removeDog = useRemoveDog(
    dogId,
    state.status === 'success' ? state.data.name : '',
    (path: string) => router.push(path)
  );

  // Keep early returns consistent so hook order never changes
  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <DogNotFound />;
  if (state.status === 'error') return <p>{state.error}</p>;

  // Use a plain const for actions (no hooks) to avoid any hook-order surprises
  const actions: Action[] = [
    { label: 'Edit', onClick: () => router.push(`/dogs/${dogId}/edit`) },
    { label: 'Remove', onClick: removeDog.open },
  ];

  return (
    <>
      <DogProfileCard profile={state.data} />
      <ActionsCard actions={actions} />
      {removeDog.dialogProps && <ConfirmDialog {...removeDog.dialogProps} />}
      {removeDog.error && (
        <p role="alert" aria-live="polite">
          {removeDog.error}
        </p>
      )}
    </>
  );
}
