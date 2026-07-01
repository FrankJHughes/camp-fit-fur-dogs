'use client';

import { getSession } from '@/api/authentication/getSession';
import { useEffect, useState } from 'react';

export function useSession() {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  async function refresh() {
    setIsLoading(true);
    setError(null);

    const result = await getSession();

    if (!result.success) {
      setIsAuthenticated(false);
      setError(result.error ?? 'Unable to determine session state');
      setIsLoading(false);
      return;
    }

    setIsAuthenticated(result.data.isAuthenticated);
    setIsLoading(false);
  }

  useEffect(() => {
    refresh();
  }, []);

  return {
    isAuthenticated,
    isLoading,
    error,
    refresh,
  };
}
