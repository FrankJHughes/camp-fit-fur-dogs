'use client';

import { useState } from 'react';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/toCommandResult';
import type { FormCommand } from '@/lib/forms/formCommand';

const client = createApiClient();

export function useApiCommand<T>(url: string, onSuccess?: () => void): FormCommand<T> {
  const [errors, setErrors] = useState<Record<string, string> | undefined>();
  const [error, setError] = useState<string | undefined>();
  const [isSubmitting, setIsSubmitting] = useState(false);

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
        const cmd = toCommandResult(res);

        if (cmd.success) {
          onSuccess?.();
          return;
        }

        if (cmd.errors) setErrors(cmd.errors);
        else if (cmd.error) setError(cmd.error);
      } finally {
        setIsSubmitting(false);
      }
    },
  };
}
