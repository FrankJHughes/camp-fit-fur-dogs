'use client';

import { useApiQuery } from '@/lib/hooks/useApiQuery';
import { getHealth } from '@/api/health/getHealth';
import { toQueryState } from '@/lib/api/queryResult';

export default function Home() {
  const state = useApiQuery(
    () => getHealth().then(toQueryState),
    []
  );

  let statusText = 'Checking...';
  let statusClass = 'status-checking';

  if (state.status === 'error') {
    statusText = 'Unreachable';
    statusClass = 'status-unreachable';
  } else if (state.status === 'success') {
    statusText = state.data.status;
    statusClass =
      state.data.status === 'Healthy'
        ? 'status-healthy'
        : 'status-unreachable';
  }

  return (
    <main className="health-container">
      <h1 className="health-title">API Health Status</h1>

      <div className="health-card">
        <span className="health-label">API Status:</span>
        <span className={`health-status ${statusClass}`}>{statusText}</span>
      </div>

      {state.status === 'error' && (
        <p className="health-error">
          The API is currently unreachable. This may be due to maintenance or a
          temporary outage.
        </p>
      )}
    </main>
  );
}
