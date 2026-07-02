'use client';

import { ActionsCard } from '@/lib/components/ActionsCard';
import { login } from '@/api/authentication/login';
import { logout } from '@/api/authentication/logout';
import { useState } from 'react';
import { useSession } from '@/lib/authentication/useSession';

import type { Metadata } from 'next';
import './globals.css';

export const metadata: Metadata = {
  title: 'Camp Fit Fur Dogs',
  description: 'Dog fitness camp management system',
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isAuthenticated, isUnavailable, error: sessionError } = useSession();

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

  return (
    <html>
      <body>
        <header className="shell-header">
          <h1>Camp Fit Fur Dogs</h1>

          {bannerMessage && (
            <div className="error-banner">{bannerMessage}</div>
          )}

          <ActionsCard
            actions={
              isUnavailable
                ? [] // No login/logout when API is down
                : isAuthenticated
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

        {children}
      </body>
    </html>
  );
}
