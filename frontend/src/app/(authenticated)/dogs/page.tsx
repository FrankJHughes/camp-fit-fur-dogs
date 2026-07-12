'use client';

import { useMemo } from 'react';
import { useRouter } from 'next/navigation';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { listDogsByCurrentUser } from '@/api/dogs/listDogsByCurrentUser';
import { toQueryState } from '@/lib/api/queryResult';
import type { ListDogsByCurrentUserResponse } from '@/lib/dogs/dogModel';
import { ListDogsByCurrentUserCard } from '@/components/dogs/ListDogsByCurrentUserCard';
import { ActionsCard } from '@/lib/components/ActionsCard';
import type { Action } from '@/lib/action';

export default function DogsPage() {
  const router = useRouter();

  const actions: Action[] = useMemo(
    () => [{ label: 'Register Dog', onClick: () => router.push('/api/dogs') }],
    [router]
  );

  const state = useApiQuery<ListDogsByCurrentUserResponse>(
    async () => {
      const result = await listDogsByCurrentUser();
      return toQueryState(result);
    },
    []
  );

  // Loading
  if (state.status === 'loading') {
    return <p>Loading…</p>;
  }

  // Error
  if (state.status === 'error') {
    return (
      <p role="alert" aria-live="assertive" className="error-message">
        Something went wrong
      </p>
    );
  }

  // Unauthenticated
  if (state.status === 'unauthenticated') {
    return (
      <p role="alert" aria-live="assertive" className="error-message">
        You must be logged in to view your dogs.
      </p>
    );
  }

  // Not found
  if (state.status === 'not-found') {
    return <p className="empty-state">Not found.</p>;
  }

  // Success — safe to access state.data
  const dogs = state.data.dogs ?? [];

  return (
    <main className="page-container">
      <h1 className="page-title">Your Dogs</h1>

      <div className="card-section">
        {dogs.length === 0 ? (
          <p className="empty-state">No dogs registered yet.</p>
        ) : (
          <ListDogsByCurrentUserCard dogs={dogs} />
        )}
      </div>

      <div className="page-actions">
        <ActionsCard actions={actions} />
      </div>
    </main>
  );
}
