'use client';

import { ActionsCard } from '@/lib/components/ActionsCard';
import { login } from '@/api/authentication/login';
import { logout } from '@/api/authentication/logout';
import { useState } from 'react';
import { useSession } from '@/lib/authentication/useSession';

export default function PublicLayout({
  children,
}: {
  children: React.ReactNode;
}) {

  const { isAuthenticated } = useSession();

  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // ------------------------------------------------------------
  // Login
  // ------------------------------------------------------------
  async function handleLogin() {
    setError(null);
    setIsLoading(true);

    const result = await login(window.location.href);

    if (!result.success) {
      setError(result.error ?? 'Login failed');
      setIsLoading(false);
      return;
    }

    // Backend redirects to Auth0; callback returns to return_url.
  }

  // ------------------------------------------------------------
  // Logout
  // ------------------------------------------------------------
  async function handleLogout() {
    setError(null);
    setIsLoading(true);

    const result = await logout(window.location.href);

    if (!result.success) {
      setError(result.error ?? 'Logout failed');
      setIsLoading(false);
      return;
    }

    // Backend clears cookie; frontend handles navigation.
  }

  return (
    <div className="public-shell">
      <header className="shell-header">
        <h1>Camp Fit Fur Dogs</h1>

        {error && (
          <div className="error-banner">
            {error}
          </div>
        )}

        <ActionsCard
          actions={
            isAuthenticated
              ? [
                {
                  label: 'Logout',
                  variant: 'destructive',
                  onClick: handleLogout,
                },
              ]
              : [
                {
                  label: 'Login',
                  variant: 'primary',
                  onClick: handleLogin,
                },
              ]
          }
        />
      </header>

      <main>{children}</main>
    </div>
  );
}
