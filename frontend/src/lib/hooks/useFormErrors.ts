import { useState } from 'react';

export function useFormErrors() {
  const [clientErrors, setClientErrors] = useState<Record<string, string>>({});

  const merge = (serverErrors?: Record<string, string>) => ({
    ...clientErrors,
    ...(serverErrors ?? {}),
  });

  const setClient = (errors: Record<string, string>) => setClientErrors(errors);

  const clear = () => setClientErrors({});

  return { clientErrors, setClient, merge, clear };
}
