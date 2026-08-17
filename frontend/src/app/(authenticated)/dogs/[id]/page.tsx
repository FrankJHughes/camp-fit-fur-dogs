'use client';

import React from 'react';
import { useParams, useRouter } from 'next/navigation';
import { getDog } from '@/api/dogs/getDog';
import { toQueryState } from '@/lib/api/queryResult';
import { DogNotFound } from '@/components/dogs/DogNotFound';
import { DogCard } from '@/components/dogs/DogCard';
import { ActionsCard } from '@/lib/components/ActionsCard';
import { ConfirmDialog } from '@/lib/components/ConfirmDialog';
import { useRemoveDog } from '@/lib/dogs/useRemoveDog';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import type { Action } from '@/lib/action';

export default function GetDogPage() {
  const params = useParams<{ id?: string }>();
  const dogId = params?.id;
  const router = useRouter();

  if (!dogId) {
    return <p className="error-message">Invalid dog id</p>;
  }

  const state = useApiQuery(() => getDog(dogId).then(toQueryState), [dogId]);

  const removeDog = useRemoveDog(
    dogId,
    state.status === 'success' ? state.data.name : '',
    (path: string) => router.push(path)
  );

  if (state.status === 'loading') {
    return <p>Loading…</p>;
  }

  if (state.status === 'unauthenticated') {
    return (
      <p role="alert" aria-live="assertive" className="error-message">
        You must be logged in to view this dog.
      </p>
    );
  }

  if (state.status === 'not-found') {
    return <DogNotFound />;
  }

  if (state.status === 'error') {
    return (
      <p role="alert" aria-live="assertive" className="error-message">
        Something went wrong
      </p>
    );
  }

  const actions: Action[] = [
    { label: 'Edit', onClick: () => router.push(`/dogs/${dogId}/edit`) },
    { label: 'Remove', onClick: removeDog.open },
  ];

  return (
    <main className="page-container">
      <h1 className="page-title">{state.data.name}</h1>

      <div className="card-section">
        <DogCard profile={state.data} />
      </div>

      <div className="page-actions">
        <ActionsCard actions={actions} />
      </div>
      {removeDog.dialogProps && <ConfirmDialog {...removeDog.dialogProps} />}

      {removeDog.error && (
        <p role="alert" aria-live="assertive" className="error-message">
          {removeDog.error}
        </p>
      )}
    </main>
  );
}
