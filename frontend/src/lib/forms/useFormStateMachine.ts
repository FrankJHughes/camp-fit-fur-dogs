// src/lib/forms/useFormStateMachine.ts
import { useCallback, useEffect, useRef, useState } from 'react';

type FormState<T> = {
  values: T;
  errors: Record<string, string> | undefined;
  isSubmitting: boolean;
};

export function useFormStateMachine<T extends Record<string, any>>(opts: {
  initialValues: T;
  externalErrors?: Record<string, string>;
  onSubmit?: (values: T) => Promise<Record<string, string> | void> | Record<string, string> | void;
}) {
  const [state, setState] = useState<FormState<T>>({
    values: opts.initialValues,
    errors: opts.externalErrors,
    isSubmitting: false,
  });

  const previousExternalErrors = useRef<Record<string, string> | undefined>(
    opts.externalErrors
  );

  useEffect(() => {
    const current = opts.externalErrors;
    const previous = previousExternalErrors.current;
    const same =
      JSON.stringify(previous ?? {}) === JSON.stringify(current ?? {});

    if (same) return;

    previousExternalErrors.current = current;
    setState((prev) => ({ ...prev, errors: current }));
  }, [opts.externalErrors]);

  // Flexible update handler: accepts either an event (e.target.value) or a raw value
  const update = useCallback(
    (field: keyof T) => (eOrValue: any) => {
      const value =
        eOrValue && typeof eOrValue === 'object' && 'target' in eOrValue
          ? (eOrValue.target as HTMLInputElement).value
          : eOrValue;

      setState((prev) => ({
        ...prev,
        values: { ...prev.values, [field]: value },
      }));
    },
    []
  );

  const handleSubmit = useCallback(
    async (e?: React.FormEvent) => {
      e?.preventDefault();
      if (!opts.onSubmit) return;
      setState((prev) => ({ ...prev, isSubmitting: true }));

      try {
        const result = await opts.onSubmit(state.values);

        if (result && Object.keys(result).length > 0) {
          setState((prev) => ({
            ...prev,
            errors: result,
            isSubmitting: false,
          }));
          return;
        }

        setState((prev) => ({
          ...prev,
          errors: undefined,
          isSubmitting: false,
        }));
      } finally {
        setState((prev) => ({ ...prev, isSubmitting: false }));
      }
    },
    [opts.onSubmit, state.values]
  );

  return {
    state,
    values: state.values,
    displayErrors: state.errors ?? {},
    update,
    handleSubmit,
    isSubmitting: state.isSubmitting,
  };
}
