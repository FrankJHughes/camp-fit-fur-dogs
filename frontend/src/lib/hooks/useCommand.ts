import { useState } from 'react';

interface CommandResult {
  success: boolean;
  errors?: Record<string, string>;
}

export function useCommand<T>(
  commandFn: (data: T) => Promise<CommandResult>,
  onSuccess: () => void
) {
  const [errors, setErrors] = useState<Record<string, string> | undefined>();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (data: T) => {
    setIsSubmitting(true);
    setErrors(undefined);

    const result = await commandFn(data);

    if (result.success) {
      onSuccess();
    } else {
      setErrors(result.errors);
      setIsSubmitting(false);
    }
  };

  return { errors, isSubmitting, handleSubmit };
}