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

      // Network unreachable → service unavailable
      if (!result.success && 'error' in result && result.error.toLowerCase() === 'failed to fetch') {
        setIsAuthenticated(false);
        setIsUnavailable(true);
        setUser(null);
        setError(null); // do NOT leak "failed to fetch"
        setIsLoading(false);
        return;
      }

      // Unauthorized → anonymous user
      if (!result.success && 'unauthorized' in result) {
        setIsAuthenticated(false);
        setUser(null);
        setError(null);
        setIsLoading(false);
        return;
      }

      // Not found → treat as anonymous
      if (!result.success && 'notFound' in result) {
        setIsAuthenticated(false);
        setUser(null);
        setError(null);
        setIsLoading(false);
        return;
      }

      // Other API errors → anonymous
      if (!result.success && 'error' in result) {
        setIsAuthenticated(false);
        setUser(null);
        setError(result.error);
        setIsLoading(false);
        return;
      }

      // Success → authenticated
      setIsAuthenticated(true);
      setUser({ name: result.data.name });
      setIsLoading(false);

    } catch {
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
