'use client';

import { ActionsCard } from '@/lib/components/ActionsCard';
import { logout } from '@/api/logout/logout';
import { useState } from 'react';

export default function AuthenticatedLayout({
  children,
}: {
  children: React.ReactNode;
}) {

  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // ------------------------------------------------------------
  // Global authenticated action: Logout
  // ------------------------------------------------------------
  async function handleLogout() {
    setError(null);
    setIsLoading(true);

    const result = await logout();

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
              label: 'Logout',
              variant: 'destructive',
              onClick: handleLogout,
            },
          ]}
        />
      </header>

      <main>{children}</main>
    </div>
  );
}
