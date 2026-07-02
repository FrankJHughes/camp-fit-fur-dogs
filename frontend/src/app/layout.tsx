'use client';

import { ActionsCard } from '@/lib/components/ActionsCard';
import { login } from '@/api/authentication/login';
import { logout } from '@/api/authentication/logout';
import { useState } from 'react';
import { useIdentity } from '@/lib/identity/useIdentity';

import './globals.css';

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const {
    isAuthenticated,
    isUnavailable,
    error: sessionError,
    user,
  } = useIdentity();

  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  async function handleLogin() {
    setError(null);
    setIsLoading(true);

    const result = await login(window.location.href);

    if (!result.success) {
      setError(result.error ?? 'Login failed');
      setIsLoading(false);
      return;
    }
  }

  async function handleLogout() {
    setError(null);
    setIsLoading(true);

    const result = await logout(window.location.href);

    if (!result.success) {
      setError(result.error ?? 'Logout failed');
      setIsLoading(false);
      return;
    }
  }

  const bannerMessage =
    isUnavailable ? 'authentication service unavailable' : sessionError ?? error;

  const identityLabel = isAuthenticated
    ? `You are ${user?.name ?? 'anonymous'}.`
    : `You are anonymous.`;

  return (
    <html lang="en">
      <body className="app-root">
        <header className="app-header">
          <div className="header-left">
            <h1 className="app-title">Camp Fit Fur Dogs</h1>
          </div>

          <div className="header-right">
            {bannerMessage && (
              <div className="error-banner">{bannerMessage}</div>
            )}

            {!isUnavailable && (
              <div className="identity-wrapper">
                <span className="identity-label">{identityLabel}</span>

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
              </div>
            )}
          </div>
        </header>

        <main className="app-main">{children}</main>
      </body>
    </html>
  );
}
