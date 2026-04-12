'use client';

import { useEffect, useState } from 'react';

type HealthStatus = 'Checking...' | 'Healthy' | 'Unreachable';

export default function Home() {
  const [status, setStatus] = useState<HealthStatus>('Checking...');

  useEffect(() => {
    fetch('/health')
      .then((res) => res.json())
      .then((data) =>
        setStatus(data.status === 'Healthy' ? 'Healthy' : 'Unreachable')
      )
      .catch(() => setStatus('Unreachable'));
  }, []);

  return (
    <main style={{ fontFamily: 'system-ui, sans-serif', padding: '2rem' }}>
      <h1>Camp Fit Fur Dogs</h1>
      <p>
        API Status:{' '}
        <span
          style={{
            fontWeight: 'bold',
            color: status === 'Healthy' ? 'green' : 'orange',
          }}
        >
          {status}
        </span>
      </p>
    </main>
  );
}
