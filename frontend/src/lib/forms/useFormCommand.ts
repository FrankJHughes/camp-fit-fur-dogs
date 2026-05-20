'use client';

import { useState } from 'react';
import type { CommandResult } from '@/lib/api/commandResult';
import type { FormCommand } from './formCommand';

export function useFormCommand<T>(options: {
  submit: (values: T) => Promise<CommandResult>;
  onSuccess?: () => void;
}): FormCommand<T> {
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
        const result = await options.submit(values);

        if (result.success) {
          options.onSuccess?.();
          return;
        }

        if (result.errors) {
          setErrors(result.errors);
        } else if (result.error) {
          setError(result.error);
        }
      } finally {
        setIsSubmitting(false);
      }
    },
  };
}
