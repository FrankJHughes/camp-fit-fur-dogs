'use client';
import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { getHealth } from '@/api/health/getHealth';
import { toQueryState } from '@/lib/api/queryResult';

type HealthStatus = 'Checking...' | 'Healthy' | 'Unreachable';

export default function Home() {

  const state = useApiQuery(
    () => getHealth().then(toQueryState),
    []
  );

  if (state.status === 'loading') return <p>Loading…</p>;
  if (state.status === 'not-found') return <p>Health Not Found</p>;
  if (state.status === 'error') return <p>{state.error}</p>;

  return (
    <main style={{ fontFamily: 'system-ui, sans-serif', padding: '2rem' }}>
      <h1>Camp Fit Fur Dogs</h1>
      <p>
        API Status:{' '}
        <span
          style={{
            fontWeight: 'bold',
            color: state.data.status === 'Healthy' ? 'green' : 'orange',
          }}
        >
          {state.data.status}
        </span>
      </p>
    </main>
  );
}
