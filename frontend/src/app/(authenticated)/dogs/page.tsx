// src/app/dogs/page.tsx
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
    () => [{ label: 'Register', onClick: () => router.push('/dogs/register') }],
    [router]
  );

  const state = useApiQuery<ListDogsByCurrentUserResponse>(
    async () => {
      const result = await listDogsByCurrentUser();
      return toQueryState(result);
    },
    []
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'error')
    return (
      <p role="alert" aria-live="assertive">
        {state.error}
      </p>
    );
  if (state.status === 'not-found') return <p>Not found.</p>;

  // state.status === 'success' here
  const dogs = state.data.dogs ?? [];

  return (
    <>
      {dogs.length === 0 ? (
        <p aria-live="polite">No dogs registered yet.</p>
      ) : (
        <ListDogsByCurrentUserCard dogs={dogs} />
      )}
      <ActionsCard actions={actions} />
    </>
  );
}
