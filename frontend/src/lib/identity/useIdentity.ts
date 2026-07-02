'use client';

import { getIdentity } from '@/api/identity/getIdentity';
import { useEffect, useState } from 'react';

export function useIdentity() {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isUnavailable, setIsUnavailable] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [user, setUser] = useState<{ name: string } | null>(null);

  async function refresh() {
    setIsLoading(true);
    setError(null);
    setIsUnavailable(false);

    try {
      const result = await getIdentity();

      // ⭐ Network unreachable → service unavailable
      if (!result.success && result.error?.toLowerCase() === 'failed to fetch') {
        setIsAuthenticated(false);
        setIsUnavailable(true);
        setUser(null);
        setError(null); // do NOT leak "failed to fetch"
        setIsLoading(false);
        return;
      }

      if (!result.success) {
        // API reachable but session invalid
        setIsAuthenticated(false);
        setUser(null);
        setError(result.error ?? 'Unable to determine session state');
        setIsLoading(false);
        return;
      }

      // API reachable and session valid
      setIsAuthenticated(result.data.isAuthenticated);
      setUser(result.data.user ?? null);
      setIsLoading(false);
    } catch {
      // ⭐ Catch-all for network failures
      setIsAuthenticated(false);
      setIsUnavailable(true);
      setUser(null);
      setError(null);
      setIsLoading(false);
    }
  }

  useEffect(() => {
    refresh();
  }, []);

  return {
    isAuthenticated,
    isUnavailable,
    isLoading,
    error,
    user,
    refresh,
  };
}
