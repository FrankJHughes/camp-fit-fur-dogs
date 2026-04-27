'use client';

import { useRouter } from 'next/navigation';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { listDogsByCurrentUser } from '@/api/dogs/listDogsByCurrentUser';
import { toQueryState } from '@/lib/api/queryResult';
import type { ListDogsByCurrentUserResponse } from '@/api/dogs/listDogsByCurrentUser';
import { ListDogsByCurrentUserCard } from '@/components/dogs/ListDogsByCurrentUserCard';
import { ActionsCard } from '@/lib/components/ActionsCard';
import { Action } from '@/lib/action';

export default function DogsPage() {
  const router = useRouter();

  const actions: Action[] = [
    { label: 'Register', onClick: () => router.push('/dogs/register') },
  ];

  const state = useApiQuery<ListDogsByCurrentUserResponse>(
    async () => {
      const result = await listDogsByCurrentUser();
      return toQueryState(result);
    },
    []
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'error') return <p role="alert">{state.error}</p>;
  if (state.status === 'not-found') return <p>Not found.</p>;

  return (
    <>
      <ListDogsByCurrentUserCard dogs={state.data.dogs} />
      <ActionsCard actions={actions} />
    </>
  );
}
