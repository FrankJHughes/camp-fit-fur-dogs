'use client';

import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { listDogs, type ListDogsResponse } from '@/api/dogs/listDogs';
import { toQueryState } from '@/lib/api/queryResult';
import Link from 'next/link';

export default function DogsPage() {
  const state = useApiQuery<ListDogsResponse>(
    async () => {
      const result = await listDogs();
      return toQueryState(result);
    },
    []
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'error') return <p role="alert">{state.error}</p>;

  const { dogs } = state.data;

  return (
    <div>
      <h1>My Dogs</h1>
      {dogs.length === 0 ? (
        <p>No dogs registered yet.</p>
      ) : (
        <ul>
          {dogs.map((dog) => (
            <li key={dog.id}>
              <Link href={`/dogs/${dog.id}`}>
                {dog.name} — {dog.breed}
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
