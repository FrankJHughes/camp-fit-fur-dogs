'use client';

import { ActionsCard } from '@/lib/components/ActionsCard';
import { login } from '@/api/login/login';
import { useState } from 'react';

export default function PublicLayout({
  children,
}: {
  children: React.ReactNode;
}) {

  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // ------------------------------------------------------------
  // Global authenticated action: Login
  // ------------------------------------------------------------
  async function handleLogin() {
    setError(null);
    setIsLoading(true);

    const result = await login();

    if (!result.success) {
      setError(result.error ?? 'Login failed');
      setIsLoading(false);
      return;
    }

    // Backend revokes the session and redirects to the logged out page.
  }

  return (
    <div className="authenticated-shell">
      <header className="shell-header">
        <h1>Camp Fit Fur Dogs</h1>

        {error && (
          <div className="error-banner">
            {error}
          </div>
        )}

        <ActionsCard
          actions={[
            {
              label: 'Login',
              variant: 'destructive',
              onClick: handleLogin,
            },
          ]}
        />
      </header>

      <main>{children}</main>
    </div>
  );
}
