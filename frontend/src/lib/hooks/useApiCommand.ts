'use client';

import { useEffect, useRef, useState } from 'react';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { CommandResult } from '@/lib/api/commandResult';
import type { FormCommand } from '@/lib/forms/formCommand';

const client = createApiClient();

export function useApiCommand<T>(
  url: string,
  onSuccess?: () => void
): FormCommand<T> {
  const [errors, setErrors] = useState<Record<string, string> | undefined>();
  const [error, setError] = useState<string | undefined>();
  const [isSubmitting, setIsSubmitting] = useState(false);

  // mounted ref to avoid setState after unmount
  const mountedRef = useRef(true);
  useEffect(() => {
    return () => {
      mountedRef.current = false;
    };
  }, []);

  return {
    errors,
    error,
    isSubmitting,
    run: async (values: T) => {
      setIsSubmitting(true);
      setErrors(undefined);
      setError(undefined);

      try {
        const res = await client.post(url, values);

        let cmd: CommandResult;
        try {
          cmd = toCommandResult(res);
        } catch (err) {
          // If conversion fails, surface a generic form-level error
          if (mountedRef.current) setError('Unexpected server response');
          return;
        }

        if (cmd.success) {
          onSuccess?.();
          return;
        }

        if (cmd.errors) {
          if (mountedRef.current) setErrors(cmd.errors);
        } else if (cmd.error) {
          if (mountedRef.current) setError(cmd.error);
        }
      } catch (err: any) {
        // Network or unexpected error
        // eslint-disable-next-line no-console
        console.error('useApiCommand error', err);
        if (mountedRef.current) setError(err?.message ?? 'Network error');
      } finally {
        if (mountedRef.current) setIsSubmitting(false);
      }
    },
  };
}
