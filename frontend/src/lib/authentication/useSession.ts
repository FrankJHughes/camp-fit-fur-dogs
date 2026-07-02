'use client';

import { getSession } from '@/api/authentication/getSession';
import { useEffect, useState } from 'react';

export function useSession() {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isUnavailable, setIsUnavailable] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  async function refresh() {
    setIsLoading(true);
    setError(null);
    setIsUnavailable(false);

    try {
      const result = await getSession();

      if (!result.success) {
        // API reachable but session invalid
        setIsAuthenticated(false);
        setError(result.error ?? 'Unable to determine session state');
        setIsLoading(false);
        return;
      }

      // API reachable and session valid
      setIsAuthenticated(result.data.isAuthenticated);
      setIsLoading(false);
    } catch {
      // ⭐ API unreachable → service unavailable
      setIsAuthenticated(false);
      setIsUnavailable(true);
      setError('authentication service unavailable');
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
    refresh,
  };
}
