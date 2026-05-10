import { useState } from 'react';
import { createApiClient } from '@/lib/api/client';
import { toCommandResult } from '@/lib/api/commandResult';

export function useApiCommand<T>(endpoint: string, onSuccess: () => void) {
  const [errors, setErrors] = useState<Record<string, string> | undefined>();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const submit = async (data: T) => {
    setIsSubmitting(true);
    setErrors(undefined);

    const client = createApiClient();
    const apiResult = await client.post(endpoint, data);
    const command = toCommandResult(apiResult);

    if (command.success) {
      setIsSubmitting(false);   // ⭐ FIX
      onSuccess();
    } else {
      setErrors(command.errors);
      setIsSubmitting(false);   // already correct
    }
  };

  return { submit, errors, isSubmitting };
}
